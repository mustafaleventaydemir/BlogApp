using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private IPostRepository _postRepository; //interface üzerinden nesne talep ettiğimde EfPostRepository getiriliyor.
        private ICommentRepository _commentRepository;
        private ITagRepository _tagRepository;
        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }


        public async Task<IActionResult> Index(string tag)
        {
            var claims = User.Claims; //cookie üzerinde saklanacak bilgiler.

            var posts = _postRepository.Posts.Where(i => i.IsActive); //sadece true olanlar
            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return View(
                new PostsViewModel
                {
                    Posts = await posts.ToListAsync()
                });
        }

        public async Task<IActionResult> Details(string url)
        {
            return View(await _postRepository
                .Posts.Include(x => x.Tags) //detay sayfasına gidince tam kaydın tag'lerini
                .Include(x => x.User) //post'taki user bilgisi
                .Include(x => x.Comments) //hemde kaydın yorumlarını görebileceğiz.
                .ThenInclude(x => x.User) //her comments alırken commentteki user'inide getirir.
                .FirstOrDefaultAsync(p => p.Url == url));
        }

        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //claimstype'in Nameidentifier'ine göre User'dan değeri alır.
            var username = User.FindFirstValue(ClaimTypes.Name); //aynı şekilde kullanıcı adınıda alırız
            var avatar = User.FindFirstValue(ClaimTypes.UserData);
            var entity = new Comment
            {
                PostId = PostId,
                Text = Text,
                PublishedOn = DateTime.Now,
                UserId = int.Parse(userId ?? "")
            };
            _commentRepository.CreateComment(entity);
            //return Redirect("/posts/details/" + Url); //sayfayı yeniliyoruz.
            // return RedirectToRoute("post_details", new { url = Url }); //post-details'i program.cs'de yazdığımız controllerroute'dan aldık. name'i = post_details
            return Json(new
            {
                username,
                Text,
                entity.PublishedOn,
                avatar
            });
        }

        [Authorize] //login olmadığında bizi account/login'e yönlendiriyor. bunu program.cs'ten düzeletelim
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //claimstype'in Nameidentifier'ine göre User'dan değeri alır.
                _postRepository.CreatePost(
                    new Post
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Url = model.Url,
                        UserId = int.Parse(userId ?? ""),
                        PublishedOn = DateTime.Now,
                        Image = "p1.jpg",
                        IsActive = false,
                    });
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""); //claimstype'in Nameidentifier'ine göre User'dan değeri alır.
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;
            if (string.IsNullOrEmpty(role)) //bir role claim'i gelmiyorsa
            {
                posts = posts.Where(i => i.UserId == userId); //sadece kendi id'si ile eşleşeni alsın
            }
            return View(await posts.ToListAsync());
        }


        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _postRepository.Posts.Include(i => i.Tags).FirstOrDefault(i => i.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Tags = _tagRepository.Tags.ToList();

            return View(new PostCreateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive,
                Tags = post.Tags
            });
        }

        [HttpPost]
        public IActionResult Edit(PostCreateViewModel model, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var entityToUpdate = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url,
                    IsActive = model.IsActive
                };

                if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    entityToUpdate.IsActive = model.IsActive;
                }
                _postRepository.EditPost(entityToUpdate, tagIds);
                return RedirectToAction("List");
            }
            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View(model);
        }
    }
}
