using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models;
using Microsoft.AspNetCore.Mvc;
using BlogTask.Models.Tag;
using Microsoft.AspNetCore.Authorization;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly TagsRepository _repository;
        private readonly UsersRepository _usersRepository;
        private readonly IService<Tag> _tagService;
        private readonly IMapper _mapper;
        private readonly ILogger<Tag> _logger;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Tag> logger, IService<Tag> service)
        {
            _repository = unitOfWork.GetRepository<Tag>() as TagsRepository;
            _usersRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _tagService = service;
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

            var user = _usersRepository.GetByLogin(userName);

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
                var newRole = _mapper.Map<AddViewModel, Tag>(model);

                await _repository.CreateAsync(newRole);

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
            var tag = await _repository.GetAsync(guid);

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
            var editTag = await _repository.GetAsync(model.Guid);

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
                    await _repository.UpdateAsync(editTag);
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
            var listTags = _repository.GetAll().ToList();

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
            var tag = await _repository.GetAsync(guid);
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
            var tag = _repository.GetAsync(guid);
            if (tag == null)
            {
                _logger.LogWarning("Тег отсутствует");
                return StatusCode(400, "Тэг не найден!");
            }

            await _repository.DeleteAsync(await tag);

            _logger.LogInformation("Тег успешно удален");

            return List();
        }
    }
}
