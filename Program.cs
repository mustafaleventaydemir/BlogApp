using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddControllersWithViews(); //controller'lar�n viewler ile ili�kilendirilmesi.

builder.Services.AddDbContext<BlogContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>(); //sanala kar��l�k ger�ek versiyonu g�nderdik.
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login"; //login olmad���m�z durumlarda y�nlendirece�i yeri belirttik. default olarak account/login geliyor
}); //cookie y�netece�imizi belirtiyoruz. bu �zelli�i tan�tt�k




var app = builder.Build();

app.UseRouting();
app.UseAuthentication(); //Cookie i�in bizi tan�mas�n� sa�l�yoruz.
app.UseAuthorization(); //yetkilendirme i�lemi. baz� b�l�mleri kullanmam�z� sa�l�yor.

SeedData.TestVerileriniDoldur(app); //Services'e ula��p i�inceki bilgileri al�yoruz. SeedData i�indekileri �a��rd�k

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


//Post detaylar� i�in
app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/details/{url}",//https://localhost:7028/posts/react-dersleri gibi istiyoruz
    defaults: new { controller = "Posts", action = "Details" });
//https://localhost:7028/Posts/Details/1 <= id yerine biz tablodaki url olarak tan�nlad���m�z sat�r� getirmek istiyoruz. 


//Tag detaylar� i�in
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
//https://localhost:7028/Posts/Details/1 <= id yerine biz tablodaki url olarak tan�nlad���m�z sat�r� getirmek istiyoruz. 

app.MapDefaultControllerRoute(); //aktif ettik

app.Run();
