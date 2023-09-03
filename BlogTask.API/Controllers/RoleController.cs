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
        private readonly IService<User> _userService;
        private readonly IService<Role> _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<Role> _logger;

        public RoleController(IMapper mapper, ILogger<Role> logger, IService<Role> roleService, IService<User> userService)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("Создан RoleController");
        }
    }
}
