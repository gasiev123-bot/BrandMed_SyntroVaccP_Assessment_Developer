using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyntroVaccPApp.Infrastructure; // Access AppRoles constants

namespace SyntroVaccPApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken] // Security: Prevents CSRF attacks
        public async Task<IActionResult> Login(string email, string password)
        {
            // Production-Grade Strategy: Validate credentials against a source
            var userRole = AuthenticateUser(email, password);

            if (userRole != null)
            { 
                List<Claim> claims =
                    [
                    new(ClaimTypes.Name, email),
                    new(ClaimTypes.Role, userRole),
                    new("LastLogin", DateTime.UtcNow.ToString())
                    ];
                 
                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme); 

                AuthenticationProperties authProperties = new()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }


            ViewBag.Error = "Access Denied: Invalid medical credentials.";
            return View();
        }

        // --- Assessment/Mock Logic (Swap this for DB logic in Prod) ---
        private static string? AuthenticateUser(string email, string password)
        {
            // Mocking different roles for the BrandMed assessment
            if (password == "SyntroP2024!") // Common test password
            {
                return email.ToLower() switch
                {
                    "admin@syntrop.com" => AppRoles.Admin,
                    "auditor@syntrop.com" => AppRoles.Auditor,
                    "nurse@syntrop.com" => AppRoles.Clinician,
                    "clerk@syntrop.com" => AppRoles.Clerk,
                    _ => null
                };
            }
            return null;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View(); // Essential for Role management
    }
}
