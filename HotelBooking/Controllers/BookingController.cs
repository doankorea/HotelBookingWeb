using Azure;
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
        HotelDbContext db = new HotelDbContext();


        public IActionResult Reservation(int? page)
        {
            // Lấy UserID từ session và kiểm tra session có dữ liệu hay không.
            string userIDStr = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(userIDStr))
            {
                // Nếu không có UserID trong session, chuyển hướng về trang đăng nhập hoặc thông báo lỗi.
                return RedirectToAction("Login", "Login");
            }

            // Chuyển đổi userID từ string sang int
            int userID = int.Parse(userIDStr);
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstReservation = db.Reservations
    .Where(rsr => rsr.UserId == userID)
    .OrderByDescending(rsr => rsr.BookingDate)
    .ToList();

            ViewBag.RoomID = new SelectList(db.Rooms.ToList(), "RoomID", "RoomName");
            ViewBag.User = new SelectList(db.Users.ToList(), "UserID", "Email");
            PagedList<Reservation> rs = new PagedList<Reservation>(lstReservation.AsQueryable(), pageNumber, pageSize);
            // Lấy thông tin reservation từ database
            // Kiểm tra nếu không có kết quả nào được tìm thấy
            if (rs == null)
            {
                // Thực hiện xử lý khi không có reservation, ví dụ trả về một view thông báo không có dữ liệu.
                return View("NoReservation");
            }

            // Trả về view với kết quả tìm được
            return View(rs);
        }

        [Route("DeleteReservation")]
        [HttpGet]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                // Nếu không tìm thấy đặt chỗ, có thể chuyển đến một trang thông báo lỗi
                return NotFound();
            }

            // Cập nhật trạng thái đặt chỗ thành "canceled"
            reservation.Status = "Cancelled"; // Giả sử bạn có thuộc tính Status trong model của bạn

            // Lưu thay đổi vào cơ sở dữ liệu
            await db.SaveChangesAsync();

            return RedirectToAction("Reservation");
        }

        public IActionResult Booking(string roomID)
        {
            var room = db.Rooms.SingleOrDefault(r => r.RoomName == roomID);
            var reservations = db.Reservations
                         .Where(r => r.RoomId == room.RoomId)
                         .Select(r => new { r.CheckInDate, r.CheckOutDate })
                         .ToList();
            var bookedDates = new List<string>();

            foreach (var reservation in reservations)
            {
                DateTime checkIn = reservation.CheckInDate ?? DateTime.MinValue;
                DateTime checkOut = reservation.CheckOutDate ?? DateTime.MinValue;

                // Add all dates between check-in and check-out to bookedDates
                for (var date = checkIn; date <= checkOut; date = date.AddDays(1))
                {
                    bookedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            ViewBag.ReservedDates = bookedDates;
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
                ViewBag.HotelID = new SelectList(db.Hotels.ToList(), "HotelID", "HotelName");

                // Truy vấn để lấy danh sách phòng dựa trên điều kiện tìm kiếm
                var roomsQuery =
                    from r in db.Rooms
                    join c in db.Countries on r.CountryId equals c.CountryId
                    join s in db.States on c.CountryId equals s.CountryId
                    join rt in db.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
                    join h in db.Hotels on r.HotelID equals h.HotelId
                    join rs in db.Reservations on r.RoomId equals rs.RoomId into roomReservations
                    from res in roomReservations.DefaultIfEmpty()
                    where
                        (string.IsNullOrWhiteSpace(location) ||
                         c.CountryName.Contains(location) ||
                         s.StateName.Contains(location))
                        &&
                        (res == null ||
                         (!checkin.HasValue ||
                          !checkout.HasValue ||
                          !(res.CheckInDate < checkout && checkin < res.CheckOutDate)))
                        &&
                        (string.IsNullOrWhiteSpace(room) || rt.TypeName == room)
                        &&
                        r.Status == "Available"
    select new RoomSearch
    {
        RoomName = r.RoomName,
        CountryName = c.CountryName,
        StateName = s.StateName,
        Price = r.Price,
        Image = r.Image,
        HotelName = h.HotelName,
    };

                roomsQuery = roomsQuery.Distinct();

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
