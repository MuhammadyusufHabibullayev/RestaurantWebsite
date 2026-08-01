using Microsoft.AspNetCore.Mvc;
using RestaurantWebsite.Models;
using System.Net.Http;
using System.Diagnostics;

namespace RestaurantWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Menu()
        {
            var items = _context.MenuItems.ToList();
            return View(items);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(string name, string phone, string message)
        {
            var msg = new ContactMessage { Name = name, Phone = phone, Message = message };
            _context.ContactMessages.Add(msg);
            _context.SaveChanges();

            string botToken = _configuration["TelegramBot:Token"]!;
            string chatId = "7795814652";
            string text = $"Yangi xabar!%0AIsm: {name}%0ATelefon: {phone}%0AXabar: {message}";
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={text}";

            using (var client = new HttpClient())
            {
                await client.GetAsync(url);
            }

            ViewBag.Success = true;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
