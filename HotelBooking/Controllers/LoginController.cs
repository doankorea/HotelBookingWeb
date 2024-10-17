using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    
        [Route("Login")]
        [Route("Login/Login")]
    public class LoginController : Controller
    {
        private readonly HotelDbContext _db;

            public LoginController(HotelDbContext db)
            {
                _db = db;
            }
            [Route("Login")]
            [HttpGet]
            public IActionResult Login()
            {
                // If the user is already logged in, redirect to the appropriate page
                if (HttpContext.Session.GetString("Email") != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            [Route("Login")]
            [HttpPost]
            public IActionResult Login(User user)
            {
                // Clear the error initially
                TempData["error"] = "";

                // Check if the user is already logged in
                if (HttpContext.Session.GetString("Email") == null)
                {
                    // Find user in the database with the matching email and password
                    var u = _db.Users.FirstOrDefault(x => x.Email == user.Email && x.PasswordHash == user.PasswordHash);

                    if (u != null)
                    {
                        // Set session variables
                        HttpContext.Session.SetString("Email", u.Email);
                        HttpContext.Session.SetString("ID", u.UserId.ToString());
                        HttpContext.Session.SetString("Ten", u.Email);

                            // Redirect to the home page
                            return RedirectToAction("Index", "Home");
                        
                    }   
                    else
                    {
                        // Invalid login attempt
                        TempData["error"] = "Tài khoản hoặc mật khẩu không đúng";
                    }
                }
                // If login fails, stay on the login page
                return View();
            }

            [Route("SignUp")]
            public IActionResult Signup()
            {
                return View();
            }
            [Route("SignUp")]
            [HttpPost]
            public IActionResult Signup(User newUser)
            {
                if (ModelState.IsValid)
                {
                    // Check if the email already exists
                    var existingUser = _db.Users.FirstOrDefault(u => u.Email == newUser.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "This email is already registered.");
                        return View(newUser);
                    }

                    // Set default values
                    newUser.Role = "Customer"; // Default role

                    // Add the new user to the database
                    _db.Users.Add(newUser);
                    _db.SaveChanges();

                    // Redirect to the login page
                    return RedirectToAction("Login", "Login");
                }

                return View(newUser);
            }
            [Route("Logout")]
            public IActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
        }
    }

