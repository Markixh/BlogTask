using AutoMapper;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogTask.BLL.Services;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TagController : ControllerBase
    {
        private readonly IService<Tag> _tagService;
        private readonly IMapper _mapper;
        private readonly ILogger<Tag> _logger;

        public TagController(IMapper mapper, ILogger<Tag> logger, IService<Tag> tagService, IService<User> userService)
        {
            _tagService = tagService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Метод для получения всех тэгов
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Возвращает список тегов</response>
        /// <response code="400">Если тегов нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var tags = _tagService.GetAllAsync().Result.ToArray();

            if (tags == null)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }
            if (tags.Length == 0)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }

            var resp = new GetTagResponse
            {
                TagAmount = tags.Length,
                TagView = _mapper.Map<Tag[], TagView[]>(tags)
            };

            _logger.LogInformation("Получен список тегов в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для получения тэга по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Возвращает тег</response>
        /// <response code="400">Если тега нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var tag = await _tagService.GetAsync(guid);

            if (tag == null)
            {
                _logger.LogWarning("Тег отсутствует");
                return StatusCode(400);
            }

            var resp = new TagView
            {
                Guid = guid,
                Name = tag.Name
            };

            _logger.LogInformation("Тег передана в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для дабавления нового тэга
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для добавления тега необходимы права модератора
        /// </remarks>
        /// <response code="201">Тег успешно добавлен</response>
        /// <response code="400">Такой тег уже существует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Модератор")]
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(TagRequest request)
        {
            var tag = await _tagService.GetAsync(request.Guid);
            if (tag != null)
            {
                _logger.LogWarning("Такой тег существует");
                return StatusCode(400);
            }

            var newTag = _mapper.Map<TagRequest, Tag>(request);
            await _tagService.CreateAsync(newTag);

            _logger.LogInformation("Тег успешно добавлен через API");

            return StatusCode(201, newTag);
        }

        /// <summary>
        /// Метод для изменения тега
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для изменения тега необходимы права модератора
        /// </remarks>
        /// <response code="201">Тег успешно изменен</response>
        /// <response code="400">Если тега нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Модератор")]
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditTagRequest request)
        {
            var tag = await _tagService.GetAsync(request.Guid);
            if (tag == null)
            {
                _logger.LogWarning("Такой тэг не существует");
                return StatusCode(400);
            }

            var updateTag = await ((TagService)_tagService).UpdateAsync(
                tag,
                new UpdateTagQuery(
                    request.NewName));

            var resultTag = _mapper.Map<Tag, TagRequest>(updateTag);

            _logger.LogInformation("Тег успешно изменен через API");

            return StatusCode(201, resultTag);
        }

        /// <summary>
        /// Метод для удаления тега
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для удаления тега необходимы права модератора
        /// </remarks>
        /// <response code="201">Тег успешно удален</response>
        /// <response code="400">Если тега нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Модератор")]
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var tag = _tagService.GetAsync(guid);
            if (tag == null)
            {
                _logger.LogWarning("Такой тэг не найден");
                return StatusCode(400, "Тэг не найден!");
            }

            await _tagService.DeleteAsync(await tag);

            _logger.LogInformation("Тег успешно удален через API");

            return StatusCode(201);
        }
    }
}
