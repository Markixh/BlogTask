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
    [Produces("application/json")]
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
        /// <response code="201">Возвращает список комментариев</response>
        /// <response code="400">Если комментария нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var comments = _commentService.GetAllAsync().Result.ToArray();

            if (comments == null)
            {
                _logger.LogWarning("Комментарии отсутствуют");
                return StatusCode(400);
            }
            if (comments.Length == 0)
            {
                _logger.LogWarning("Комментарии отсутствуют");
                return StatusCode(400);
            }

            var resp = new GetCommentResponse
            {
                CommentAmount = comments.Length,
                CommentView = _mapper.Map<Comment[], CommentView[]>(comments)
            };

            _logger.LogInformation("Получен список комментариев в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для получения комментария по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Возвращает комментарий</response>
        /// <response code="400">Если комментарий отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var comment = await _commentService.GetAsync(guid);

            if (comment == null)
            {
                _logger.LogWarning("Комментарий отсутствует");
                return StatusCode(400);
            }

            var resp = new CommentView
            {
                Guid = guid,
                Text = comment.Text,
                UserGuid = comment.UserGuid,
                ArticleGuid = comment.ArticleGuid
            };

            _logger.LogInformation("Комментарий передан в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для дабавления нового коментария
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Пример запроса:
        ///
        ///     POST /Комментарии
        ///     {
        ///        "text": "Текст комментария",
        ///        "articleGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Комментарий успешно добавлен</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(CommentRequest request)
        {
            var newComment = _mapper.Map<CommentRequest, Comment>(request);
            await _commentService.CreateAsync(newComment);

            _logger.LogInformation("Комментарий успешно добавлена через API");

            return StatusCode(201, newComment);
        }

        /// <summary>
        /// Метод для изменения комментария
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Пример запроса:
        ///
        ///     PATCH /Комментарии
        ///     {
        ///        "guid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "newText": "Текст комментария"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Комментарий успешно изменен</response>
        /// <response code="400">Комментарий не существует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditCommentRequest request)
        {
            var comment = await _commentService.GetAsync(request.Guid);
            if (comment == null)
            {
                _logger.LogWarning("Такой комментарий не существует");
                return StatusCode(400);
            }

            var updateComment = await ((CommentService)_commentService).UpdateAsync(
                comment,
                new UpdateCommentQuery(
                    request.NewText
                    ));

            var resultComment = _mapper.Map<Comment, CommentRequest>(updateComment);

            _logger.LogInformation("Комментарий успешно изменен через API");

            return StatusCode(201, resultComment);
        }

        /// <summary>
        /// Метод для удаления комментария
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Комментарий успешно удален</response>
        /// <response code="400">Коментарий не найден</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var comment =  await _commentService.GetAsync(guid);
            if (comment == null)
            {
                _logger.LogWarning("Комментарий не найден");
                return StatusCode(400);
            }

            await _commentService.DeleteAsync(comment);

            _logger.LogInformation("Комментарий успешно удален через API");

            return StatusCode(201);
        }
    }
}
