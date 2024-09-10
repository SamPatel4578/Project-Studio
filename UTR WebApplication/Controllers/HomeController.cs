using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using UTR_WebApplication.Data;
using UTR_WebApplication.Models;

namespace UTR_WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UtrContext _context;

        public HomeController(ILogger<HomeController> logger, UtrContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Payment()
        {
            return View();  
        }


        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentDetail paymentData, string SavePaymentDetails)
        {
            if (string.IsNullOrEmpty(paymentData.CardholderName) || string.IsNullOrEmpty(paymentData.CardLastDigits))
            {
                return Content("Invalid cardholder name or card number.");
            }

            paymentData.PaymentStatus = "Success";

            if (SavePaymentDetails == "yes")
            {
                _context.PaymentDetails.Add(paymentData);
                _context.SaveChanges();
            }

            return Ok();
        }


        private int GetUserId()
        {
            return 1; 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDetail model)
        {
            if (ModelState.IsValid)
            {
                var loginDetail = _context.LoginDetails.FirstOrDefault(u => u.Email == model.Email);

                if (loginDetail != null)
                {
                    if (loginDetail.PasswordHash == model.PasswordHash)
                    {
                        
                        return RedirectToAction("FoodMenu", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email.");
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("FirstName,LastName,Email,PhoneNumber,Address,RoleId")] User user, string password)
        {
            if (ModelState.IsValid)
            {
                var existingLoginDetail = _context.LoginDetails.FirstOrDefault(ld => ld.Email == user.Email);
                if (existingLoginDetail != null)
                {
                    ModelState.AddModelError("", "Email is already in use.");
                    ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
                    return View("SignUp", user);
                }

                try
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    System.Diagnostics.Debug.WriteLine("User saved successfully with UserId: " + user.UserId);

                    var loginDetail = new LoginDetail
                    {
                        UserId = user.UserId,  
                        Email = user.Email,    
                        PasswordHash = password,  
                        TwoFactorEnabled = false,  
                        TermsAccepted = true  
                    };

                    System.Diagnostics.Debug.WriteLine("LoginDetail prepared for UserId: " + loginDetail.UserId + " with Email: " + loginDetail.Email);

                    _context.LoginDetails.Add(loginDetail);
                    _context.SaveChanges();

                    System.Diagnostics.Debug.WriteLine("LoginDetail saved successfully for UserId: " + loginDetail.UserId);
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "There was a problem saving data. Please try again later.");
                    ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
                    return View("SignUp", user);
                }

                return RedirectToAction("Login");
            }

            ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
            return View("SignUp", user);
        }




        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,PhoneNumber,Address,RoleId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public ActionResult TwofactorAuth()
        {
            return View();
        }

        public IActionResult FoodMenu()
        {
            var menuItems = _context.MenuItems.ToList();
            return View(menuItems);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
