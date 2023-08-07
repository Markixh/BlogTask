using AutoMapper;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Contracts.Models.Users;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;
using static BlogTask.Contracts.Models.Users.GetUserRequest;
using Microsoft.AspNetCore.Authorization;

namespace BlogTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UsersRepository _repository;
        private readonly IMapper _mapper;
                
        public UserController(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _repository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Метод для получения всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var user = _repository.GetAll().ToArray();

            var resp = new GetUserResponse
            {
                UserAmount = user.Length,
                UserView = _mapper.Map<User[], UserView[]>(user)
            };

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для получения информации о пользователе по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var user = await _repository.GetAsync(guid);

            if (user == null)
                return StatusCode(400, $"Пользователь с Guid = {guid} отсутствует!");

            var resp = new UserView
            {
                Guid = guid,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Surname = user.SurName
            };

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для добавления нового пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Registration(UserRequest request)
        {
            var user = await _repository.GetAsync(request.Guid);
            if (user != null)
                return StatusCode(400, "Такой пользователь уже существует!");

            var newUser = _mapper.Map<UserRequest, User>(request);
            await _repository.CreateAsync(newUser);

            return StatusCode(200, newUser);
        }

        /// <summary>
        /// Метод для обновления пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] EditUserRequest request)
        {
            var user = await _repository.GetAsync(request.Guid);
            if (user == null)
                return StatusCode(400, "Такой пользователь не существует!");
                       

            var updateUser = _repository.UpdateByUser(
                user,
                new UpdateUserQuery(
                    request.NewLogin,
                    request.NewFirstName,
                    request.NewLastName,
                    request.NewSurname,
                    request.NewPassword));

            var resultUser = _mapper.Map<User, UserRequest>(updateUser);

            return StatusCode(200, resultUser);
        }

        /// <summary>
        /// Метод для удаления пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _repository.GetAsync(id);
            if (user == null)
                return StatusCode(400, "Пользователь не найден!");

            await _repository.DeleteAsync(user);

            return StatusCode(200);
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate(string login, string password)
        {
            if (String.IsNullOrEmpty(login) ||
              String.IsNullOrEmpty(password))
                return StatusCode(400, "Запрос не корректен!");

            User user = _repository.GetByLogin(login);
            if (user is null)
                return StatusCode(400, "Пользователь на найден!");

            if (user.Password != password)
                return StatusCode(400, "Введенный пароль не корректен!");

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };

            ClaimsIdentity claimsIdentity = new(
                claims,
                "AddCookies",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


            return StatusCode(200, _mapper.Map<User, UserView>(user));
        }
    }
}
