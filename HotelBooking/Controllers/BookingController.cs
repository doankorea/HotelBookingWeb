using HotelBooking.Models;
using HotelBooking.Models_View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;

namespace HotelBooking.Controllers
{
    public class BookingController : Controller
    {
          HotelDbContext db=new HotelDbContext();
        public IActionResult Index(string location)
        {
            var rooms = (from r in db.Rooms
                         join c in db.Countries on r.CountryId equals c.CountryId
                         join s in db.States on c.CountryId equals s.CountryId
                         where c.CountryName.Contains(location)
                         select new 
                         {
                             r.RoomName,
                             c.CountryName,
                             r.Price,
                             s.StateName,
                             r.Image
                         }).ToList();
            List<RoomSearch> lroom = rooms.Select(r=> new RoomSearch{

                            RoomName= r.RoomName,
                            CountryName= r.CountryName,
                            Price= r.Price,
                            StateName= r.StateName,
                            Image= r.Image
                         }).ToList();
            
            return View(lroom);
        }
    }
}
