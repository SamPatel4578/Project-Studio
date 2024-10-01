using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;
using UTR_WebApplication.Data;
using UTR_WebApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


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

        public IActionResult StaffLogin()
        {
            return View();
        }

        public IActionResult OrderHistory()
        {
            var orders = from orderItem in _context.OrderItems
                         join foodOrder in _context.FoodOrders
                         on orderItem.FoodOrderId equals foodOrder.FoodOrderId
                         select new
                         {
                             FoodOrderId = orderItem.FoodOrderId,
                             ItemName = orderItem.ItemName,
                             Quantity = orderItem.Quantity,
                             Price = orderItem.Price,
                             ImageUrl = orderItem.ImageUrl
                         };

            ViewBag.Orders = orders.ToList();

            return View();
        }




        [Authorize]
        public IActionResult Payment()
        {
            return View();  
        }

        [Authorize]
        public IActionResult PaymentDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var paymentDetail = _context.PaymentDetails.FirstOrDefault(pd => pd.UserId == int.Parse(userId)) ?? new PaymentDetail();

            return View(paymentDetail);

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePaymentDetail(int paymentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var paymentDetail = _context.PaymentDetails.FirstOrDefault(pd => pd.PaymentId == paymentId && pd.UserId == int.Parse(userId));

            if (paymentDetail == null)
            {
                return NotFound("No payment details found for this user.");
            }

            _context.PaymentDetails.Remove(paymentDetail);
            _context.SaveChanges();

            return RedirectToAction("PaymentDetailDeleted");
        }

        [Authorize]
        public IActionResult PaymentDetailDeleted()
        {
            return View();
        }



        [Authorize]
        public IActionResult CashierDashboard()
        {
            // Fetch all food orders including related order items
            var orders = _context.FoodOrders
                                 .Include(o => o.OrderItems)
                                 .Include(o => o.User)
                                 .Where(o => o.Status == "Pending" || o.Status == "Accepted" || o.Status == "Declined")
                                 .ToList();

            return View(orders);
        }


        [Authorize]
        public IActionResult Cart()
        {
            return View();
        }

        [Authorize]
        public ActionResult Fuel()
        {
            var suppliers = new List<FuelSupplier>
            {
                new FuelSupplier { FuelSupplierId = 1, SupplierName = "BP", ImageUrl = "https://download.logo.wine/logo/BP/BP-Logo.wine.png" },
                new FuelSupplier { FuelSupplierId = 2, SupplierName = "Shell", ImageUrl = "https://logos-world.net/wp-content/uploads/2020/11/Shell-Logo.png" },
                new FuelSupplier { FuelSupplierId = 3, SupplierName = "Caltex", ImageUrl = "https://download.logo.wine/logo/Caltex/Caltex-Logo.wine.png" },
                new FuelSupplier { FuelSupplierId = 4, SupplierName = "7-Eleven", ImageUrl = "https://logos-world.net/wp-content/uploads/2021/08/7-Eleven-Logo.png" },
                new FuelSupplier { FuelSupplierId = 5, SupplierName = "Mobil", ImageUrl = "https://logolook.net/wp-content/uploads/2023/12/Mobil-Logo.png" },
                new FuelSupplier { FuelSupplierId = 6, SupplierName = "United", ImageUrl = "https://images.crunchbase.com/image/upload/c_pad,h_256,w_256,f_auto,q_auto:eco,dpr_1/v1492063146/lx58tlei4cswwlpqkkj3.png" },
                new FuelSupplier { FuelSupplierId = 7, SupplierName = "Ampol", ImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/4/4e/Ampol_Logo_May_2020.svg/2029px-Ampol_Logo_May_2020.svg.png" }
            };

            return View(suppliers);
        }


        [HttpPost]
        [Authorize]
        public IActionResult ProcessPayment(PaymentDetail paymentData, string SavePaymentDetails)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            paymentData.UserId = int.Parse(userId);

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

            return RedirectToAction("PaymentSuccess");
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewInvoice()
        {
            int orderId = 3; // Hardcoded orderId for this case

            var foodOrder = _context.FoodOrders
                .Where(o => o.FoodOrderId == orderId)
                .FirstOrDefault();

            if (foodOrder != null)
            {
                var orderItems = _context.OrderItems
                    .Where(oi => oi.FoodOrderId == orderId)
                    .ToList();

                foodOrder.OrderItems = orderItems;

                return View(foodOrder);
            }

            return NotFound(); // 404 if the order is not found
        }

        public IActionResult DownloadInvoice(int orderId)
        {
            var foodOrder = _context.FoodOrders
                .Where(o => o.FoodOrderId == orderId)
                .FirstOrDefault();

            if (foodOrder == null)
            {
                return NotFound(); // If order not found, return 404
            }

            // Fetch the associated order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.FoodOrderId == orderId)
                .ToList();

            foodOrder.OrderItems = orderItems;

            // Create a PDF document
            MemoryStream workStream = new MemoryStream();
            Document doc = new Document();
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;

            doc.Open();

            // Add content to the PDF
            doc.Add(new Paragraph("Order Invoice"));
            doc.Add(new Paragraph("Order ID: " + foodOrder.FoodOrderId));
            doc.Add(new Paragraph("Total Price: $" + foodOrder.TotalPrice));
            doc.Add(new Paragraph("Items:"));

            foreach (var item in foodOrder.OrderItems)
            {
                doc.Add(new Paragraph($"{item.ItemName} - Quantity: {item.Quantity} - Price: ${item.Price}"));
            }

            doc.Close();

            // Return the PDF as a download
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", $"Invoice_{foodOrder.FoodOrderId}.pdf");
        }

        private int GetUserId()
        {
            return 1; 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDetail model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var loginDetail = _context.LoginDetails.FirstOrDefault(u => u.Email == model.Email);

                if (loginDetail != null)
                {
                    if (loginDetail.PasswordHash == model.PasswordHash)
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, loginDetail.Email),
                                new Claim(ClaimTypes.NameIdentifier, loginDetail.UserId.ToString()) 
                            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, 
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30) 
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
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

        [Authorize]
        public IActionResult FoodMenu()
        { 
            var menuItems = _context.MenuItems.ToList();
            return View(menuItems);
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [Authorize]
        public IActionResult Profile()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home"); 
            }

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            
            var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var loginDetail = _context.LoginDetails.FirstOrDefault(ld => ld.UserId == user.UserId);
            if (loginDetail == null)
            {
                return NotFound();
            }

            var model = Tuple.Create(user, loginDetail);

            return View(model);
        }




        [HttpPost]
        public IActionResult UpdateProfile(User userModel, string NewPassword)
        {
            var user = _context.Users.Find(userModel.UserId);
            if (user != null)
            {
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;
                user.PhoneNumber = userModel.PhoneNumber;
                user.Address = userModel.Address;

                if (!string.IsNullOrEmpty(NewPassword))
                {
                    var loginDetails = _context.LoginDetails.FirstOrDefault(ld => ld.UserId == user.UserId);
                    if (loginDetails != null)
                    {
                        loginDetails.PasswordHash = NewPassword; 
                    }
                }

                var loginDetail = _context.LoginDetails.FirstOrDefault(ld => ld.UserId == user.UserId);
                if (loginDetail != null)
                {
                    loginDetail.Email = userModel.Email; 
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Profile", "Home"); 
        }



        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
