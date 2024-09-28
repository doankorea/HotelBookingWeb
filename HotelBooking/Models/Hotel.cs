namespace HotelBooking.Models
{
    public class Hotel
    {
        public int HotelId { get; set; } 
        public string HotelName { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
