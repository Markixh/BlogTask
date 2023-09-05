using AutoMapper;
using BlogTask.Contracts.Models.Users;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using Microsoft.AspNetCore.Mvc;
using static BlogTask.Contracts.Models.Users.GetUserRequest;
using Microsoft.AspNetCore.Authorization;
using BlogTask.BLL.Services;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IService<User> _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<User> _logger;

        public UserController(IMapper mapper, ILogger<User> logger, IService<User> service) 
        {
            _userService = service;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Метод для получения всех пользователей
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Возвращает список всех пользователей</response>
        /// <response code="400">Если пользователи отсутствуют</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAllAsync().Result.ToArray();

            if (users == null)
            {
                _logger.LogWarning("Пользователи отсутствуют");
                return StatusCode(400);
            }
            if (users.Length == 0)
            {
                _logger.LogWarning("Пользователи отсутствуют");
                return StatusCode(400);
            }

            var resp = new GetUserResponse
            {
                UserAmount = users.Length,
                UserView = _mapper.Map<User[], UserView[]>(users)
            };

            _logger.LogInformation("Получен список пользователей в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для получения информации о пользователе по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Возвращает пользователя</response>
        /// <response code="400">Если пользователь отсутствуют</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var user = await _userService.GetAsync(guid);

            if (user == null)
            {
                _logger.LogWarning("Пользователь отсутствует");
                return StatusCode(400);
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

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для добавления нового пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">Пользователь успешно зарегистрирован</response>
        /// <response code="400">Такой пользователь уже существует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Registration(UserRequest request)
        {
            var user = await _userService.GetAsync(request.Guid);
            if (user != null)
            {
                _logger.LogWarning("Такой пользователь уже существует");
                return StatusCode(400, "Такой пользователь уже существует!");
            }

            var newUser = _mapper.Map<UserRequest, User>(request);
            await _userService.CreateAsync(newUser);

            _logger.LogInformation("Регистрация нового пользователя прошла успешно в API");

            return StatusCode(201, newUser);
        }

        /// <summary>
        /// Метод для обновления пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="201">Данные по пользователю изменены</response>
        /// <response code="400">Если пользователи отсутствуют</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] EditUserRequest request)
        {
            var user = await _userService.GetAsync(request.Guid);
            if (user == null)
            {
                _logger.LogWarning("Такой пользователь не существует");
                return StatusCode(400);
            }                       

            var updateUser = await ((UserService)_userService).UpdateAsync(
                user,
                new UpdateUserQuery(
                    request.NewLogin,
                    request.NewFirstName,
                    request.NewLastName,
                    request.NewSurname,
                    request.NewPassword));

            var resultUser = _mapper.Map<User, UserRequest>(updateUser);

            _logger.LogInformation("Данные пользователя успешно изменены через API");

            return StatusCode(201, resultUser);
        }

        /// <summary>
        /// Метод для удаления пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для удаления пользователя необходимы права администратора
        /// </remarks>
        /// <response code="201">Пользователь успешно удален</response>
        /// <response code="400">Если пользователи отсутствуют</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Администратор")]
        [Authorize]
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден");
                return StatusCode(400);
            }

            await _userService.DeleteAsync(user);

            _logger.LogInformation("Пользователя успешно удален через API");

            return StatusCode(201);
        }
    }
}
