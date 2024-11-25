using Azure;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using HotelBooking.Models.Authentication;
using HotelBooking.Models_View;
namespace HotelBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<HomeController> _logger;
        private readonly HotelDbContext _db;
        public HomeController(IVnPayService vnPayService, ILogger<HomeController> logger, HotelDbContext db)
        {
            _vnPayService = vnPayService;
            _logger = logger;
            _db = db;
        }
        public IActionResult PaymentA()
        {
            return View();
        }
        public async Task<IActionResult> Cash(string roomID, DateTime checkin, DateTime checkout, string userName, string userEmail, decimal price, string nameRoom, string description)
        {
            ViewBag.userName = userName;
            ViewBag.userEmail = userEmail;
            var totalDays = (checkout - checkin).Days;
            price = price * totalDays;
            ViewBag.price = price;
            ViewBag.nameRoom = nameRoom;
            ViewBag.description = description;
            Console.WriteLine(checkin.ToString());

            var roomid = _db.Rooms.SingleOrDefault(r => r.RoomName == roomID).RoomId;
            int userid = _db.Users.SingleOrDefault(u => u.Email == userEmail).UserId;

            Reservation reservation = new Reservation
            {
                UserId = userid,
                RoomId = roomid,
                CheckInDate = checkin,
                CheckOutDate = checkout,
                NumberOfGuests = 2,
                Status = null,
                BookingDate = DateTime.Now
            };
            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();

            ViewBag.ReservationID = reservation.ReservationId;
            ViewBag.Status = "Sucess";

            var room = _db.Rooms.SingleOrDefault(r => r.RoomName == roomID);
            return View(room);
        }
        public async Task<IActionResult> Payment(string roomID, DateTime checkin, DateTime checkout,string userName, string userEmail, decimal price, string nameRoom, string description)
        {
            ViewBag.userName = userName;
            ViewBag.userEmail = userEmail;
            var totalDays = (checkout - checkin).Days;
            price = price * totalDays;
            ViewBag.price = price;
            ViewBag.nameRoom = nameRoom;
            ViewBag.description = description;

            var roomid = _db.Rooms.SingleOrDefault(r => r.RoomName == roomID).RoomId;
            int userid = _db.Users.SingleOrDefault(u => u.Email == userEmail).UserId;

            Reservation reservation= new Reservation
            {
                UserId= userid,
                RoomId= roomid,
                CheckInDate= checkin,
                CheckOutDate= checkout,
                NumberOfGuests= 2,
                Status= null,
                BookingDate= DateTime.Now
            };
            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();

            ViewBag.ReservationID = reservation.ReservationId;
            var room = _db.Rooms.SingleOrDefault(r => r.RoomName == roomID);
            return View(room);
        }

        public async Task<IActionResult> CreatePaymentUrl(PaymentInformationModel model, int ReservationID, decimal Amount)
        {
            Payment payment = new Payment
            {
                ReservationId = ReservationID,
                Amount = Amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = null
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            model.Name = ReservationID.ToString();
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Lấy payment và cập nhật phương thức thanh toán
            var payment = await _db.Payments.SingleOrDefaultAsync(p => p.ReservationId == int.Parse(response.OrderDescription));
            if (payment != null)
            {
                payment.PaymentMethod = response.PaymentMethod;
                await _db.SaveChangesAsync();
            }

            // Lấy reservation
            var reservation = await _db.Reservations.FindAsync(int.Parse(response.OrderDescription));
            if (reservation != null)
            {
                // Kiểm tra mã phản hồi và cập nhật trạng thái đặt phòng
                if (response.VnPayResponseCode == "00")
                {
                    reservation.Status = "Reserved";
                    await _db.SaveChangesAsync();
                    ViewBag.Message = "Success";
                    return View(); // Thay "PaymentSuccess" bằng tên view của bạn
                }
                else
                {
                    ViewBag.Message = "Cancelled";
                    reservation.Status = "Cancelled";
                    await _db.SaveChangesAsync();
                    return View();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            var lstroom = (from r in _db.Rooms
                              where r.Status == "Available"
                              select r).ToList();

            return View(lstroom);
        }

        public IActionResult RoomDetail(string roomID)
        {
            ViewBag.RoomTypeId = new SelectList(_db.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            ViewBag.CountryID = new SelectList(_db.Countries.ToList(), "CountryID", "CountryName");
            ViewBag.HotelID = new SelectList(_db.Hotels.ToList(), "HotelID", "HotelName");
            var room = _db.Rooms.SingleOrDefault(x => x.RoomName == roomID);
            
            return View(room);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
