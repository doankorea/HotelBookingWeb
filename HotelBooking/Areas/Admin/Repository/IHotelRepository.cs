using HotelBooking.Models;
namespace HotelBooking.Areas.Admin.Repository
{
    public interface IHotelRepository
    {
        Hotel Add(Hotel HotelName);
        Hotel Update(Hotel HotelName);
        Hotel Delete(int HotelID);
        Hotel GetHotelName(int HotelID);
        IEnumerable<Hotel> GetAllHotel();
    }
}
