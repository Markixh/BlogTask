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
        private readonly CommentsRepository _commentsRepository;
        private readonly IService<Comment> _commentService;
        private readonly IMapper _mapper;
        private readonly ILogger<Comment> _logger;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<Comment> _logger, IService<Comment> service)
        {
            _commentsRepository = unitOfWork.GetRepository<Comment>() as CommentsRepository;
            _commentService = service;
            _mapper = mapper;
            _logger.LogInformation("Создан CommentController");
        }
    }
}
