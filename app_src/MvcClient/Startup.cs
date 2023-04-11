#region header of Lib
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MvcClient.Models.Data;
using MvcClient.Models.Data.PdfData;
using MvcClient.Models.Data.TestpaperData;
using MvcClient.Models.PdfVms;
using MvcLib.Db;
using MvcLib.Dto.ColumnDto;
using MvcLib.Dto.PropDto;
using MvcLib.Dto.UploadDto;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UEditor.Core;
#endregion


namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public const string ConnString = "Default";

        // This method gets called by the runtime. Use this method to add services to the container.

        Action<OpenIdConnectOptions> OidcOpts = options =>
        {
            options.Authority = "http://localhost:5001";
            options.RequireHttpsMetadata = false;
            options.ClientId = "mvc";
            options.ClientSecret = "secret";
            options.ResponseType = "code";
            options.SaveTokens = true;
        };

        Action<AuthenticationOptions> OAuthOpts = options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
        };
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<FormOptions>(ops =>
            {
                ops.MultipartBodyLengthLimit = int.MaxValue;
                ops.MultipartBoundaryLengthLimit = int.MaxValue;
            });
            //services.AddCors(opts =>
            //{
            //    opts.AddPolicy("myCors", b =>
            //    {
            //        b.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader();
            //    });
            //});

            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        
            services.AddAuthentication(OAuthOpts)
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", OidcOpts);
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(Configuration.GetConnectionString(ConnString),
                options =>
                {
                    options.MigrationsAssembly("MvcLib");
                }));

  

            services.AddUEditorService(); //ueditor lib
            services.AddMvcCore().AddNewtonsoftJson();
            services.AddTransient<ISidebarRepo, SidebarRepo>();
            services.AddTransient<IPdfRepo, PdfRepo>();
            services.AddTransient<IPdfFileRepo, PdfFileRepo>();
            services.AddTransient<IPdfUrlRepo, PdfUrlRepo>();
            services.AddTransient<IMenuModuleRepo, MenuModuleRepo>();
            services.AddTransient<IItemCategoryRepo, ItemCategoryRepo>();
            services.AddTransient<IBindingRepo, BindingRepo>();
            services.AddTransient<IBindingVm, BindingVm>();
            services.AddTransient<IHomeIndex, HomeRepo>();
            services.AddTransient<ITestpaperUploadRepo, TestpaperUploadRepo>();

            //testpaper proj.
            services.AddTransient<IColumnDataRepo, ColumnDataRepo>();
            services.AddTransient<IPropRepo, PropsRepo>();

        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            DummyData.DataInit();
            var datas = DummyData.Modules;
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                foreach (var item in datas)
                {
                    if (!context.menu_modules.Any(a => a.title.Equals(item.title)))
                    {
                        context.menu_modules.Add(item);
                    }
                }

                context.SaveChanges();



            }
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            InitializeDatabase(app);
            //app.UseCors("myCors");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{pg?}");
            }
            );
        }
    }
}