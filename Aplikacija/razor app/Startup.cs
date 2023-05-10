using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Globalization;
using backend.Filter;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using backend.ViewComponents;
namespace backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment en)
        {
            Configuration = configuration;
            env1=en;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment env1 { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DbContextSCActive> (options=>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SCActiveCS"));
            });

            services.AddCors(options=>
            {
                options.AddPolicy("CORS",builder=>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "backend", Version = "v1" });
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("sr"),
                };
                options.DefaultRequestCulture = new RequestCulture("sr");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddHttpContextAccessor();
            services.AddSession();
            services.AddMvc();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<IHashService, HashService>();
            services.AddTransient<ISessionDataService, SessionDataService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<ITranslatorService,TranslatorService>();
            services.AddSingleton<ICloudStorage, AzureStorage>();
            services.AddSingleton<IStorageConnectionFactory, StorageConnectionFactory>(sp =>
            {
                CloudStorageOptionsDTO cloudStorageOptionsDTO = new CloudStorageOptionsDTO();
                cloudStorageOptionsDTO.ConnectionString = Configuration["AzureBlobStorage:ConnectionString"];
                cloudStorageOptionsDTO.ProfilePicsContainer = Configuration["AzureBlobStorage:BlobContainer"];
                return new StorageConnectionFactory(cloudStorageOptionsDTO);

            });
           services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(new RazorPageFilter(new SessionDataService()));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "backend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
           
            app.UseRequestLocalization(localizationOptions);
            
            app.UseCors("CORS");
            
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseSession();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
