using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestaurantWebsite.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlite("Data Source=restaurant.db"));
builder.Services.AddSession();
builder.Services.AddHostedService<RestaurantWebsite.Services.TelegramBotService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.MenuItems.Any())
    {
        db.MenuItems.AddRange(
            new MenuItem { Name = "Osh", Description = "An'anaviy o'zbek oshi, go'sht va sabzi bilan", Price = 35000 },
            new MenuItem { Name = "Shashlik", Description = "Cho'g'da pishirilgan mol go'shti shashlik", Price = 40000 },
            new MenuItem { Name = "Manti", Description = "Bug'da pishirilgan mayin manti", Price = 25000 }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
