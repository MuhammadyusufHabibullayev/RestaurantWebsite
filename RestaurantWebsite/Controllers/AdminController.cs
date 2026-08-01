using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RestaurantWebsite.Models;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace RestaurantWebsite.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == _configuration["AdminSettings:password"])
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Parol noto'g'ri";
            return View();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var items = _context.MenuItems.ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult Add(string name, string description, decimal price, string imageUrl)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var item = new MenuItem { Name = name, Description = description, Price = price, ImageUrl = imageUrl ?? "" };
            _context.MenuItems.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var item = _context.MenuItems.Find(id);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var item = _context.MenuItems.Find(id);
            if (item == null)
                return RedirectToAction("Index");

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(int id, string name, string description, decimal price, string imageUrl)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var item = _context.MenuItems.Find(id);
            if (item != null)
            {
                item.Name = name;
                item.Description = description;
                item.Price = price;
                item.ImageUrl = imageUrl ?? "";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Messages()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var messages = _context.ContactMessages.OrderByDescending(m => m.CreatedDate).ToList();
            return View(messages);
        }

        public IActionResult MarkAsRead(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var msg = _context.ContactMessages.Find(id);
            if (msg != null)
            {
                msg.IsRead = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Messages");
        }

        public IActionResult DeleteMessage(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var msg = _context.ContactMessages.Find(id);
            if (msg != null) ;
            {
                _context.ContactMessages.Remove(msg);
                _context.SaveChanges();
            }
            return RedirectToAction("Messages");
        }
        [HttpPost]
        public async Task<IActionResult> Reply(int id, string responseText)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var msg = _context.ContactMessages.Find(id);
            if (msg != null && msg.ChatId.HasValue)
            {
                string botToken = _configuration["TelegramBot:Token"]!;
                string text = $"Admin javobi:\n{responseText}";
                string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={msg.ChatId}&text={Uri.EscapeDataString(text)}";

                using var client = new HttpClient();
                await client.GetAsync(url);
            }
            return RedirectToAction("Messages");
        }
        public IActionResult Reservations()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var reservations =_context.TableReservations.OrderByDescending(r => r.CreateedDate).ToList();
            return View(reservations);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmReservation(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login");

            var reservation = _context.TableReservations.Find(id);
            if (reservation != null)
            {
                reservation.IsConfirmed = true;
                _context.SaveChanges();

                string botToken = _configuration["TelegramBot:Token"]!;
                string text = $"✅ Broningiz tasdiqlandi!\n📅 {reservation.ReservationDate:dd.MM.yyyy} soat {reservation.Time}\n👥 {reservation.GuestCount} kishi\n\nSizni kutamiz!";
                string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={reservation.ChatId}&text={Uri.EscapeDataString(text)}";

                using var client = new HttpClient();
                await client.GetAsync(url);
            }

            return RedirectToAction("Reservations");
        }
    }
}