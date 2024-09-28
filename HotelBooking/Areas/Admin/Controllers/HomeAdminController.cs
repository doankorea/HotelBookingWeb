
using HotelBooking.Models;
using HotelBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelBooking.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("Admin")]
    [Route("Admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        HotelDbContext db = new HotelDbContext();
        
        [Route("roomtype")]
        public IActionResult RoomTypeManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstRoomType = db.RoomTypes.ToList();

            PagedList<RoomType> lst = new PagedList<RoomType>(lstRoomType, pageNumber, pageSize);
            return View(lst);
        }
        [Route("Addnewroomtype")]
        [HttpGet]
        public IActionResult Addnewroomtype()
        {
            return View();
        }
        [Route("Addnewroomtype")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addnewroomtype(RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                db.RoomTypes.Add(roomType);
                await db.SaveChangesAsync();
                return RedirectToAction("Addnewroomtype");
            }
            return View(roomType);

        }
        [Route("Editroomtype")]
        [HttpGet]
        public async Task<IActionResult> Editroomtype(int roomTypeID)
        {
            var roomType = db.RoomTypes.Find(roomTypeID);
            return View();
        }
        [Route("Editroomtype")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editroomtype(RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("RoomTypeManagement");
            }
            return View(roomType);
        }
        [Route("Deleteroomtype")]
        [HttpGet]
        public async Task<IActionResult> Deleteroomtype(int id)
        {
            db.Remove(db.RoomTypes.Find(id));
            db.SaveChanges();
            return RedirectToAction("RoomType");
        }

        
        [Route("")]
        [Route("index")]
        [Route("room")]
        public IActionResult RoomManagement(int? page, string searchString)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstRoom = db.Rooms.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                lstRoom = lstRoom
                    .Where(n => n.RoomName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            
            if (!lstRoom.Any())
            {
                ViewBag.Message = "No rooms found matching your search criteria."; // Thông báo không tìm thấy phòng
            }
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.HotelID = new SelectList(db.Hotels.ToList(), "HotelID", "HotelName");
            
            PagedList<Room> lst = new PagedList<Room>(lstRoom, pageNumber, pageSize);
            return View(lst);
        }

        [Route("Addnewroom")]
        [HttpGet]
        public IActionResult Addnewroom()
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
            return View();

        }

        [Route("Addnewroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addnewroom(Room room, IFormFile imageFile)
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
                    ViewBag.message = "File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.message = $"File upload failed: {ex.Message}";
                }
           

            // Kiểm tra xem room có tồn tại hay không
            var existingRoom = await db.Rooms.FirstOrDefaultAsync(r => r.RoomName == room.RoomName);
            if (existingRoom != null)
            {
                ModelState.AddModelError("RoomName", "This room name already exists.");
                return View(room);
            }

            // Đổ dữ liệu vào dropdown
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeID", "TypeName");
            ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");

            // Lưu room vào cơ sở dữ liệu
            db.Rooms.Add(room);
            await db.SaveChangesAsync();

            return RedirectToAction("RoomManagement");
        }
        [Route("IsActive")]
        public async Task<IActionResult> IsActive(int RoomID)
        {
            // Find the room by ID
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.RoomId == RoomID);

            // Check if the room exists
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction("RoomManagement");
            }

            // Update the room status
            room.Status = room.Status == "Available" ? "Occupied" : "Available"; 

            // Save changes to the database
            try
            {
                db.Entry(room).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room status updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Failed to update room status due to a concurrency issue.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("RoomManagement"); // Redirect to the Room Management page
        }
        [Route("IsHotelActive")]
        public async Task<IActionResult> IsHotelActive(int HotelId)
        {
            // Find the room by ID
            var hotel = await db.Hotels.FirstOrDefaultAsync(r => r.HotelId == HotelId);

            // Check if the room exists
            if (hotel == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction("RoomManagement");
            }

            // Update the room status
            hotel.IsActive = hotel.IsActive== false? true: false;

            // Save changes to the database
            try
            {
                db.Entry(hotel).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "hotel status updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Failed to update hotel status due to a concurrency issue.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("HotelManagement"); // Redirect to the Room Management page
        }
        [Route("IsUserActive")]
        public async Task<IActionResult> IsUserActive(int UserID)
        {
            // Find the room by ID
            var user = await db.Users.FirstOrDefaultAsync(r => r.UserId == UserID);

            // Check if the room exists
            if (user == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction("RoomManagement");
            }

            // Update the room status
            user.IsActive = user.IsActive== false? true: false;

            // Save changes to the database
            try
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room status updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Failed to update room status due to a concurrency issue.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("UserManagement"); // Redirect to the Room Management page
        }

        [Route("Deleteroom")]
        [HttpGet]
        public async Task<IActionResult> Deleteroom(int id)
        {
            db.Remove(db.Rooms.Find(id));
            db.SaveChanges();
            return RedirectToAction("Room");
        }

        [Route("ProductbyHotel")]
        public IActionResult ProductbyHotel(int HotelID, int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstroom = db.Rooms.AsNoTracking()
             .Include(r => r.Hotel)      // Load thông tin Hotel
             .Include(r => r.RoomType)   // Load thông tin RoomType
             .Where(x => x.HotelID == HotelID)
             .OrderBy(x => x.RoomName);

            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.HotelID = new SelectList(db.Hotels.ToList(), "HotelID", "HotelName");
            ViewBag.HotelID2 = HotelID;
            PagedList<Room> lst = new PagedList<Room>(lstroom, pageNumber, pageSize);
            return View(lst);
        }




        [Route("Reservation")]
        public IActionResult ReservationManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            decimal Revenues = db.Reservations
            .Where(rs => rs.Status == "Reserved")
            .Join(db.Rooms,
            rs => rs.RoomId,
            r => r.RoomId,
            (rs, r) => r.Price)
            .Sum() ?? 0;
            decimal Revenuess = Revenues * 0.05m;
            var lstReservation = db.Reservations.ToList();
            ViewBag.Revenues = Revenuess;
            ViewBag.RoomID = new SelectList(db.Rooms.ToList(), "RoomID", "RoomName");
            ViewBag.User = new SelectList(db.Users.ToList(), "UserID", "Email");
            PagedList<Reservation> lst = new PagedList<Reservation>(lstReservation, pageNumber, pageSize);
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


        [Route("DeleteReservation")]
        [HttpGet]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            db.Remove(db.Reservations.Find(id));
            db.SaveChanges();
            return RedirectToAction("ReservationManagement");
        }







        [Route("User")]
        public IActionResult UserManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstUser = db.Users.ToList();
            
            PagedList<User> lst = new PagedList<User>(lstUser, pageNumber, pageSize);
            return View(lst);
        }
        [Route("AddnewUser")]
        [HttpGet]
        public IActionResult AddnewUser()
        {
            var hotel = db.Hotels
               .Select(rt => new
               {
                   HotelID = rt.HotelId,
                   HotelName = rt.HotelName
               }).ToList();

            ViewBag.Hotels = new SelectList(hotel, "HotelID", "HotelName");
            return View();
        }
        [Route("AddnewUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddnewUser(User user)
        {
            
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("UserManagement");
            
            return View(user);

        }


        [Route("EditUser")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            return View(user); 
        }

        [Route("EditUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User user)
        {
                if (ModelState.IsValid)
                {
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("UserManagement");
                }
            return View(user);
        
    }



        [Route("DeleteUser")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            db.Remove(db.Users.Find(UserId));
            db.SaveChanges();
            return RedirectToAction("UserManagement");
        }


        [Route("Hotel")]
        public IActionResult HotelManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstHotel = db.Hotels.ToList();

            PagedList<Hotel> lst = new PagedList<Hotel>(lstHotel, pageNumber, pageSize);
            return View(lst);
        }

        [Route("AddnewHotel")]
        [HttpGet]
        public IActionResult AddnewHotel()
        {
            return View();
        }
        [Route("AddnewHotel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddnewHotel(Hotel hotel)
        {

            db.Hotels.Add(hotel);
            await db.SaveChangesAsync();
            return RedirectToAction("HotelManagement");

            return View(hotel);

        }







        [Route("Payment")]
        public IActionResult PaymentManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstPayment = db.Payments.ToList();

            PagedList<Payment> lst = new PagedList<Payment>(lstPayment, pageNumber, pageSize);
            return View(lst);
        }

        [Route("DeletePayment")]
        [HttpGet]
        public async Task<IActionResult> DeletePayment(int id)
        {
            db.Remove(db.Payments.Find(id));
            db.SaveChanges();
            return RedirectToAction("PaymentManagement");
        }
    }
}