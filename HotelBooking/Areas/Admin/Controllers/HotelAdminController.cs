using HotelBooking.Models;
using HotelBooking.Models.Authentication;
using HotelBooking.Models_View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using X.PagedList;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Authentication]
    [Area("admin")]
    [Route("HotelAdmin")]
    [Route("Admin/HotelAdmin")]
    public class HotelAdminController : Controller
    {
        
        HotelDbContext db = new HotelDbContext();
        [Route("")]
        [Route("index")]
        [Route("room")]
        public IActionResult RoomManagement(int? page, int hoteladminID, string searchString)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var roomsQuery = (from r in db.Rooms
                              join h in db.Hotels on r.HotelID equals h.HotelId
                              join u in db.Users on h.HotelId equals u.HotelID
                              where u.UserId == hoteladminID
                              select r).ToList();
            var hotelID = db.Users
                   .Where(u => u.UserId == hoteladminID)
                   .Select(u => u.HotelID)
                   .FirstOrDefault();
            if (!string.IsNullOrEmpty(searchString))
            {
                roomsQuery = roomsQuery
                .Where(n => n.RoomName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            }
            if (!roomsQuery.Any())
            {
                ViewBag.Message = "No rooms found matching your search criteria.";
            }
            var pagedRooms = new PagedList<Room>(roomsQuery.AsQueryable(), pageNumber, pageSize);
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
            ViewBag.HotelID = hotelID;
            ViewBag.hoteladminID = hoteladminID;
            
            return View(pagedRooms);
        }
        [Route("Addnewroom")]
        [HttpGet]
        public IActionResult Addnewroom(int HotelID)
        {
            // Lấy danh sách RoomTypes
            var roomTypes = db.RoomTypes
                .Select(rt => new
                {
                    RoomTypeId = rt.RoomTypeId,
                    TypeName = rt.TypeName
                }).ToList();

            ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeId", "TypeName");

            // Lấy danh sách Countries
            var countries = db.Countries
                .Select(ct => new
                {
                    CountryId = ct.CountryId,
                    CountryName = ct.CountryName
                }).ToList();
            ViewBag.Countries = new SelectList(countries, "CountryId", "CountryName");
            ViewBag.HotelID = HotelID;
            return View();
        }

        [Route("Addnewroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addnewroom(Room room, IFormFile imageFile, int HotelID)
        {

            try
            {
                string filename = Path.GetFileName(imageFile.FileName);
                string uploadfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\room", filename);

                using (var stream = new FileStream(uploadfilepath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                room.Image = filename;
            }
            catch (Exception ex)
            {
                ViewBag.message = $"File upload failed: {ex.Message}";
                ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeID", "TypeName");
                ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
                return View(room); // Trả về view cùng dữ liệu đã nhập
            }
            // Thêm room vào cơ sở dữ liệu
            db.Rooms.Add(room);
            await db.SaveChangesAsync();
            var hoteladminID = db.Users
                   .Where(u => u.HotelID == HotelID)
                   .Select(u => u.UserId)
                   .FirstOrDefault();
            return RedirectToAction("RoomManagement", new { hoteladminID = hoteladminID });
        }
        [Route("Editroom")]
        [HttpGet]
        public async Task<IActionResult> Editroom(int roomID, int HotelID)
        {
            // Lấy danh sách RoomTypes
            var roomTypes = db.RoomTypes
                .Select(rt => new
                {
                    RoomTypeId = rt.RoomTypeId,
                    TypeName = rt.TypeName
                }).ToList();

            ViewBag.RoomTypes = new SelectList(roomTypes, "RoomTypeId", "TypeName");

            // Lấy danh sách Countries
            var countries = db.Countries
                .Select(ct => new
                {
                    CountryId = ct.CountryId,
                    CountryName = ct.CountryName
                }).ToList();
            ViewBag.Countries = new SelectList(countries, "CountryId", "CountryName");
            ViewBag.HotelID = HotelID;
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomID);
            return View(room);
        }

        [Route("Editroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editroom(Room room, IFormFile imageFile, int HotelID)
        {
            try
            {
                if (imageFile != null)
                {
                    string filename = Path.GetFileName(imageFile.FileName);
                    string uploadfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\room", filename);

                    using (var stream = new FileStream(uploadfilepath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    room.Image = filename;
                    ViewBag.message = "File uploaded successfully!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = $"File upload failed: {ex.Message}";
                ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeID", "TypeName");
                ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
                return View(room); 
            }

            var hoteladminID = db.Users
               .Where(u => u.HotelID == HotelID)
               .Select(u => u.UserId)
               .FirstOrDefault();
            if (ModelState.IsValid)
            {
                room.CreatedDate = DateTime.Now;
                db.Entry(room).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("RoomManagement", new { hoteladminID = hoteladminID });
                
            }

            return View(room);
        }

        [Route("Reservation")]
        public IActionResult ReservationManagement(int? page, int hoteladminID)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var roomsQuery = (from r in db.Rooms
                              join rs in db.Reservations on r.RoomId equals rs.RoomId
                              join h in db.Hotels on r.HotelID equals h.HotelId
                              join u in db.Users on h.HotelId equals u.HotelID
                              where u.UserId == hoteladminID
                              select rs).ToList();
            ViewBag.RoomID = new SelectList(db.Rooms.ToList(), "RoomID", "RoomName");
            ViewBag.User = new SelectList(db.Users.ToList(), "UserID", "Email");
            PagedList<Reservation> lst = new PagedList<Reservation>(roomsQuery.AsQueryable(), pageNumber, pageSize);
            return View(lst);
        }
        [Route("AddnewReservation")]
        [HttpGet]
        public IActionResult AddnewReservation()
        {

            var room = db.Rooms
                .Select(rt => new
                {
                    RoomId = rt.RoomId,
                    RoomName = rt.RoomName
                }).ToList();

            ViewBag.Rooms = new SelectList(room, "RoomId", "RoomName");
            var user = db.Users
                .Select(ct => new
                {
                    UserID = ct.UserId,
                    Email = ct.Email
                }).ToList();
            ViewBag.Users = new SelectList(user, "UserID", "Email");
            return View();
        }
        [Route("AddnewReservation")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddnewReservation(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                await db.SaveChangesAsync();
                return RedirectToAction("ReservationManagement");
            }


            ViewBag.RoomID = new SelectList(db.Rooms.ToList(), "RoomID", "RoomName");
            ViewBag.User = new SelectList(db.Users.ToList(), "UserID", "Email");
            return View(reservation);

        }
        [Route("EditReservation")]
        [HttpGet]
        public async Task<IActionResult> EditReservation(int ReservationID)
        {
            var reservation = db.Reservations.Find(ReservationID);
            var room = db.Rooms
               .Select(rt => new
               {
                   RoomId = rt.RoomId,
                   RoomName = rt.RoomName
               }).ToList();

            ViewBag.Rooms = new SelectList(room, "RoomId", "RoomName");

            var user = db.Users
                .Select(ct => new
                {
                    UserID = ct.UserId,
                    Email = ct.Email
                }).ToList();
            ViewBag.Users = new SelectList(user, "UserID", "Email");
            return View();
        }
        [Route("EditReservation")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReservation(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("ReservationManagement");
            }
            return View(reservation);
        }


        [Route("IsReserved")]
        public async Task<IActionResult> IsReserved(int ReservationId)
        {
            // Find the room by ID
            var reservation = await db.Reservations.FirstOrDefaultAsync(r => r.ReservationId == ReservationId);

            // Check if the room exists
            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction("ReservationManagement");
            }
//            select u.UserID from Rooms as r
//join Reservations as rs on r.RoomID = rs.RoomID
//join Hotels as h on h.HotelID = r.HotelID
//join Users as u on u.HotelID = h.HotelID
//where rs.ReservationID = 98
            // Update the room status
            reservation.Status = reservation.Status == "Reserved"? null: "Reserved";
            var hoteladminID = (from r in db.Rooms
                                join h in db.Hotels on r.HotelID equals h.HotelId
                                join u in db.Users on h.HotelId equals u.HotelID
                                join rs in db.Reservations on r.RoomId equals rs.RoomId
                                where rs.ReservationId == ReservationId
                                select u.UserId)
                    .FirstOrDefault();
            // Save changes to the database
            try
            {
                db.Entry(reservation).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reservation status updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Failed to update room status due to a concurrency issue.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("ReservationManagement", new { hoteladminID =HttpContext.Session.GetString("ID")});
        }
    }
}
