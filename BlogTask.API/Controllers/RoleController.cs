using AutoMapper;
using BlogTask.BLL.Services;
using BlogTask.Contracts.Models.Role;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class RoleController : ControllerBase
    {
        private readonly IService<User> _userService;
        private readonly IService<Role> _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<Role> _logger;

        public RoleController(IMapper mapper, ILogger<Role> logger, IService<Role> roleService, IService<User> userService)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("Создан RoleController");
        }

        /// <summary>
        /// Метод для получения всех ролей
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Возвращает список ролей</response>
        /// <response code="400">Если ролей нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var roles = _roleService.GetAllAsync().Result.ToArray();

            if (roles == null)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }
            if (roles.Length == 0)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }

            var resp = new GetRoleResponse
            {
                RoleAmount = roles.Length,
                RoleView = _mapper.Map<Role[], RoleView[]>(roles)
            };

            _logger.LogInformation("Получен список ролей в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для получения роли по id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Возвращает роль</response>
        /// <response code="400">Если роль отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(int guid)
        {
            var role = await _roleService.GetAsync(guid);

            if (role == null)
            {
                _logger.LogWarning("Роль отсутствует");
                return StatusCode(400);
            }

            var resp = new RoleView
            {
                Id = guid,
                Name = role.Name,
                Description = role.Description
            };

            _logger.LogInformation("Роль передана в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для дабавления новой роли
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для добавления роли необходимы права администратора
        /// 
        /// Пример запроса:
        /// 
        ///     POST /Роли
        ///     {
        ///        "name": "Название роли",
        ///        "description": "описание роли"
        ///     }
        /// </remarks>
        /// <response code="201">Роль успешно добавлена</response>
        /// <response code="400">Такая роль уже существует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(RoleRequest request)
        {
            var role = _roleService.GetAllAsync().Result.ToArray();
            if (role != null)
            {
                _logger.LogWarning("Такая роль уже существует");
                return StatusCode(400);
            }

            var newRole = _mapper.Map<RoleRequest, Role>(request);
            await _roleService.CreateAsync(newRole);

            _logger.LogInformation("Роль успешно добавлена через API");

            return StatusCode(201, newRole);
        }

        /// <summary>
        /// Метод для изменения роли
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для добавления роли необходимы права администратора
        /// 
        /// Пример запроса:
        /// 
        ///     PATCH /Роли
        ///     {
        ///        "id": "id роли",
        ///        "newName": "Название роли",
        ///        "newDescription": "описание роли"
        ///     }
        /// </remarks>
        /// <response code="201">Роль успешно изменена</response>
        /// <response code="400">Роль отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Администратор")]
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditRoleRequest request)
        {
            var role = await _roleService.GetAsync(request.Id);
            if (role == null)
            {
                _logger.LogWarning("Такая роль не существует");
                return StatusCode(400);
            }

            var updateRole = await ((RoleService)_roleService).UpdateAsync(
                role,
                new UpdateRoleQuery(
                    request.NewName,
                    request.NewDescription));

            var resultRole = _mapper.Map<Role, RoleRequest>(updateRole);

            _logger.LogInformation("Роль успешно изменена через API");

            return StatusCode(201, resultRole);
        }

        /// <summary>
        /// Метод для удаления роли
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для удаления роли необходимы права администратора
        /// </remarks>
        /// <response code="201">Роль успешно удалена</response>
        /// <response code="400">Роль отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Администратор")]
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(int guid)
        {
            var role = await _roleService.GetAsync(guid);
            if (role == null)
            {
                _logger.LogWarning("Такой роли не найдено");
                return StatusCode(400);
            }

            await _roleService.DeleteAsync(role);

            _logger.LogInformation("Роль успешно удалена через API");

            return StatusCode(201);
        }
    }
}
