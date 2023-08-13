using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using BlogTask.Models;
using BlogTask.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RolesRepository _repository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Role>() as RolesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Вывод формы для добавления роли
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Обработка данных для добавления роли
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

            if (user.RoleId != 1)
                return StatusCode(400, "Отсутствует необходимая роль!");

            var newRole = _mapper.Map<AddViewModel, Role>(model);
            
            await _repository.CreateAsync(newRole);

            return List();
        }

        /// <summary>
        /// Вывод формы для редактирования роли
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAsync(int guid)
        {
            var role = await _repository.GetAsync(guid);

            var editRole = _mapper.Map<Role, EditViewModel>(role);

            return View(editRole);
        }

        /// <summary>
        /// Вывод списка ролей
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public IActionResult List()
        {

            var listArticles = _repository.GetAll().ToList();

            if (listArticles == null)
            {
                return View("Event", new EventViewModel() { Send = "Роли отсутствуют!" });
            }
            if (listArticles.Count() == 0)
            {
                return View("Event", new EventViewModel() { Send = "Роли отсутствуют!" });
            }

            ListViewModel view = new ListViewModel();
            view.List = _mapper.Map<List<Role>, List<RoleViewModel>>(listArticles);

            return View("List", view);
        }

        /// <summary>
        /// просмотр информации о роли
        /// </summary>
        /// <returns></returns>
        [Route("View")]
        [HttpGet]
        public async Task<IActionResult> ViewRoleAsync(int guid)
        {
            var role = await _repository.GetAsync(guid);
            RoleViewModel model = new RoleViewModel();

            if (role is not null)
            {                
                model = _mapper.Map<Role, RoleViewModel>(role);
            }
            return View(model);
        }
    }
}
