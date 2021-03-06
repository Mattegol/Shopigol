﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopigol.Core.Contracts;
using Shopigol.Core.Models;
using Shopigol.Services;
using Shopigol.WebUI.Data;
using Shopigol.WebUI.Data.Repositories;
using Shopigol.WebUI.Models;
using Shopigol.WebUI.Services;

namespace Shopigol.WebUI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IRepository<Product>, SqlRepository<Product>>();
            services.AddScoped<IRepository<ProductCategory>, SqlRepository<ProductCategory>>();
            services.AddScoped<IRepository<Basket>, SqlRepository<Basket>>();
            services.AddScoped<IRepository<BasketItem>, SqlRepository<BasketItem>>();
            services.AddScoped<IRepository<Customer>, SqlRepository<Customer>>();
            services.AddScoped<IRepository<Order>, SqlRepository<Order>>();
            services.AddScoped<IRepository<OrderItem>, SqlRepository<OrderItem>>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
