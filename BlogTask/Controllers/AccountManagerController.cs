using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static BlogTask.Contracts.Models.Users.GetUserRequest;
using System.Security.Claims;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public AccountManagerController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        /// <summary>
        /// Обработка данный для аутентификации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(model.Login) ||
              String.IsNullOrEmpty(model.Password))
                    return StatusCode(400, "Запрос не корректен!");

                var user = _mapper.Map<User>(model);
                if (user is null)
                    return StatusCode(400, "Пользователь на найден!");

                if (!PasswordIsCorrect(user))
                {
                    return StatusCode(400, "Введенный пароль не корректен!");
                }

                var role = user.Role is null ? new Role() { Name = "Пользователь"} : user.Role;

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name)
                };

                ClaimsIdentity claimsIdentity = new(
                    claims,
                    "AddCookies",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Страница по редактированию информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = User;

            //var result = await _userManager.GetUserAsync(user);

            //var editmodel = _mapper.Map<UserEditViewModel>(result);

            return View("Edit");
        }

        /// <summary>
        /// Страница по редактированию информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewUser()
        {
            var user = User;

            //var result = await _userManager.GetUserAsync(user);

            //var editmodel = _mapper.Map<UserEditViewModel>(result);

            return View();
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>       
        /// <returns></returns>
        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var user = User;


            //if (result == null) return View("UserListNoButton", model);
            return View("UserList");
        }

        /// <summary>
        /// Метод, выхода из аккаунта
        /// </summary>
        [Route("Logout")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        private bool PasswordIsCorrect(User user)
        {
            var repository = _unitOfWork.GetRepository<User>() as UsersRepository;

            var findUser = repository.GetByLogin(user.Login);

            if (findUser is null) { return false; }

            if (user.Password == null) { return false; }

            if (findUser.Password != user.Password) { return false; }

            return true;
        }

    }
}
