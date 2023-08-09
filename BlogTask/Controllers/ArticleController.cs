using AutoMapper;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly ArticlesRepository _repository;
        private readonly IMapper _mapper;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
            _mapper = mapper;
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
                return StatusCode(400, $"Статья с Guid = {guid} отсутствует!");

            var resp = new ArticleView
            {
                Guid = guid,
                Text = article.Text,
                Title = article.Title,
                UserGuid = article.UserGuid,
                Tags = _mapper.Map<Tag[], TagView[]>(article.Tags.ToArray())
            };

            return StatusCode(200, resp);
        }

        /// <summary>
        /// Метод для дабавления новой статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Add(ArticleRequest request)
        {
            var article = await _repository.GetAsync(request.Guid);
            if (article != null)
                return StatusCode(400, "Такая статья уже существует!");

            var newArticle = _mapper.Map<ArticleRequest, Article>(request);
            await _repository.CreateAsync(newArticle);

            return StatusCode(200, newArticle);
        }

        /// <summary>
        /// Метод для изменения статьи
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] EditArticleRequest request)
        {
            var article = await _repository.GetAsync(request.Guid);
            if (article == null)
                return StatusCode(400, "Такая статья не существует!");


            var updateArticle = _repository.UpdateByArticle(
                article,
                new UpdateArticleQuery(
                    request.NewTitle,
                    request.NewText
                    ));

            var resultArticle = _mapper.Map<Article, ArticleRequest>(updateArticle);

            return StatusCode(200, resultArticle);
        }

        /// <summary>
        /// Метод для удаления статьи
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete(Guid guid)
        {
            var article = await _repository.GetAsync(guid);
            if (article == null)
                return StatusCode(400, "Статья не найдена!");

            await _repository.DeleteAsync(article);

            return StatusCode(200);
        }

        /// <summary>
        /// Вывод формы для добавления статьи
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Вывод списка статей
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {
            var listArticles = _repository.GetAll().ToList();

            if (listArticles == null) return StatusCode(400, "Статьи отсутствуют!");
            if (listArticles.Count() == 0) return StatusCode(400, "Статьи отсутствуют!");

            Models.Article.ListViewModel view = new Models.Article.ListViewModel();
            view.List = _mapper.Map<List<Article>, List<Models.Article.ArticleViewModel>>(listArticles);

            return View("List", view);
        }

        /// <summary>
        /// Просмотр статьи
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public IActionResult ViewArticle()
        {
            return View();
        }
    }
}
