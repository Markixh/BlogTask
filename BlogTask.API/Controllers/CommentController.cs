using AutoMapper;
using BlogTask.BLL.Services;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Comment;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IService<Comment> _commentService;
        private readonly IMapper _mapper;
        private readonly ILogger<Comment> _logger;

        public CommentController(IMapper mapper, ILogger<Comment> _logger, IService<Comment> service)
        {
            _commentService = service;
            _mapper = mapper;
            _logger.LogInformation("Создан CommentController");
        }

        /// <summary>
        /// Метод для получения всех комментариев
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var comment = _commentService.GetAllAsync().Result.ToArray();

            var resp = new GetCommentResponse
            {
                CommentAmount = comment.Length,
                CommentView = _mapper.Map<Comment[], CommentView[]>(comment)
            };

            _logger.LogInformation("Получен список комментариев в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для получения комментария по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var comment = await _commentService.GetAsync(guid);

            if (comment == null)
            {
                _logger.LogWarning("Комментарий отсутствует");
                return StatusCode(400, $"Комментарий с Guid = {guid} отсутствует!");
            }

            var resp = new CommentView
            {
                Guid = guid,
                Text = comment.Text,
                UserGuid = comment.UserGuid,
                ArticleGuid = comment.ArticleGuid
            };

            _logger.LogInformation("Комментарий передан в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления нового коментария
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(CommentRequest request)
        {
            var comment = await _commentService.GetAsync(request.Guid);
            if (comment != null)
            {
                _logger.LogWarning("Такой комментарий уже существует");
                return StatusCode(400, "Такой комментарий уже существует!");
            }

            var newComment = _mapper.Map<CommentRequest, Comment>(request);
            await _commentService.CreateAsync(newComment);

            _logger.LogInformation("Комментарий успешно добавлена через API");

            return StatusCode(200, newComment);
        }

        /// <summary>
        /// Метод для изменения комментария
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditCommentRequest request)
        {
            var comment = await _commentService.GetAsync(request.Guid);
            if (comment == null)
            {
                _logger.LogWarning("Такой комментарий не существует");
                return StatusCode(400, "Такой комментарий не существует!");
            }

            var updateComment = await ((CommentService)_commentService).UpdateAsync(
                comment,
                new UpdateCommentQuery(
                    request.NewText
                    ));

            var resultComment = _mapper.Map<Comment, CommentRequest>(updateComment);

            _logger.LogInformation("Комментарий успешно изменен через API");

            return StatusCode(200, resultComment);
        }

        /// <summary>
        /// Метод для удаления комментария
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var comment =  await _commentService.GetAsync(guid);
            if (comment == null)
            {
                _logger.LogWarning("Комментарий не найден");
                return StatusCode(400, "Комментарий не найден!");
            }

            await _commentService.DeleteAsync(comment);

            _logger.LogInformation("Комментарий успешно удален через API");

            return StatusCode(200);
        }
    }
}
