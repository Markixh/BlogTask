﻿using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RolesRepository _repository;
        private readonly IMapper _mapper;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = unitOfWork.GetRepository<Role>() as RolesRepository;
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
    }
}
