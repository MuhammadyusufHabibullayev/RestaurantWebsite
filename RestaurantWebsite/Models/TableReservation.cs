namespace RestaurantWebsite.Models
{
    public class TableReservation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public string Time {  get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public long ChatId {  get; set; }
        public DateTime CreateedDate { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; } = false;
    }
}
