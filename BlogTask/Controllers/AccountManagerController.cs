using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

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

        public IActionResult Index()
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
                var user = _mapper.Map<User>(model);

                if (PasswordIsCorrect(user))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Home", "error");
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



        public bool PasswordIsCorrect(User user)
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
