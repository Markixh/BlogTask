using AutoMapper;
using BlogTask.Data;
using BlogTask.Data.Models;
using BlogTask.Data.Repositories;
using BlogTask.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BlogTask
{
    public class Program
    {
        public static void Main(string[] args)
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

            var app = builder.Build();

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

        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddJsonFile("appsettings.Development.json")
          .Build();
    }
}