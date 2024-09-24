
using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Threading.Tasks;
using HotelBooking.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
namespace HotelBooking.Areas.Admin.Controllers
{
    
    [Authentication]
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
        public IActionResult RoomManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstRoom = db.Rooms.ToList();
            PagedList<Room> lst = new PagedList<Room>(lstRoom, pageNumber, pageSize);
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
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
        [Route("Editroom")]
        [HttpGet]
        public async Task<IActionResult> Editroom(int roomID)
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
            var room = db.Rooms.Find(roomID);
            return View();
        }
        [Route("Editroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editroom(Room room, IFormFile imageFile)
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

            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("RoomManagement");
            }
            return View(room);
        }
        [Route("Deleteroom")]
        [HttpGet]
        public async Task<IActionResult> Deleteroom(int id)
        {
            db.Remove(db.Rooms.Find(id));
            db.SaveChanges();
            return RedirectToAction("Room");
        }










        [Route("Reservation")]
        public IActionResult ReservationManagement(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstReservation = db.Reservations.ToList();

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
            return View();
        }
        [Route("AddnewUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddnewUser(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("UserManagement");
            }
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