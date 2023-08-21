using AutoMapper;
using BlogTask.BLL.Services;
using BlogTask.Data;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog;
using NLog.Web;

namespace BlogTask
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

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


                // Add services to the container.
                builder.Services.AddControllersWithViews();

                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Swagger Demo API",
                        Description = "Demo API for showing Swagger",
                        Version = "v1"
                    });
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

                // обработка ошибок HTTP
                app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Demo API");
                });

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