using AutoMapper;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.API.Controllers
{
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly ArticlesRepository _repository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IService<Article> _sevice;
        private readonly ILogger<Article> _logger;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, IService<Article> service, ILogger<Article> logger)
        {
            _repository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
            _sevice = service;
            _logger = logger;
            _logger.LogInformation("Создан AccountManagerController");
        }

        /// <summary>
        /// Метод для получения всех статей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var articles = _repository.GetAll().ToArray();

            var resp = new GetAeticleResponse
            {
                ArticleAmount = articles.Length,
                ArticleView = _mapper.Map<Article[], ArticleView[]>(articles)
            };

            _logger.LogInformation("Получен список статей в API");

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для получения статьи по Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byGuid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var article = await _repository.GetAsync(guid);

            if (article == null)
            {
                _logger.LogWarning("Статья отсутствует");
                return StatusCode(400, $"Статья с Guid = {guid} отсутствует!");
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

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления новой статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Add(ArticleRequest request)
        {
            var newArticle = _mapper.Map<ArticleRequest, Article>(request);
            await _repository.CreateAsync(newArticle);

            _logger.LogInformation("Статья успешно добавлена через API");

            return StatusCode(200, newArticle);
        }

        /// <summary>
        /// Метод для изменения статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] EditArticleRequest request)
        {
            var article = await _repository.GetAsync(request.Guid);
            if (article == null)
            {
                _logger.LogWarning("Статья отсутствует");
                return StatusCode(400, "Такая статья не существует!");
            }


            var updateArticle = _repository.UpdateByArticle(
                article,
                new UpdateArticleQuery(
                    request.NewTitle,
                    request.NewText
                    ));

            var resultArticle = _mapper.Map<Article, ArticleRequest>(updateArticle);

            _logger.LogInformation("Статья изменена через API");

            return StatusCode(200, resultArticle);
        }

        /// <summary>
        /// Метод для удаления статьи
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var article = await _repository.GetAsync(guid);
            if (article == null)
            {
                _logger.LogWarning("Статья не найдена");
                return StatusCode(400, "Статья не найдена!");
            }

            await _repository.DeleteAsync(article);

            _logger.LogInformation("Статья удалена через API");

            return StatusCode(200);
        }
    }
}
