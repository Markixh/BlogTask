using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    public class RegisterController : Controller
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public RegisterController(IMapper mapper, IUnitOfWork unitOfWork)
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
            return View("Home");
        }
    }
}
