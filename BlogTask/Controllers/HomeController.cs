using BlogTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogInformation("Создан HomeController");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error")]
        [HttpGet]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 400 || statusCode == 403 || statusCode == 404)
                {
                    var viewName = statusCode.ToString();
                    _logger.LogError($"Произошла ошибка - {statusCode}\n{viewName}");
                    return View(viewName);
                }
            }
            return View("400");
        }

        /// <summary>
        /// Генерация ошибки 400
        /// </summary>
        /// <returns></returns>
        [Route("GetException400")]
        [HttpGet]
        public IActionResult GetException400()
        {
            try
            {
                throw new HttpRequestException("400");
            }
            catch
            {
                return View("400");
            }
        }

        /// <summary>
        /// Генерация ошибки 403
        /// </summary>
        /// <returns></returns>
        [Route("GetException403")]
        [HttpGet]
        public IActionResult GetException403()
        {
            try
            {
                throw new HttpRequestException("403");
            }
            catch
            {
                return View("403");
            }
        }

        /// <summary>
        /// Генерация ошибки 404
        /// </summary>
        /// <returns></returns>
        [Route("GetException404")]
        [HttpGet]
        public IActionResult GetException404()
        {
            try
            {
                throw new HttpRequestException("404");
            }
            catch
            {
                return View("404");
            }
        }
    }
}
