using AutoMapper;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BlogTask.API.Controllers
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
    }
}
