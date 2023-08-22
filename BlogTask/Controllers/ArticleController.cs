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

        /// <summary>
        /// Вывод формы для добавления статьи
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            _logger.LogInformation("Пользователь перешел на страницу добавления статьи");
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
            {
                _logger.LogWarning("Данные на странице добавления статьи не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (ModelState.IsValid)
            {
                var newArticle = _mapper.Map<AddViewModel, Article>(model);
                newArticle.UserGuid = user.Guid;
                await _repository.CreateAsync(newArticle);

                _logger.LogInformation("Статья успешно добавлена");

                return await ListAsync();
            }
            else
            {
                _logger.LogWarning("Данные при добавлении статьи не прошли валидацию");
                return View(model);
            }
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

            _logger.LogInformation("Пользователь перешел на страницу изменения статьи");

            return View(editArticle);
        }

        /// <summary>
        /// Обработка данных для редактирования статьи
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var editArticle = await _repository.GetAsync(model.Guid);

            if (model is null)
            {
                _logger.LogWarning("Данные изменения статьи не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (ModelState.IsValid)
            {
                bool isUpdate = false;

                if (editArticle.Title != model.Title)
                {
                    editArticle.Title = model.Title;
                    isUpdate = true;
                }
                if (editArticle.Text != model.Text)
                {
                    editArticle.Text = model.Text;
                    isUpdate = true;
                }

                if (isUpdate)
                {

                    await _repository.UpdateAsync(editArticle);
                }

                _logger.LogInformation("Статья успешно изменена");

                return await ListAsync();
            }
            else
            {
                _logger.LogWarning("Данные для изменения статьи не прошли валидацию");
                return View(model);
            }
        }

        /// <summary>
        /// Вывод списка статей
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public async Task<IActionResult> ListAsync()
        {
            var listArticles = _sevice.GetAllAsync().Result.ToList();

            if (listArticles == null)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return View("Event", new EventViewModel() { Send = "Статьи отсутствуют!" });
            }
            if (listArticles.Count == 0)
            {
                _logger.LogWarning("Статьи отсутствуют");
                return View("Event", new EventViewModel() { Send = "Статьи отсутствуют!" });
            }

            ListViewModel view = new()
            {
                List = _mapper.Map<List<Article>, List<ArticleViewModel>>(listArticles)
            };

            _logger.LogInformation("Пользователь перешел на страницу просмотра всех статей");

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
            var article = _repository.GetWithTags(guid);
            ArticleViewModel model = new();

            if (article is not null)
            {
                _logger.LogInformation("Статья отсутствует");
                model = _mapper.Map<Article, ArticleViewModel>(article);
            }

            _logger.LogInformation("Пользователь перешел на страницу просмотра статьи");

            return View(model);
        }
    }
}
