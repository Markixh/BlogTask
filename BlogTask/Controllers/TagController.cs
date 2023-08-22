using AutoMapper;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
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
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Tag> _logger;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Tag> logger)
        {
            _repository = unitOfWork.GetRepository<Tag>() as TagsRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Метод для получения всех тэгов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var tag = _repository.GetAll().ToArray();

            var resp = new GetTagResponse
            {
                TagAmount = tag.Length,
                TagView = _mapper.Map<Tag[], TagView[]>(tag)
            };

            _logger.LogInformation("Получен список тегов в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для получения тэга по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var tag = await _repository.GetAsync(guid);

            if (tag == null)
            {
                _logger.LogWarning("Тег отсутствует");
                return StatusCode(400, $"Tэг с Guid = {guid} отсутствует!");
            }

            var resp = new TagView
            {
                Guid = guid,
                Name = tag.Name
            };

            _logger.LogInformation("Тег передана в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления нового тэга
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(TagRequest request)
        {
            var tag = await _repository.GetAsync(request.Guid);
            if (tag != null)
            {
                _logger.LogWarning("Тег отсутствует");
                return StatusCode(400, "Такой тэг уже существует!");
            }

            var newTag = _mapper.Map<TagRequest, Tag>(request);
            await _repository.CreateAsync(newTag);

            _logger.LogInformation("Тег успешно добавлен через API");

            return StatusCode(200, newTag);
        }

        /// <summary>
        /// Метод для изменения тега
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditTagRequest request)
        {
            var tag = await _repository.GetAsync(request.Guid);
            if (tag == null)
            {
                _logger.LogWarning("Такой тэг не существует");
                return StatusCode(400, "Такой тэг не существует!");
            }

            var updateTag = _repository.UpdateByTag(
                tag,
                new UpdateTagQuery(
                    request.NewName));

            var resultTag = _mapper.Map<Tag, TagRequest>(updateTag);

            _logger.LogInformation("Тег успешно изменен через API");

            return StatusCode(200, resultTag);
        }

        /// <summary>
        /// Метод для удаления тега
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var tag = _repository.GetAsync(guid);
            if (tag == null)
            {
                _logger.LogWarning("Такой тэг не найден");
                return StatusCode(400, "Тэг не найден!");
            }

            await _repository.DeleteAsync(await tag);

            _logger.LogInformation("Тег успешно удален через API");

            return StatusCode(200);
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

            var user = _userRepository.GetByLogin(userName);

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
