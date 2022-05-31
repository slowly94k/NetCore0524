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
            //Common.cs�� ȣ�� 13.
            //�⺻���� ���� �������� �Ϻ�ȣȭ�� ���Ǵ� ���� CngCbc�� ���
            Common.SetDataProtection(services, @"C:\hwfile\vs2019\z0516\NetCore\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

            //������ ������ ����ϱ� ���ؼ� ���񽺷� ���
            //IUser������, UserService ���빰
            //IUser �������̽��� UserService Ŭ���� �ν��Ͻ� ����
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddHttpContextAccessor();

            //DBFirst DB����������
            services.AddDbContext<DBFirstDbContext>(options =>
                options.UseSqlServer(connectionString: Configuration.GetConnectionString(name: "DBFirstDBConnection")));

            // .Net Core 2.1�� AddMvc()���� ������ ���� �޼������ �����. 
            services.AddControllersWithViews();


            //�ſ������� ���α��� (2���� ���)14
            services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        //���ٹ��� ������ ����
                        options.AccessDeniedPath = "/Membership/Forbidden";
                        //�α��ΰ�� ����
                        options.LoginPath = "/Membership/Login";
                    });

            services.AddAuthorization();

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
           app.UseSession(), app.UseEndpoints()
           �̷��� 5���� �޼���� �ݵ�� ������ ���Ѿ� �ùٷ� �۵���.
           */

            // �Ʒ��� app.UseEndpoints()�޼��带 ����ð� �����ϱ� ���� ����.
            app.UseRouting();

            //�ſ�������(14. )
            app.UseAuthentication();

            // ������ �����ϱ� ���� �޼��尡 �߰���.
            app.UseAuthorization();

            // .Net Core 2.1�� UseMvc()���� ������ ���� �޼������ �����. 
            app.UseEndpoints(endpoints =>
            {
                // .Net Core 2.1�� UseMvc()���� ������ ���� �޼������ �����.
                endpoints.MapControllerRoute(
                    name: "default",
                    // .Net Core 2.1�� template���� ������ ���� �Ķ���͸��� �����.
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
