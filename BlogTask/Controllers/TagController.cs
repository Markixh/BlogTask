using AutoMapper;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Contracts.Models.Users;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using static BlogTask.Contracts.Models.Users.GetUserRequest;

namespace BlogTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly TagsRepository _repository;
        private readonly IMapper _mapper;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Tag>() as TagsRepository;
            _mapper = mapper;
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
                return StatusCode(400, $"Tэг с Guid = {guid} отсутствует!");

            var resp = new TagView
            {
                Guid = guid,
                Name = tag.Name
            };

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления нового тэга
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(TagRequest request)
        {
            var tag = await _repository.GetAsync(request.Guid);
            if (tag != null)
                return StatusCode(400, "Такой тэг уже существует!");

            var newTag = _mapper.Map<TagRequest, Tag>(request);
            await _repository.CreateAsync(newTag);

            return StatusCode(200, newTag);
        }

        /// <summary>
        /// Метод для изменения тега
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] EditTagRequest request)
        {
            var tag = await _repository.GetAsync(request.Guid);
            if (tag == null)
                return StatusCode(400, "Такой тэг не существует!");


            var updateTag = _repository.UpdateByTag(
                tag,
                new UpdateTagQuery(
                    request.NewName));

            var resultTag = _mapper.Map<Tag, TagRequest>(updateTag);

            return StatusCode(200, resultTag);
        }

        /// <summary>
        /// Метод для удаления тега
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var tag = _repository.GetAsync(guid);
            if (tag == null)
                return StatusCode(400, "Тэг не найден!");

            await _repository.DeleteAsync(await tag);

            return StatusCode(200);
        }

        /// <summary>
        /// Вывод формы для добавления тега
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Вывод формы для редактирования тега
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        /// <summary>
        /// Вывод списка тегов
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            return View("List");
        }
    }
}
