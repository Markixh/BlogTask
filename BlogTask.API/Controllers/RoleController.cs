using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BlogTask.API.Controllers
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RolesRepository _repository;
        private readonly UsersRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Role> _logger;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Role> logger)
        {
            _repository = unitOfWork.GetRepository<Role>() as RolesRepository;
            _userRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("Создан RoleController");
        }
    }
}
