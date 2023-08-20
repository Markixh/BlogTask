using AutoMapper;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Comment;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentsRepository _repository;
        private readonly IMapper _mapper;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Comment>() as CommentsRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Метод для получения всех комментариев
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var comment = _repository.GetAll().ToArray();

            var resp = new GetCommentResponse
            {
                CommentAmount = comment.Length,
                CommentView = _mapper.Map<Comment[], CommentView[]>(comment)
            };

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
            var comment = await _repository.GetAsync(guid);

            if (comment == null)
                return StatusCode(400, $"Комментарий с Guid = {guid} отсутствует!");

            var resp = new CommentView
            {
                Guid = guid,
                Text = comment.Text,
                UserGuid = comment.UserGuid,
                ArticleGuid = comment.ArticleGuid
            };

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
            var comment = await _repository.GetAsync(request.Guid);
            if (comment != null)
                return StatusCode(400, "Такой комментарий уже существует!");

            var newComment = _mapper.Map<CommentRequest, Comment>(request);
            await _repository.CreateAsync(newComment);

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
            var comment = await _repository.GetAsync(request.Guid);
            if (comment == null)
                return StatusCode(400, "Такой комментарий не существует!");


            var updateComment = _repository.UpdateByComment(
                comment,
                new UpdateCommentQuery(
                    request.NewText
                    ));

            var resultComment = _mapper.Map<Comment, CommentRequest>(updateComment);

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
            var comment =  await _repository.GetAsync(guid);
            if (comment == null)
                return StatusCode(400, "Коментарий не найден!");

            await _repository.DeleteAsync(comment);

            return StatusCode(200);
        }
    }
}
