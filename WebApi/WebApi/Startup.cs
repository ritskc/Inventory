using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DAL.DBHelper;
using DAL.IRepository;
using DAL.Repository;
using WebApi.IServices;
using WebApi.Services;
using WebApi.Settings;


namespace WebApi
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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {

                        builder.WithOrigins("http://yellow-chips.com",
                                            "https://yellow-chips.com");
                    });

                options.AddPolicy("AnotherPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://yellow-chips.com",
                            "https://yellow-chips.com")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });

                options.AddPolicy("AllowSubdomain",
    builder =>
    {
        builder.WithOrigins("http://*.yellow-chips.com", "https://*.yellow-chips.com")
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

                options.AddPolicy("AllowSubdomain",
    builder =>
    {
        builder.WithOrigins("http://*.yellow-chips.com")
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });


            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<IISServerOptions>(option =>
            {
                option.AutomaticAuthentication = false;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connSettingsSection = Configuration.GetSection("ConnectionSettings");
            services.Configure<ConnectionSettings>(connSettingsSection);

            var connSettings = connSettingsSection.Get<ConnectionSettings>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>
             {
                 x.RequireHttpsMetadata = false;
                 x.SaveToken = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = false,
                     ValidateAudience = false
                 };
             });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            //add services here            
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISupplierService, SupplierService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IPartService, PartService>();
            services.AddTransient<IPoService, PoService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ISupplierInvoiceService, SupplierInvoiceService>();
            services.AddTransient<IEntityTrackerService, EntityTrackerService>();
            services.AddTransient<IPackingSlipService, PackingSlipService>();

            //add repositories here
            //services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<IPoRepository, PoRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddTransient<ISupplierInvoiceRepository, SupplierInvoiceRepository>();
            services.AddTransient<IEntityTrackerRepository, EntityTrackerRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IPackingSlipRepository, PackingSlipRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();

            //add helpers here
            services.AddScoped<ISqlHelper, SqlHelper>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();            
        }
    }
}
