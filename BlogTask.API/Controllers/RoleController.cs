using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BlogTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RolesRepository _rolesRepository;
        private readonly UsersRepository _usersRepository;
        private readonly IService<Role> _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<Role> _logger;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Role> logger, IService<Role> service)
        {
            _rolesRepository = unitOfWork.GetRepository<Role>() as RolesRepository;
            _usersRepository = unitOfWork.GetRepository<User>() as UsersRepository;
            _roleService = service;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("Создан RoleController");
        }
    }
}
