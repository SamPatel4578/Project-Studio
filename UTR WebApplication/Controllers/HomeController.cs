using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDetail model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var loginDetail = _context.LoginDetails.FirstOrDefault(u => u.Email == model.Email);

                if (loginDetail != null)
                {
                    // Compare the entered password with the stored password
                    if (loginDetail.PasswordHash == model.PasswordHash)
                    {
                        // Handle successful login
                        // You can add logic for setting up session, cookies, etc.
                        return RedirectToAction("FoodMenu", "Home");
                    }
                    else
                    {
                        // If the password is incorrect
                        ModelState.AddModelError("", "Invalid password.");
                    }
                }
                else
                {
                    // If no user is found with the entered email
                    ModelState.AddModelError("", "Invalid email.");
                }
            }

            // If we get to this point, something failed, redisplay the form
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
                // Check if the email already exists in the LoginDetail table
                var existingLoginDetail = _context.LoginDetails.FirstOrDefault(ld => ld.Email == user.Email);
                if (existingLoginDetail != null)
                {
                    // Email already exists, show an error
                    ModelState.AddModelError("", "Email is already in use.");
                    ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
                    return View("SignUp", user);
                }

                try
                {
                    // Add the user to the context and save changes to the database
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    // Add debugging info to confirm the process
                    System.Diagnostics.Debug.WriteLine("User saved successfully with UserId: " + user.UserId);

                    // After saving the user, create a new LoginDetail record
                    var loginDetail = new LoginDetail
                    {
                        UserId = user.UserId,  // Associate with the newly created User
                        Email = user.Email,    // Store email
                        PasswordHash = password,  // Store the password directly (no hashing)
                        TwoFactorEnabled = false,  // Default to not using two-factor authentication
                        TermsAccepted = true  // Assuming the user accepted the terms during signup
                    };

                    // Add debugging info for login details
                    System.Diagnostics.Debug.WriteLine("LoginDetail prepared for UserId: " + loginDetail.UserId + " with Email: " + loginDetail.Email);

                    // Add login details to the context and save changes
                    _context.LoginDetails.Add(loginDetail);
                    _context.SaveChanges();

                    // Debug info after saving login details
                    System.Diagnostics.Debug.WriteLine("LoginDetail saved successfully for UserId: " + loginDetail.UserId);
                }
                catch (DbUpdateException ex)
                {
                    // Handle any errors related to saving data to the database
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "There was a problem saving data. Please try again later.");
                    ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
                    return View("SignUp", user);
                }

                // Redirect to the login page after successful signup
                return RedirectToAction("Login");
            }

            // If ModelState is not valid, reload the SignUp view with the input data
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

        public ActionResult FoodMenu()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
