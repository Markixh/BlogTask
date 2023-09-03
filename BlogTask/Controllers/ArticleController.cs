using AutoMapper;
using BlogTask.Data.Models;
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
        private readonly ArticlesRepository _articleRepository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IService<Article> _sevice;
        private readonly ILogger<Article> _logger;

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, IService<Article> service, ILogger<Article> logger)
        {
            _articleRepository = unitOfWork.GetRepository<Article>() as ArticlesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
            _sevice = service;
            _logger = logger;
            _logger.LogInformation("Создан AccountManagerController");
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
                await _articleRepository.CreateAsync(newArticle);

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
            var article = await _articleRepository.GetAsync(guid);

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
            var editArticle = await _articleRepository.GetAsync(model.Guid);

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

                    await _articleRepository.UpdateAsync(editArticle);
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
            var article = _articleRepository.GetWithTags(guid);
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
