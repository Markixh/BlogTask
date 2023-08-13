using AutoMapper;
using Azure.Core;
using BlogTask.Contracts.Models.Article;
using BlogTask.Contracts.Models.Tags;
using BlogTask.Data.Models;
using BlogTask.Data.Queries;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models;
using BlogTask.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly ArticlesRepository _repository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
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
        //[HttpPost]
        //[Route("")]
        //public async Task<IActionResult> Add(ArticleRequest request)
       // {
       //     var newArticle = _mapper.Map<ArticleRequest, Article>(request);
       //     await _repository.CreateAsync(newArticle);
       //
         //   return StatusCode(200, newArticle);
       // }

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
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Обработка данных для добавления статьи
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
                return StatusCode(400, "Данные не внесены!");

            var newArticle = _mapper.Map<AddViewModel, Article>(model);
            newArticle.UserGuid = user.Guid;
            await _repository.CreateAsync(newArticle);

            return List();
        }

        /// <summary>
        /// Вывод формы для добавления статьи
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var article = await _repository.GetAsync(guid);

            var editArticle = _mapper.Map<Article, EditViewModel>(article);

            return View(editArticle);
        }

        /// <summary>
        /// Обработка данных для редактирования статьи
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(AddViewModel model)
        {
            var userName = User.Identity.Name;

            var user = _userRepository.GetByLogin(userName);

            if (model is null)
                return StatusCode(400, "Данные не внесены!");

            var newArticle = _mapper.Map<AddViewModel, Article>(model);
            newArticle.UserGuid = user.Guid;
            await _repository.CreateAsync(newArticle);

            return List();
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

            if (listArticles == null)
            {
                return View("Event", new EventViewModel() { Send = "Статьи отсутствуют!"});
            }
            if (listArticles.Count() == 0)
            {
                return View("Event", new EventViewModel() { Send = "Статьи отсутствуют!" });
            }

            ListViewModel view = new ListViewModel();
            view.List = _mapper.Map<List<Article>, List<ArticleViewModel>>(listArticles);

            return View("List", view);
        }

        /// <summary>
        /// Просмотр статьи
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewArticle(Guid guid)
        {
            var article = await _repository.GetAsync(guid);
            ArticleViewModel model = new ArticleViewModel();

            if (article is not null)
            {
                var user = await _userRepository.GetAsync(article.UserGuid);
                article.User = user;
                model = _mapper.Map<Article, ArticleViewModel>(article);
            }           
            
            return View(model);
        }
    }
}
