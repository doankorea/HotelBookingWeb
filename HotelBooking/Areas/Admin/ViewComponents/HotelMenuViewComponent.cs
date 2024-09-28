using HotelBooking.Areas.Admin.Repository;
using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.Areas.Admin.ViewComponents
{
    public class HotelMenuViewComponent: ViewComponent
    {
        private readonly IHotelRepository _hotel;
        public HotelMenuViewComponent(IHotelRepository hotelRepository)
        {
            _hotel = hotelRepository;
        }
        public IViewComponentResult Invoke()
        {
            var hotel = _hotel.GetAllHotel().OrderBy(x => x.HotelName);
            return View(hotel);
        }
    }
}
