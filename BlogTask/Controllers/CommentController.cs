using AutoMapper;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data.UoW;
using Microsoft.AspNetCore.Mvc;

namespace BlogTask.Controllers
{
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly CommentsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<Comment> _logger;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Comment> _logger)
        {
            _repository = unitOfWork.GetRepository<Comment>() as CommentsRepository;
            _mapper = mapper;
            _logger.LogInformation("Создан CommentController");
        }
    }
}
