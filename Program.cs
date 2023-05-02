using BloodNet.Models.Auth;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration); // My custom startup class.



startup.ConfigureServices(builder.Services); // Add services to the container.

var app = builder.Build();

using (var scope = app.Services.CreateScope())

{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var roles = new[] { "Admin", "Hospital", "Donor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new Role(role));
        }
    }
}

//create admin
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string email = "admin@bloodnet.com";
    string password = "Smurfin111!";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }
}


startup.Configure(app, app.Environment); // Configure the HTTP request pipeline.

app.Run();