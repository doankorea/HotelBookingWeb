using HotelBooking.Models;
using HotelBooking.Models_View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;
using X.PagedList;

namespace HotelBooking.Controllers
{
    public class BookingController : Controller
    {
          HotelDbContext db=new HotelDbContext();



        public IActionResult Booking(string roomID)
        {
            var room = db.Rooms.SingleOrDefault(r => r.RoomName == roomID);
            return View(room);
        }
        public IActionResult RoomSearch(int? page, string location, DateTime? checkin, DateTime? checkout, string? room)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1; // Sử dụng toán tử null-coalescing

            try
            {
                // Lưu thông tin tìm kiếm vào ViewBag
                ViewBag.location = location;
                ViewBag.checkin = checkin;
                ViewBag.checkout = checkout;
                ViewBag.room = room;

                // Lấy danh sách RoomTypes và Countries để đổ vào ViewBag
                ViewBag.RoomTypes = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
                ViewBag.Countries = new SelectList(db.Countries.ToList(), "CountryId", "CountryName");

                // Truy vấn để lấy danh sách phòng dựa trên điều kiện tìm kiếm
                var roomsQuery = (from r in db.Rooms
                                  join c in db.Countries on r.CountryId equals c.CountryId
                                  join s in db.States on c.CountryId equals s.CountryId
                                  join rt in db.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
                                  join rs in db.Reservations on r.RoomId equals rs.RoomId into roomReservations
                                  join h in db.Hotels on r.HotelID equals h.HotelId
                                  from res in roomReservations.DefaultIfEmpty()
                                  where (string.IsNullOrWhiteSpace(location) || c.CountryName.Contains(location) || s.StateName.Contains(location))
                                  && (res == null || (!checkin.HasValue || !checkout.HasValue || !(res.CheckInDate < checkout && checkin < res.CheckOutDate)))
                                  && (string.IsNullOrWhiteSpace(room) || rt.TypeName == room)
                                  && r.Status == "Available"
                                  select new RoomSearch
                                  {
                                      RoomName = r.RoomName,
                                      CountryName = c.CountryName,
                                      StateName = s.StateName,
                                      Price = r.Price,
                                      Image = r.Image,
                                      HotelName= h.HotelName,
                                     
                                  }).Distinct();

                // Phân trang
                var pagedRooms = new PagedList<RoomSearch>(roomsQuery.AsQueryable(), pageNumber, pageSize);


                return View(pagedRooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
