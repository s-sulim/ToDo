using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        //Create default user roles

        var roleManager = services.GetService<RoleManager<IdentityRole>>();

        string defaultRole = "User";
        string adminRole = "Admin";
        string[] roleNames = { defaultRole, adminRole };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        //Create default admin user

        string userName = "admin@todoapp.net", role = adminRole, password = "Yerobota!1";
        var userManager = services.GetService<UserManager<IdentityUser>>();

        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            var newUser = new IdentityUser() { UserName = userName, Email = userName };
            await userManager.CreateAsync(newUser, password);
        }
        user = await userManager.FindByNameAsync(userName);
        userManager.AddToRoleAsync(user, role).Wait();


        // Get all users
        var users = userManager.Users.ToList();
        foreach (var usr in users)
        {
            // Check if the user is in the Admin role
            if (!await userManager.IsInRoleAsync(usr, adminRole))
            {
                // Check if the user is already in the default role
                if (!await userManager.IsInRoleAsync(usr, defaultRole))
                {
                    // Assign default role
                    await userManager.AddToRoleAsync(usr, defaultRole);
                }
            }
        }
    }
    catch (Exception exception)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An error occurred while creating roles");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
