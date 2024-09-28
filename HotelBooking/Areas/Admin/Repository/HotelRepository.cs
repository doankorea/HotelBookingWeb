using HotelBooking.Models;

namespace HotelBooking.Areas.Admin.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelDbContext _context;
        public HotelRepository(HotelDbContext context)
        {
            _context = context;
        }
        public Hotel Add(Hotel HotelName)
        {
            _context.Hotels.Add(HotelName);
            _context.SaveChanges();
            return HotelName;
        }

        public Hotel Delete(int HotelID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hotel> GetAllHotel()
        {
            return _context.Hotels;
        }

        public Hotel GetHotelName(int HotelID)
        {
            return _context.Hotels.Find(HotelID);
        }

        public Hotel Update(Hotel HotelName)
        {
            _context.Update(HotelName);
            _context.SaveChanges(true);
            return HotelName;
        }
    }
}
