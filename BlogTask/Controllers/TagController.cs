using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models;
using Microsoft.AspNetCore.Mvc;
using BlogTask.Models.Tag;
using Microsoft.AspNetCore.Authorization;
using BlogTask.BLL.Services;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly IService<User> _userService;
        private readonly IService<Tag> _tagService;
        private readonly IMapper _mapper;
        private readonly ILogger<Tag> _logger;

        public TagController(IMapper mapper, ILogger<Tag> logger, IService<Tag> tagService, IService<User> userService)
        {
            _userService = userService;
            _tagService = tagService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Вывод формы для добавления тега
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            _logger.LogInformation("Пользователь перешел на страницу добавления тега");
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

            var user = await ((UserService)_userService).GetByLogin(userName);

            if (model is null)
            {
                _logger.LogWarning("Данные для добавлении тега не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (user.RoleId != 2)
            {
                _logger.LogWarning("Отсутствует необходимая роль для добавления тега");
                return StatusCode(400, "Отсутствует необходимая роль!");
            }

            if (ModelState.IsValid)
            {
                var newTag = _mapper.Map<AddViewModel, Tag>(model);

                await _tagService.CreateAsync(newTag);

                _logger.LogInformation("Тег успешно добавлена");

                return List();
            }
            else
            {
                _logger.LogWarning("Данные для добавлении тега не прошли валидацию");
                return View(model);
            }
        }

        /// <summary>
        /// Вывод формы для добавления тега
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var tag = await _tagService.GetAsync(guid);

            var editTag = _mapper.Map<Tag, EditViewModel>(tag);

            _logger.LogInformation("Пользователь перешел на страницу редактирования тега");

            return View(editTag);
        }

        /// <summary>
        /// Обработка данных для редактирования статьи
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var editTag = await _tagService.GetAsync(model.Guid);

            if (model is null)
            {
                _logger.LogWarning("Данные для редактирования тега не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (ModelState.IsValid)
            {
                if (editTag.Name != model.Name)
                {
                    editTag.Name = model.Name;
                    await _tagService.UpdateAsync(editTag);
                }

                _logger.LogInformation("Тег успешно изменен");

                return List();
            }
            else
            {
                _logger.LogWarning("Данные для редактирования тега не прошли валидацию");
                return View(model);
            }
        }

        /// <summary>
        /// Вывод списка тегов
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            var listTags = _tagService.GetAllAsync().Result.ToList();

            if (listTags == null)
            {
                _logger.LogWarning("Теги отсутствуют");
                return View("Event", new EventViewModel() { Send = "Теги отсутствуют!" });
            }
            if (listTags.Count == 0)
            {
                _logger.LogWarning("Теги отсутствуют");
                return View("Event", new EventViewModel() { Send = "Теги отсутствуют!" });
            }

            ListViewModel view = new()
            {
                List = _mapper.Map<List<Tag>, List<TagViewModel>>(listTags)
            };

            _logger.LogInformation("Пользователь перешел на страницу просмотра всех тегов");

            return View("List", view);
        }

        /// <summary>
        /// Вывод информации о теге
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewTagAsync(Guid guid)
        {
            var tag = await _tagService.GetAsync(guid);
            TagViewModel model = new();

            if (tag is not null)
            {
                _logger.LogWarning("Тег отсутствует");
                model = _mapper.Map<Tag, TagViewModel>(tag);
            }

            _logger.LogInformation("Пользователь перешел на страницу просмотра тега");

            return View(model);
        }

        /// <summary>
        /// Метод для удаления тега
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Del")]
        [Authorize]
        public async Task<IActionResult> Del(Guid guid)
        {
            var tag = _tagService.GetAsync(guid);
            if (tag == null)
            {
                _logger.LogWarning("Тег отсутствует");
                return StatusCode(400, "Тэг не найден!");
            }

            await _tagService.DeleteAsync(await tag);

            _logger.LogInformation("Тег успешно удален");

            return List();
        }
    }
}
