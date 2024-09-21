
using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Threading.Tasks;
namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("Admin")]
    [Route("Admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        HotelDbContext db = new HotelDbContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
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
            
                
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = imageFile.FileName;
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "~wwwroot/img/room/", uniqueFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        imageFile.CopyToAsync(stream);
                    }

                    room.Image = "/img/room/" + uniqueFileName;
                }
                var existingRoom = await db.Rooms
                .FirstOrDefaultAsync(r => r.RoomName == room.RoomName);
                if (existingRoom != null)
                {
                    ModelState.AddModelError("RoomName", "This room name already exists.");
                    return View(room);
                }
                ViewBag.RoomTypeId = new SelectList(db.RoomTypes.ToList(), "RoomTypeID", "TypeName");
                ViewBag.CountryID = new SelectList(db.Countries.ToList(), "CountryID", "CountryName");
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("RoomManagement");
            
        }
        [Route("Editroom")]
        [HttpGet]
        public async Task<IActionResult> Editroom(int roomID)
        {
            var room = db.Rooms.Find(roomID);
            return View();
        }
        [Route("Editroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editroom(Room room)
        {
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















    }
}