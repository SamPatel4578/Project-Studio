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

        public ActionResult Fuel()
        {
            var suppliers = new List<FuelSupplier>
            {
                new FuelSupplier { FuelSupplierId = 1, SupplierName = "BP", ImageUrl = "https://w7.pngwing.com/pngs/83/43/png-transparent-bp-logo-company-petroleum-technical-miscellaneous-company-leaf-thumbnail.png" },
                new FuelSupplier { FuelSupplierId = 2, SupplierName = "Shell", ImageUrl = "https://e7.pngegg.com/pngimages/616/199/png-clipart-royal-dutch-shell-shell-oil-company-petroleum-natural-gas-shell-orange-logo.png" },
                new FuelSupplier { FuelSupplierId = 3, SupplierName = "Caltex", ImageUrl = "https://w7.pngwing.com/pngs/608/336/png-transparent-caltex-hd-logo-thumbnail.png" },
                new FuelSupplier { FuelSupplierId = 4, SupplierName = "7-Eleven", ImageUrl = "https://w7.pngwing.com/pngs/466/674/png-transparent-logo-7-eleven-convenience-shop-franchising-7-miscellaneous-company-text-thumbnail.png" },
                new FuelSupplier { FuelSupplierId = 5, SupplierName = "Mobil", ImageUrl = "https://logolook.net/wp-content/uploads/2023/12/Mobil-Logo.png" },
                new FuelSupplier { FuelSupplierId = 6, SupplierName = "United", ImageUrl = "https://images.crunchbase.com/image/upload/c_pad,h_256,w_256,f_auto,q_auto:eco,dpr_1/v1492063146/lx58tlei4cswwlpqkkj3.png" },
                new FuelSupplier { FuelSupplierId = 7, SupplierName = "Ampol", ImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/4/4e/Ampol_Logo_May_2020.svg/2029px-Ampol_Logo_May_2020.svg.png" }
            };

            return View(suppliers);
        }

        //[HttpPost]
        //public IActionResult ProcessPayment(PaymentDetail paymentData, string SavePaymentDetails)
        //{
        //    if (string.IsNullOrEmpty(paymentData.CardholderName) || string.IsNullOrEmpty(paymentData.CardLastDigits))
        //    {
        //        return Content("Invalid cardholder name or card number.");
        //    }

        //    paymentData.PaymentStatus = "Success";

        //    if (SavePaymentDetails == "yes")
        //    {
        //        _context.PaymentDetails.Add(paymentData);
        //        _context.SaveChanges();
        //    }

        //    return Ok();
        //}

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

            // Redirect to the success page after payment is processed
            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
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

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Action to display the user profile
        public IActionResult Profile(int id)
        {
            var user = _context.Users
                               .Where(u => u.UserId == id)
                               .Select(u => new User
                               {
                                   UserId = u.UserId,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,
                                   PhoneNumber = u.PhoneNumber,
                                   Address = u.Address
                               })
                               .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Action to edit the user profile (GET)
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            var user = _context.Users
                               .Where(u => u.UserId == id)
                               .Select(u => new User
                               {
                                   UserId = u.UserId,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,
                                   PhoneNumber = u.PhoneNumber,
                                   Address = u.Address
                               })
                               .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Action to edit the user profile (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;

                _context.SaveChanges();

                return RedirectToAction("Profile", new { id = model.UserId });
            }

            return View(model);
        }

//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
