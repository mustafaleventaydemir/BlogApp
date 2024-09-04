using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace BlogApp.Data.Concrete.EfCore
{
    public class SeedData
    {
        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();

            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any()) //bir ya da birden fazla migration varsa uygula
                {
                    context.Database.Migrate(); //uygulama her çalıştırıldığında database tekrar oluşturulsun.
                }

                if (!context.Tags.Any()) //Tags tablosunda herhangi bir kayıt yoksa
                {
                    context.Tags.AddRange(
                        new Entity.Tag { Text = "web programlama", Url = "web-programlama", Colors = TagColors.warning },
                        new Entity.Tag { Text = "backend", Url = "backend", Colors = TagColors.info },
                        new Entity.Tag { Text = "frontend", Url = "frontend", Colors = TagColors.success },
                        new Entity.Tag { Text = "fullstack", Url = "fullstack", Colors = TagColors.secondary },
                        new Entity.Tag { Text = "php", Url = "php", Colors = TagColors.primary }
                        );
                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new Entity.User { UserName = "leventaydemir", Name = "Levent Aydemir", Email = "levent.ydmr@gmail.com", Password = "123456", Image = "p1.jpg" },
                        new Entity.User { UserName = "emrenefesli", Name = "Emre Nefesli", Email = "emrenefesli@gmail.com", Password = "123456", Image = "p2.jpg" }
                        );
                    context.SaveChanges();
                }

                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Entity.Post
                        {
                            Title = "Asp.Net Core",
                            Description = "Asp.Net Core Dersleri",
                            Content = "Asp.net core dersleri",
                            Url = "aspnet-core",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(3).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "1.jpg",
                            UserId = 1,
                            Comments = new List<Comment> {
                                new Comment { Text="İyi bir kurs", PublishedOn= DateTime.Now.AddDays(-20),UserId=1},
                                new Comment { Text="Çok faydalandığım bir kurs", PublishedOn= DateTime.Now.AddDays(-10),UserId=2},
                            }
                        },
                        new Entity.Post
                        {
                            Title = "Php",
                            Description = "Php dersleri",
                            Content = "Php dersleri",
                            Url = "php",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(2).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "2.jpg",
                            UserId = 1
                        },
                        new Entity.Post
                        {
                            Title = "Django",
                            Description = "Django dersleri",
                            Content = "Django dersleri",
                            Url = "django",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-30),
                            Tags = context.Tags.Take(4).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "3.jpg",
                            UserId = 2
                        },
                        new Entity.Post
                        {
                            Title = "React",
                            Description = "React Dersleri",
                            Content = "React dersleri",
                            Url = "react-dersleri",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-40),
                            Tags = context.Tags.Take(4).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "3.jpg",
                            UserId = 2
                        },
                        new Entity.Post
                        {
                            Title = "Angular",
                            Description = "Angular Dersleri",
                            Content = "Angular dersleri",
                            Url = "angular-dersleri",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-35),
                            Tags = context.Tags.Take(4).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "3.jpg",
                            UserId = 1
                        },
                        new Entity.Post
                        {
                            Title = "Web Tasarım",
                            Description = "Web tasarım dersleri",
                            Content = "Web tasarım dersleri",
                            Url = "web-tasarim",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-65),
                            Tags = context.Tags.Take(4).ToList(), //veritabanında olan ilk üç taneyi al
                            Image = "3.jpg",
                            UserId = 2
                        }
                        );
                    context.SaveChanges(); //çağırmak için program.cs gidiyoruz.
                }
            }
        }
    }
}