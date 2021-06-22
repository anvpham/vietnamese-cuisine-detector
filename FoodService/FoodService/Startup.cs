using System;
using FoodService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace FoodService
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
            services.AddAuthorization(options => {
                options.AddPolicy("Users", policyBuilder => {
                    policyBuilder.RequireClaim("Identity", "User");
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtSecretKey"])),

                };
            });

            services.AddHttpClient("googlePlaces", c =>
            {
                c.BaseAddress = new Uri(Configuration["GooglePlacesEndpoint"]);
            });
            
            services.AddHttpClient("foodPrediction", c =>
            {
                c.BaseAddress = new Uri(Configuration["FoodPredictionEndpoint"]);
            });
            
            services.AddControllers();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration["RedisEndpoint"]));
            services.AddSingleton<ICacheService, RedisService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Food service API");
                });
                
                endpoints.MapControllers();
            });
        }
    }
}