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
using BlogTask.Models;

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

        /// <summary>
        /// Вывод формы для регистрации
        /// </summary>
        /// <returns></returns>
        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var repository = _unitOfWork.GetRepository<User>() as UsersRepository;

                var findUser = repository.GetByLogin(user.Login);
                if (findUser is not null) { return View("Register"); }

                await repository.CreateAsync(user);
            }
            return View("Login");
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

                var userRepository = _unitOfWork.GetRepository<User>() as UsersRepository;
                var roleId = userRepository.GetByLogin(user.Login).RoleId;

                var roleRepository = _unitOfWork.GetRepository<Role>() as RolesRepository;
                var roleName = roleId is null ? "Пользователь" : roleRepository.GetAsync((int)roleId).Result.Name;

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)
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
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            var repository = _unitOfWork.GetRepository<User>() as UsersRepository;

            var listUsers = repository.GetAll().ToList();

            if (listUsers == null)
            {
                return View("Event", new EventViewModel() { Send = "Пользователи отсутствуют!" });
            }
            if (listUsers.Count() == 0)
            {
                return View("Event", new EventViewModel() { Send = "Пользователи отсутствуют!" });
            }

            ListViewModel view = new ListViewModel();
            view.UserList = _mapper.Map<List<User>, List<UserViewModel>>(listUsers);

            return View(view);
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
