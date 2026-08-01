namespace RestaurantWebsite.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public long? ChatId { get; set; }
    }
}
