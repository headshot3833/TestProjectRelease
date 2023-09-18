using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MvcApp.Models;
using Tests.Models;
using Tests.Services;

namespace Tests.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly Aplicationdb _context;
        public AuthController(AuthService authService, Aplicationdb context)
        {
            _authService = authService;
            _context = context;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            if (claimuser.Identity.IsAuthenticated)
                return RedirectToAction("index", "home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                // Здесь предполагается, что у вас есть сервис или репозиторий для работы с базой данных,
                // который позволяет вам проверить данные пользователя.
                var user = await _authService.GetUserByLoginAsync(loginModel.Login);

                if (user != null && _authService.VerifyPassword(user, loginModel.Password))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                // Добавьте другие утверждения, если это необходимо.
            };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginModel.keeploggedin
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("CreateSubject", "TestCreate");
                }
            }

            ViewData["ValidateMessage"] = "Неправильное имя пользователя или пароль";
            return View();
        }
        //[Authorize]
        //[HttpGet]
        //public IActionResult ManageUsers()
        //{
        //    var Users = _context.Users.ToList();

        //    return View(Users);

        //}
        [Authorize]
        [HttpGet]
        public IActionResult CreateUserAdmin()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateUserAdmin(CreateUserAdminViewModel model)
        {

            if (ModelState.IsValid)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(model.Password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    // Создайте нового пользователя на основе данных из модели представления
                    var newUser = new User
                    {
                        Login = model.Login,
                        Password = sb.ToString(),
                        Roles = new List<UserRole> // Создайте список ролей
                        {
                            new UserRole { Role = model.Role } // Добавьте роль из модели представления
                        } // Установите роль пользователя из модели представления
                    };

                    // Добавьте нового пользователя в контекст базы данных
                    _context.Users.Add(newUser);
                    _context.SaveChanges(); // Сохраните изменения в базе данных

                    // Верните редирект на страницу, которая отображает список пользователей
                    return RedirectToAction("ManageUsers");
                }
            }
            
            // Если модель недействительна, верните представление с ошибками валидации
            return View(model);
        }
    }
}