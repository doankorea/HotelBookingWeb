using HotelBooking.Models;
using HotelBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace HotelBooking.Controllers
{
    public class RoomsController : Controller
    {
        HotelDbContext db = new HotelDbContext();


        public IActionResult Room(int? page)
        {
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
            ViewBag.HotelID = new SelectList(db.Hotels.ToList(), "HotelID", "HotelName");
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstroom = db.Rooms.ToList();
            PagedList<Room> lst = new PagedList<Room>(lstroom, pageNumber, pageSize);
            return View(lst);
        }
        public IActionResult GetAllCountry()
        {

            var lstcountry = db.Countries.ToList();
            return Json(lstcountry);
        }
    }
}