using HotelManagement.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SchemaBuilder;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.FileProviders;
using HotelManagement.Domain;
using HotelManagement.Application;
using HotelManagement.Api;
using HotelManagement.Application.User.Service;

namespace HotelManagement.API
{
    public class Startup
    {

        public IWebHostEnvironment Env { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {   
            Env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Custom Scheme";
                options.DefaultChallengeScheme = "Custom Scheme";
            }).AddCustomAuth(o => { });
            services.AddCors();
            services.AddControllers();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddSingleton<AutoMapper.IConfigurationProvider>(AutoMapperConfig.RegisterMappings());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContextFactory<HotelDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IUnitOfWork, UnitOfWork>()
                .AddScoped<DatabaseFactory>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IDatabaseFactory, DatabaseFactory>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IRightService, RightService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IItemService, ItemService>()
                .AddScoped<IRoomService, RoomService>()
                .AddScoped<IHistoryService, HistoryService>()
                .AddScoped<IBookingService, BookingService>()
                .AddScoped<IContactService, ContactService>()
                .AddScoped<IRoomCategoryService, RoomCategoryService>()
                .AddScoped<IRightMapUserService, RightMapUserService>()
                .AddScoped<IUserMapRoleService, UserMapRoleService>()
                .AddScoped<IRegisterCodeService, RegisterCodeService>()
                .AddScoped<IRightMapRoleService, RightMapRoleService>();
            
            services.AddSwaggerSchemaBuilder();

            services.AddHealthChecks()
                .AddDbContextCheck<HotelDbContext>();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin",
                    options => options.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .WithExposedHeaders("Content-Disposition", "downloadfilename"));
            });
            var children = this.Configuration.GetSection("Caching").GetChildren();

            #region Swagger API Versioning

            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(cfg =>
            {
                cfg.GroupNameFormat = "'v'VVV";
                cfg.SubstituteApiVersionInUrl = true;
            });
            services.AddSwaggerGen(
                options =>
                {
                    var assembly = typeof(Startup).Assembly;
                    var assemblyProduct = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
                    //var assemblyDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

                    // options.DescribeAllEnumsAsStrings();
                    options.DescribeAllParametersInCamelCase();
                    // options.DescribeStringEnumsInCamelCase();
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Sử dụng Authen JWT. VD: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                        }
                    });

                    // Group name
                    // https://rimdev.io/swagger-grouping-with-controller-name-fallback-using-swashbuckle-aspnetcore/
                    options.TagActionsBy(api =>
                    {
                        if (api.GroupName != null)
                        {
                            return new[] { api.GroupName };
                        }

                        var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                        if (controllerActionDescriptor != null)
                        {
                            return new[] { controllerActionDescriptor.ControllerName };
                        }

                        throw new InvalidOperationException("Unable to determine tag for endpoint.");
                    });

                    options.DocInclusionPredicate((name, api) => true);

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                });

            // Newtonsoft json support
            //services.AddSwaggerGenNewtonsoftSupport();
           
            #endregion Swagger API Versioning
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(
                  options =>
                  {
                      options.DefaultModelRendering(ModelRendering.Model);
                      options.DisplayRequestDuration();
                      options.DocExpansion(DocExpansion.None);
                      options.EnableDeepLinking();
                      options.EnableFilter();
                      options.ShowExtensions();
                      options.EnableValidator();
                      options.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head, SubmitMethod.Post, SubmitMethod.Delete, SubmitMethod.Put);

                      var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
                      foreach (var apiVersionDescription in provider
                          .ApiVersionDescriptions
                          .OrderByDescending(x => x.ApiVersion).ThenByDescending(s => s.GroupName))
                      {
                          options.SwaggerEndpoint(
                              $"/swagger/{apiVersionDescription.GroupName}/swagger.json",
                              $"Version {apiVersionDescription.ApiVersion}");
                      }
                      options.InjectStylesheet("/swagger.css");
                  });
            app.UseRouting();

            app.UseCors(
               options => options.AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) 
               .AllowCredentials()
           );

            #region Prepare static files

            var path = Configuration["StaticFiles:Folder"];
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, string.Empty);
                }
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Configuration["StaticFiles:Folder"]),
                RequestPath = Configuration["StaticFiles:Request"]
            });

            #endregion Prepare static files

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        

        //private static ILogger ConfigureLogger()
        //{
        //    return new LoggerConfiguration()
        //        .Enrich.FromLogContext()
        //        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Context}] {Message:lj}{NewLine}{Exception}")
        //        .WriteTo.RollingFile(new CompactJsonFormatter(), "logs/logs")
        //        .CreateLogger();
        //}

    }
    
}
