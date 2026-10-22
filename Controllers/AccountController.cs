using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UbthIctTicketing.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            // General ICT staff user option
            if (username.Equals("support", StringComparison.OrdinalIgnoreCase) && password == "support")
            {
                HttpContext.Session.SetString("UserRole", "support");
                return RedirectToAction("Index", "Incidents");
            }
            // Admin user option
            else if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "admin")
            {
                HttpContext.Session.SetString("UserRole", "admin");
                return RedirectToAction("Index", "Incidents");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}