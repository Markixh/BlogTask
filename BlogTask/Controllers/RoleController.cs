using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models;
using BlogTask.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RolesRepository _repository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Role> _logger;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Role> logger)
        {
            _repository = unitOfWork.GetRepository<Role>() as RolesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("Создан RoleController");
        }

        /// <summary>
        /// Вывод формы для добавления роли
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            _logger.LogInformation("Пользователь перешел на страницу добавления роли");
            return View();
        }

        /// <summary>
        /// Обработка данных для добавления роли
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(AddViewModel model)
        {
            var userName = User.Identity.Name;

            var user = _userRepository.GetByLogin(userName);

            if (model is null)
            {
                _logger.LogWarning("Данные на странице добавления роли не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (user.RoleId != 1)
            {
                _logger.LogWarning("Отсутствует необходимая роль для добавления роли");
                return StatusCode(400, "Отсутствует необходимая роль!");
            }

            if (ModelState.IsValid)
            {

                var newRole = _mapper.Map<AddViewModel, Role>(model);

                await _repository.CreateAsync(newRole);

                _logger.LogInformation("Роль успешно добавлена");

                return List();
            }
            else
            {
                _logger.LogWarning("Данные при добавлении роли не прошли валидацию");
                return View(model);
            }
        }

        /// <summary>
        /// Вывод формы для редактирования роли
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAsync(int guid)
        {
            var role = await _repository.GetAsync(guid);

            var editRole = _mapper.Map<Role, EditViewModel>(role);

            _logger.LogInformation("Пользователь перешел на страницу редактирования роли");

            return View(editRole);
        }

        /// <summary>
        /// Обработка данных для редактирования роли
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var editRole = await _repository.GetAsync(model.Id);

            if (model is null)
            {
                _logger.LogWarning("Данные при редактировании роли не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (ModelState.IsValid)
            {
                bool isUpdate = false;

                if (editRole.Name != model.Name)
                {
                    editRole.Name = model.Name;
                    isUpdate = true;
                }
                if (editRole.Description != model.Description)
                {
                    editRole.Description = model.Description;
                    isUpdate = true;
                }

                if (isUpdate)
                {
                    await _repository.UpdateAsync(editRole);
                }

                _logger.LogInformation("Роль успешно изменена");

                return List();
            }
            else
            {
                _logger.LogWarning("Данные при редактировании роли не прошли валидацию");
                return View(model);
            }
        }

        /// <summary>
        /// Вывод списка ролей
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            var listArticles = _repository.GetAll().ToList();

            if (listArticles == null)
            {
                _logger.LogWarning("Роли отсутствуют");
                return View("Event", new EventViewModel() { Send = "Роли отсутствуют!" });
            }
            if (listArticles.Count == 0)
            {
                _logger.LogWarning("Роли отсутствуют");
                return View("Event", new EventViewModel() { Send = "Роли отсутствуют!" });
            }

            ListViewModel view = new()
            {
                List = _mapper.Map<List<Role>, List<RoleViewModel>>(listArticles)
            };

            _logger.LogInformation("Пользователь перешел на страницу просмотра списка ролей");

            return View("List", view);
        }

        /// <summary>
        /// просмотр информации о роли
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewRoleAsync(int guid)
        {
            var role = await _repository.GetAsync(guid);
            RoleViewModel model = new();

            if (role is not null)
            {
                _logger.LogWarning("Роль отсутствуют");
                model = _mapper.Map<Role, RoleViewModel>(role);
            }

            _logger.LogInformation("Пользователь перешел на страницу просмотра роли");

            return View(model);
        }

        /// <summary>
        /// Метод для удаления роли
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Del")]
        [Authorize]
        public async Task<IActionResult> Del(int guid)
        {
            var role = _repository.GetAsync(guid);
            if (role == null)
            {
                _logger.LogWarning("Роль не найдена");
                return StatusCode(400, "Роль не найдена!");
            }

            await _repository.DeleteAsync(await role);

            _logger.LogInformation("Удаление роли прошло успешно");

            return List();
        }
    }
}
