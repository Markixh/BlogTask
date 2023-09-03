using AutoMapper;
using BlogTask.Contracts.Models.Users;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using static BlogTask.Contracts.Models.Users.GetUserRequest;
using Microsoft.AspNetCore.Authorization;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UsersRepository _userRepository;
        private readonly IService<User> _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<User> _logger;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<User> logger, IService<User> service) 
        {
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _userService = service;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Метод для получения всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var user = _userRepository.GetAll().ToArray();

            var resp = new GetUserResponse
            {
                UserAmount = user.Length,
                UserView = _mapper.Map<User[], UserView[]>(user)
            };

            _logger.LogInformation("Получен список пользователей в API");

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
            var user = await _userRepository.GetAsync(guid);

            if (user == null)
            {
                _logger.LogWarning("Пользователь отсутствует");
                return StatusCode(400, $"Пользователь с Guid = {guid} отсутствует!");
            }

            var resp = new UserView
            {
                Guid = guid,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Surname = user.SurName
            };

            _logger.LogInformation("Данные пользователя переданы в API");

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
            var user = await _userRepository.GetAsync(request.Guid);
            if (user != null)
            {
                _logger.LogWarning("Такой пользователь уже существует");
                return StatusCode(400, "Такой пользователь уже существует!");
            }

            var newUser = _mapper.Map<UserRequest, User>(request);
            await _userRepository.CreateAsync(newUser);

            _logger.LogInformation("Регистрация нового пользователя прошла успешно в API");

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
            var user = await _userRepository.GetAsync(request.Guid);
            if (user == null)
            {
                _logger.LogWarning("Такой пользователь не существует");
                return StatusCode(400, "Такой пользователь не существует!");
            }
                       

            var updateUser = _userRepository.UpdateByUser(
                user,
                new UpdateUserQuery(
                    request.NewLogin,
                    request.NewFirstName,
                    request.NewLastName,
                    request.NewSurname,
                    request.NewPassword));

            var resultUser = _mapper.Map<User, UserRequest>(updateUser);

            _logger.LogInformation("Данные пользователя успешно изменены через API");

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
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден");
                return StatusCode(400, "Пользователь не найден!");
            }

            await _userRepository.DeleteAsync(user);

            _logger.LogInformation("Пользователя успешно удален через API");

            return StatusCode(200);
        }
    }
}
