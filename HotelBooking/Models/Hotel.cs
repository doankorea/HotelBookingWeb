namespace HotelBooking.Models
{
    public class Hotel
    {
        public int HotelId { get; set; } 
        public string HotelName { get; set; } 
        public string Location { get; set; } 
        public double Rating { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
