using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddControllersWithViews(); //controller'larýn viewler ile iliþkilendirilmesi.

builder.Services.AddDbContext<BlogContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>(); //sanala karþýlýk gerçek versiyonu gönderdik.
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login"; //login olmadýðýmýz durumlarda yönlendireceði yeri belirttik. default olarak account/login geliyor
}); //cookie yöneteceðimizi belirtiyoruz. bu özelliði tanýttýk




var app = builder.Build();

app.UseRouting();
app.UseAuthentication(); //Cookie için bizi tanýmasýný saðlýyoruz.
app.UseAuthorization(); //yetkilendirme iþlemi. bazý bölümleri kullanmamýzý saðlýyor.

SeedData.TestVerileriniDoldur(app); //Services'e ulaþýp içinceki bilgileri alýyoruz. SeedData içindekileri çaðýrdýk

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//Post detaylarý için
app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/details/{url}",//https://localhost:7028/posts/react-dersleri gibi istiyoruz
    defaults: new { controller = "Posts", action = "Details" });
//https://localhost:7028/Posts/Details/1 <= id yerine biz tablodaki url olarak tanýnladýðýmýz satýrý getirmek istiyoruz. 


//Tag detaylarý için
app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{tag}",//https://localhost:7028/posts/tag/react-dersleri gibi istiyoruz
    defaults: new { controller = "Posts", action = "Index" });


app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" });



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); //https://localhost:7028/Post/react-dersleri gibi istiyoruz
//https://localhost:7028/Posts/Details/1 <= id yerine biz tablodaki url olarak tanýnladýðýmýz satýrý getirmek istiyoruz. 

app.MapDefaultControllerRoute(); //aktif ettik

app.Run();
