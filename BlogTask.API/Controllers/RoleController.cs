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
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var role = _roleService.GetAllAsync().Result.ToArray();

            var resp = new GetRoleResponse
            {
                RoleAmount = role.Length,
                RoleView = _mapper.Map<Role[], RoleView[]>(role)
            };

            _logger.LogInformation("Получен список ролей в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для получения роли по id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(int guid)
        {
            var role = await _roleService.GetAsync(guid);

            if (role == null)
            {
                _logger.LogWarning("Роль отсутствует");
                return StatusCode(400, $"Роль с id = {guid} отсутствует!");
            }

            var resp = new RoleView
            {
                Id = guid,
                Name = role.Name,
                Description = role.Description
            };

            _logger.LogInformation("Роль передана в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления новой роли
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(RoleRequest request)
        {
            var role = await _roleService.GetAsync(request.Id);
            if (role != null)
            {
                _logger.LogWarning("Роль отсутствует");
                return StatusCode(400, "Такая роль уже существует!");
            }

            var newRole = _mapper.Map<RoleRequest, Role>(request);
            await _roleService.CreateAsync(newRole);

            _logger.LogInformation("Роль успешно добавлена через API");

            return StatusCode(200, newRole);
        }

        /// <summary>
        /// Метод для изменения роли
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditRoleRequest request)
        {
            var role = await _roleService.GetAsync(request.Id);
            if (role == null)
            {
                _logger.LogWarning("Такая роль не существует");
                return StatusCode(400, "Такая роль не существует!");
            }

            var updateRole = await ((RoleService)_roleService).UpdateAsync(
                role,
                new UpdateRoleQuery(
                    request.NewName,
                    request.NewDescription));

            var resultRole = _mapper.Map<Role, RoleRequest>(updateRole);

            _logger.LogInformation("Роль успешно изменена через API");

            return StatusCode(200, resultRole);
        }

        /// <summary>
        /// Метод для удаления роли
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(int guid)
        {
            var role = await _roleService.GetAsync(guid);
            if (role == null)
            {
                _logger.LogWarning("Такой роли не найдено");
                return StatusCode(400, "Роль не найдена!");
            }

            await _roleService.DeleteAsync(role);

            _logger.LogInformation("Hjkm успешно удалена через API");

            return StatusCode(200);
        }
    }
}
