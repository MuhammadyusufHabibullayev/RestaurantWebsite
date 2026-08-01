using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using RestaurantWebsite.Models;

namespace RestaurantWebsite.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceProvider _services;
        private readonly string _botToken;


        public TelegramBotService(IServiceProvider services, IConfiguration configuration)
        {
            _botToken = configuration["TelegramBot:Token"]!;
            _botClient = new TelegramBotClient(_botToken);
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );

            await Task.Delay(-1, stoppingToken);
        }

        private InlineKeyboardMarkup MainMenu()
        {
            return new InlineKeyboardMarkup(new[]
            {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📋 Menyu", "menu"),
            InlineKeyboardButton.WithCallbackData("📍 Manzil", "location")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("☎️ Aloqa", "contact"),
            InlineKeyboardButton.WithCallbackData("🪑 Stol bron qilish", "reserve")
        }
    });
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            
            if (update.CallbackQuery is { } callback)
            {
                var chatId = callback.Message!.Chat.Id;

                if (callback.Data == "menu")
                {
                    var items = context.MenuItems.ToList();
                    string menuText = "🍽 *Bizning menyu:*\n\n";
                    foreach (var item in items)
                    {
                        menuText += $"• {item.Name} — {item.Price:N0} so'm\n  {item.Description}\n\n";
                    }
                    await botClient.SendMessage(chatId, menuText, cancellationToken: ct);
                }
                else if (callback.Data == "location")
                {
                    await botClient.SendMessage(chatId,
                        "📍 Manzil: Toshkent shahri, Chilonzor tumani, 33-uy\n🕒 Ish vaqti: Har kuni 09:00 - 23:00",
                        cancellationToken: ct);
                }
                else if (callback.Data == "contact")
                {
                    await botClient.SendMessage(chatId,
                        "☎️ Telefon: +998 90 123 45 67\nSavolingiz bo'lsa shu yerga yozing, tez orada javob beramiz.",
                        cancellationToken: ct);
                }

                else if (callback.Data == "reserve")
                {
                    await botClient.SendMessage(chatId,
                    "🪑 Stol bron qilish uchun quyidagi formatda yozing:\n\n" +
                    "Ismingiz, Telefon, Sana (kun.oy.yil), Soat, Odamlar soni\n\n" +
                    "Masalan:\nAziz, +998901234567, 02.08.2026, 19:00, 4",
                    cancellationToken: ct);
                }
            
                     

                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
                return;
            }
 

            if (update.Message is not { } message) return;
            if (message.Text is not { } text) return;

            var msgChatId = message.Chat.Id;

            if (text == "/start")
            {
                await botClient.SendMessage(msgChatId,
                    "Assalomu alaykum! Mazza Restoraniga xush kelibsiz 🍕\nKerakli bo'limni tanlang:",
                    replyMarkup: MainMenu(),
                    cancellationToken: ct);
                return;
            }

            string lowerText = text.ToLower();

            if (lowerText.Contains("menyu") || lowerText.Contains("narx"))
            {
                var items = context.MenuItems.ToList();
                string menuText = "🍽 *Bizning menyu:*\n\n";
                foreach (var item in items)
                {
                    menuText += $"• {item.Name} — {item.Price:N0} so'm\n  {item.Description}\n\n";
                }
                await botClient.SendMessage(msgChatId, menuText, cancellationToken: ct);
                return;
            }

            if (lowerText.Contains("manzil") ||  lowerText.Contains("qayerda") || lowerText.Contains("ish vaqti"))
            {
                await botClient.SendMessage(msgChatId,
                    "📍 Manzil: Toshkent shahri, Chilonzor tumani, 33-uy\n🕒 Ish vaqti: Har kuni 09:00 - 23:00",
                    cancellationToken: ct);
                return;
            }

            if (lowerText.Contains("aloqa") || lowerText.Contains("telefon") || lowerText.Contains("raqam"))
            {
                await botClient.SendMessage(msgChatId,
                    "☎️ Telefon: +998 90 123 45 67",
                    cancellationToken: ct);
                return;
            }

            if (text.Contains(",") && text.Split(",").Length == 5)
            {
                var parts = text.Split(",");
                string name = parts[0].Trim();
                string phone = parts[1].Trim();
                string dateStr = parts[2].Trim();
                string time = parts[3].Trim();
                string guestStr = parts[4].Trim();

                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime resDate)
                    && int.TryParse(guestStr, out int guestCount))
                {
                    var reservation = new TableReservation
                    {
                        Name = name,
                        Phone = phone,
                        ReservationDate = resDate,
                        Time = time,
                        GuestCount = guestCount,
                        ChatId = msgChatId
                    };
                    context.TableReservations.Add(reservation);
                    await context.SaveChangesAsync();

                    await botClient.SendMessage(msgChatId,
                    $"✅ Bron qilindi!\n👤 {name}\n📅 {dateStr} soat {time}\n👥 {guestCount} kishi\n\nTasdiqlanishini kuting.",
                    cancellationToken: ct);
                            return;
                }
            }

            var msg = new ContactMessage
            {
                Name = message.Chat.FirstName ?? "Noma'lum",
                Phone = "Telegram orqali",
                Message = text,
                ChatId = msgChatId
            };
            context.ContactMessages.Add(msg);
            await context.SaveChangesAsync();

            await botClient.SendMessage(msgChatId,
                "Xabaringiz qabul qilindi! Tez orada javob beramiz.",
                cancellationToken: ct);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}