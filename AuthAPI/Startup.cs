using System;
using AuthAPI.Extensions;
using AuthAPI.Models;
using AuthAPI.Services;
using AuthAPI.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PertixCore.Helpers;

namespace AuthAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();

            services.AddControllers();

            services.AddDbContext<AuthDataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PertixCoreDB")));
            services.AddAutoMapper(typeof(Startup));
            services.AddIdentity<User, Role>( options => {
                    options.SignIn.RequireConfirmedAccount = true;
                    
                })
                .AddEntityFrameworkStores<AuthDataContext>()
                .AddDefaultTokenProviders();

            services.AddAuth(jwtSettings);

            services.AddSwagger(jwtSettings);

            services.AddScoped<IJwtService, JwtService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuth();

            app.UseSwag();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }

        private void ConfigureAuth(IApplicationBuilder app)
        {
            throw new NotImplementedException();
        }
    }
}
