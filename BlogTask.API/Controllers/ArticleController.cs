using AutoMapper;
using BlogTask.BLL.Services;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ArticleController : ControllerBase
    { 
        private readonly IMapper _mapper;
        private readonly IService<Article> _articleService;
        private readonly ILogger<Article> _logger;

        public ArticleController(IMapper mapper, IService<Article> service, ILogger<Article> logger)
        {
            _mapper = mapper;
            _articleService = service;
            _logger = logger;
            _logger.LogInformation("Создан AccountManagerController");
        }

        /// <summary>
        /// Метод для получения всех статей
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Возвращает список статей</response>
        /// <response code="400">Если статей нет</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var articles = _articleService.GetAllAsync().Result.ToArray();

            if (articles == null)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }
            if (articles.Length == 0)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return StatusCode(400);
            }

            var resp = new GetArticleResponse
            {
                ArticleAmount = articles.Length,
                ArticleView = _mapper.Map<Article[], ArticleView[]>(articles)
            };

            _logger.LogInformation("Получен список статей в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для получения статьи по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Статья отсутствует</response>
        /// <response code="400">Статья отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var article = await _articleService.GetAsync(guid);

            if (article == null)
            {
                _logger.LogWarning("Статья отсутствует");
                return StatusCode(400);
            }

            var resp = new ArticleView
            {
                Guid = guid,
                Text = article.Text,
                Title = article.Title,
                UserGuid = article.UserGuid,
                Tags = _mapper.Map<Tag[], TagView[]>(article.Tags.ToArray())
            };

            _logger.LogInformation("Статья передана в API");

            return StatusCode(201, resp);
        }

        /// <summary>
        /// Метод для дабавления новой статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Пример запроса:
        ///
        ///     POST /Статьи
        ///     {
        ///        "title": "Название статьи",
        ///        "text": "Текст статьи"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Статья успешно добавлена</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(ArticleRequest request)
        {
            var newArticle = _mapper.Map<ArticleRequest, Article>(request);
            await _articleService.CreateAsync(newArticle);

            _logger.LogInformation("Статья успешно добавлена через API");

            return StatusCode(201, newArticle);
        }

        /// <summary>
        /// Метод для изменения статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// Пример запроса:
        ///
        ///     PATCH /Статьи
        ///     {
        ///        "guid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "newTitle": "Название статьи",
        ///        "newText": "Текст статьи"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Статья успешно добавлена</response>
        /// <response code="400">Статья отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditArticleRequest request)
        {
            var article = await _articleService.GetAsync(request.Guid);
            if (article == null)
            {
                _logger.LogWarning("Статья отсутствует");
                return StatusCode(400);
            }

            var updateArticle = await ((ArticleService)_articleService).UpdateAsync(
                article,
                new UpdateArticleQuery(
                    request.NewTitle,
                    request.NewText
                    ));

            var resultArticle = _mapper.Map<Article, ArticleRequest>(updateArticle);

            _logger.LogInformation("Статья изменена через API");

            return StatusCode(201, resultArticle);
        }

        /// <summary>
        /// Метод для удаления статьи
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <response code="201">Статья успешно удалена</response>
        /// <response code="400">Статья отсутствует</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var article = await _articleService.GetAsync(guid);
            if (article == null)
            {
                _logger.LogWarning("Статья не найдена");
                return StatusCode(400);
            }

            await _articleService.DeleteAsync(article);

            _logger.LogInformation("Статья удалена через API");

            return StatusCode(201);
        }
    }
}
