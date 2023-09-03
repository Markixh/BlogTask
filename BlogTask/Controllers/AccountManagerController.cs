using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogTask.Models;
using BlogTask.BLL.Services;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class AccountManagerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UsersRepository _usersRepository;
        private readonly RolesRepository _rolesRepository;
        private readonly IService<User> _userService;
        private readonly ILogger<AccountManagerController> _logger;

        public AccountManagerController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AccountManagerController> logger, IService<User> service)
        {
            _mapper = mapper;
            _usersRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _rolesRepository = unitOfWork.GetRepository<Role>() as RolesRepository;
            _userService = service;
            _logger = logger;
            _logger.LogInformation("Создан AccountManagerController");
        }

        /// <summary>
        /// Вывод формы для регистрации
        /// </summary>
        /// <returns></returns>
        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Перешли на страницу Register");
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

                var findUser = _usersRepository?.GetByLogin(user.Login);
                if (findUser is not null) 
                {
                    _logger.LogInformation("Модель передана пустой");
                    return View("Register"); 
                }

                if (_usersRepository is not null) { await _usersRepository.CreateAsync(user); }
                _logger.LogInformation("Пользователь успешно зарегистрировался");
            }
            else 
            {
                _logger.LogWarning("Ошибка Верификации при регистрации");
            }
            return View("Register", model);
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Перешли на страницу Login");
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
                if (String.IsNullOrEmpty(model.Login) || String.IsNullOrEmpty(model.Password))
                {
                    _logger.LogWarning("Логин или пароль не заданы");
                    return StatusCode(400, "Запрос не корректен!");
                }

                var user = _mapper.Map<User>(model);
                if (user is null)
                {
                    _logger.LogWarning("Модель передана пустой");
                    return StatusCode(400, "Пользователь на найден!");
                }

                if (!PasswordIsCorrect(user))
                {
                    _logger.LogWarning("Пароль введен не правильный");
                    return StatusCode(400, "Введенный пароль не корректен!");
                }
                
                var roleId = _usersRepository?.GetByLogin(user.Login).RoleId;

                var roleName = roleId is null ? "Пользователь" : _rolesRepository?.GetAsync((int)roleId).Result.Name;
                roleName = roleName is null ? "Пользователь" : roleName;

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
                _logger.LogInformation("Пользователь успешно авторизовался");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning("Ошибка Верификации при логировании");
                return View("Login", model);
            }
        }

        /// <summary>
        /// Страница по редактированию информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var user = await _usersRepository.GetAsync(guid);

            var editUser = _mapper.Map<User, EditUserVeiwModel>(user);

            _logger.LogInformation("Перешли на страницу Редактирования пользователя");

            return View("Edit", editUser);
        }

        /// <summary>
        /// Обработка результата редактирования пользователя
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserVeiwModel model)
        {
            var editUser = await _usersRepository.GetAsync(model.Guid);

            if (ModelState.IsValid)
            {

                bool isUpdate = false;

                if (editUser.Login != model.Login)
                {
                    editUser.Login = model.Login;
                    isUpdate = true;
                }
                if (editUser.FirstName != model.FirstName)
                {
                    editUser.FirstName = model.FirstName;
                    isUpdate = true;
                }
                if (editUser.LastName != model.LastName)
                {
                    editUser.LastName = model.LastName;
                    isUpdate = true;
                }
                if (editUser.SurName != model.SurName)
                {
                    editUser.SurName = model.SurName is null ? "" : model.SurName;
                    isUpdate = true;
                }
                if (editUser.Password != model.Password)
                {
                    editUser.Password = model.Password;
                    isUpdate = true;
                }

                if (isUpdate)
                {
                    await _usersRepository.UpdateAsync(editUser);
                }

                _logger.LogInformation("Данные о пользователе успешно изменены");

                return await ListAsync();
            }
            else
            {
                _logger.LogWarning("Ошибка Верификации при редактировании пользователя");
                return View("Edit", model);
            }
        }

        /// <summary>
        /// Страница по редактированию информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewUser(Guid guid)
        {
            var user = await _usersRepository.GetAsync(guid);
            UserViewModel model = new();

            if (user is not null)
            {
                model = _mapper.Map<User, UserViewModel>(user);

                var role = await _rolesRepository.GetAsync((int)user.RoleId);

                if (role != null)
                {
                    model.Role = role;
                }
            }

            _logger.LogInformation("Перешли на страницу просмотра информации о пользователе");

            return View(model);
        }

        /// <summary>
        /// Страница информации о текущем пользователе
        /// </summary>
        /// <returns></returns>
        [Route("Profile")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var login = User.Identity.Name;

            var user = _usersRepository.GetByLogin(login);

            UserViewModel model = new();

            if (user is not null)
            {
                model = _mapper.Map<User, UserViewModel>(user);

                var role = await _rolesRepository.GetAsync((int)user.RoleId);

                if (role != null)
                {
                    model.Role = role;
                }
            }

            _logger.LogInformation("Перешли на страницу просмотра информации о себе");

            return View("ViewUser", model);
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>       
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public async Task<IActionResult> ListAsync()
        {
            var listUsers = _usersRepository.GetAll().ToList();

            if (listUsers == null)
            {
                _logger.LogWarning("Пользователи на сайте отсутствуют");
                return View("Event", new EventViewModel() { Send = "Пользователи отсутствуют!" });
            }
            if (listUsers.Count == 0)
            {
                _logger.LogWarning("Пользователи на сайте отсутствуют");
                return View("Event", new EventViewModel() { Send = "Пользователи отсутствуют!" });
            }

            foreach (var user in listUsers)
            {
                var role = await _rolesRepository.GetAsync((int)user.RoleId);

                if (role != null)
                {
                    user.Role = role;
                }
            }

            ListViewModel view = new()
            {
                UserList = _mapper.Map<List<User>, List<UserViewModel>>(listUsers)
            };

            _logger.LogInformation("Перешли на страницу просмотра списка пользователей");

            return View("List", view);
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

            _logger.LogInformation("Пользователь успешно вышел с сайта");

            return RedirectToAction("Index", "Home");
        }

        private bool PasswordIsCorrect(User user)
        {
            var findUser = _usersRepository.GetByLogin(user.Login);

            if (findUser is null) { return false; }

            if (user.Password == null) { return false; }

            if (findUser.Password != user.Password) { return false; }

            return true;
        }

    }
}
