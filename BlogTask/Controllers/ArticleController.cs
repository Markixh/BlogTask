using AutoMapper;
using BlogTask.BLL.Services;
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
        private readonly IMapper _mapper;
        private readonly IService<User> _userService;
        private readonly IService<Article> _articleService;
        private readonly ILogger<Article> _logger;

        public ArticleController(IMapper mapper, IService<Article> articleService, IService<User> userService, ILogger<Article> logger)
        {
            _mapper = mapper;
            _userService = userService;
            _articleService = articleService;
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

            var user = await ((UserService)_userService).GetByLogin(userName);

            if (model is null)
            {
                _logger.LogWarning("Данные на странице добавления статьи не внесены");
                return StatusCode(400, "Данные не внесены!");
            }

            if (ModelState.IsValid)
            {
                var newArticle = _mapper.Map<AddViewModel, Article>(model);
                newArticle.UserGuid = user.Guid;
                await _articleService.CreateAsync(newArticle);

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
            var article = await _articleService.GetAsync(guid);

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
            var editArticle = await _articleService.GetAsync(model.Guid);

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

                    await _articleService.UpdateAsync(editArticle);
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
            var listArticles = _articleService.GetAllAsync().Result.ToList();

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
            var article = await _articleService.GetAsync(guid);
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
