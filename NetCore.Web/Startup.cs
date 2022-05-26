using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Common.cs를 호출 13.
            //기본적인 것을 가져오고 암복호화에 사용되는 것은 CngCbc를 사용
            Common.SetDataProtection(services, @"C:\hwfile\vs2019\z0516\NetCore\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

            //의존성 주입을 사용하기 위해서 서비스로 등록
            //IUser껍데기, UserService 내용물
            //IUser 인터페이스에 UserService 클래스 인스턴스 주입
            services.AddScoped<IUser, UserService>();


            services.AddHttpContextAccessor();


            //DBFirst DB접속정보만
            services.AddDbContext<DBFirstDbContext>(options =>
                options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "DBFirstDBConnection")));

            //MVC 패턴을 사용하기 위해서 서비스로 등록
            services.AddMvc();

            //신원보증과 승인권한 (2가지 등록)14
            services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Membership/Forbidden";
                    options.LoginPath = "/Membership/Login";
                });

            services.AddAuthorization();

            //services.AddControllersWithViews(); 원래 있던거
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            /*
             app.UseRouting(), app.UseAuthentication(), app.UseAuthorization(),
            app.UseSession(), app.UseEndpoints() 순서대로 작성 해야한다.
             */

            // 아래의 app.UseEndpoints()메서드를 라우팅과 연결하기 위해 사용됨.
            app.UseRouting();
            
            //신원보증만 14
            app.UseAuthentication();

            // 권한을 승인하기 위해 메서드가 추가됨.
            app.UseAuthorization();

            ////강의내용
            //세션 지정
            //System.InvalidOperationException:
            //'Session has not been configured for this application or request.'
            //app.UseSession();

            // .Net Core 2.1의 UseMvc()에서 다음과 같이 메서드명이 변경됨. 
            app.UseEndpoints(endpoints =>
            {
                // .Net Core 2.1의 UseMvc()에서 다음과 같이 메서드명이 변경됨.
                endpoints.MapControllerRoute(
                    name: "default",
                    // .Net Core 2.1의 template에서 다음과 같이 파라미터명이 변경됨.
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
