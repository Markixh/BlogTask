using AutoMapper;
using BlogTask.BLL.Services;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Data;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using BlogTask.API.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BlogTask.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                });

                IMapper mapper = mapperConfig.CreateMapper();
                builder.Services.AddSingleton(mapper);

                string? connection = Configuration.GetConnectionString("DefaultConnection");

                builder.Services
                   .AddDbContext<BlogContext>(options => options.UseSqlServer(connection))
                   .AddUnitOfWork()
                   .AddCustomRepository<User, UsersRepository>()
                   .AddCustomRepository<Article, ArticlesRepository>()
                   .AddCustomRepository<Tag, TagsRepository>()
                   .AddCustomRepository<Comment, CommentsRepository>()
                   .AddCustomRepository<Role, RolesRepository>();

                builder.Services.AddTransient<IService<Article>, ArticleService>();
                builder.Services.AddTransient<IService<Tag>, TagService>();
                builder.Services.AddTransient<IService<Role>, RoleService>();
                builder.Services.AddTransient<IService<User>, UserService>();

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Blog API",
                        Description = "ASP.NET Core Web API для управления бизнес-моделями Блога",
                        Contact = new OpenApiContact
                        {
                            Name = "Андрей Марков",
                            Email = "Andrejmarko@yandex.ru"
                        }
                    });

                    // using System.Reflection;
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });

                builder.Services.AddAuthentication(options => options.DefaultScheme = "Cookies")
                    .AddCookie("Cookies", options =>
                    {
                        options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = 401;
                                return Task.CompletedTask;
                            }
                        };
                    });

                // NLog: Setup NLog for Dependency injection
                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Host.UseNLog();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }

            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .AddJsonFile("appsettings.Development.json")
         .Build();
    }
}