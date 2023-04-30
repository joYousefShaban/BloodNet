using BloodNet.Controllers.Account;
using BloodNet.Models;
using BloodNet.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add serices to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        //Session
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(1);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddControllersWithViews();
        //Add DBContext
        services.AddDbContext<BloodNetContext>(options =>
        options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

        services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<Role>()
            .AddEntityFrameworkStores<BloodNetContext>().AddDefaultTokenProviders();
        services.AddScoped<RegisterController>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? string.Empty))
                };
            });
        //Razor Pages
        services.AddRazorPages();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSession();
        //Middle ware process request
        app.Use(async (context, next) =>
        {
            var token = context.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token)) //if in session has a token
            {
                //Get session and append to request
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            await next();
        });

        //Middleware process http status code
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;

            if (!request.Path.Value.Contains("/api")) //Only apply process for API process
            {
                if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    response.Redirect("/Identity/Account/Login");
                }
            }
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        //this is the default page for the website
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=AdminHome}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}