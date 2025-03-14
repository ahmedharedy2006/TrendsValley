using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.User.RequireUniqueEmail = true; //  => For unique emails 
    opt.Password.RequireDigit = false; // => Controll RequireCapitale 
    opt.Password.RequireLowercase = false; 
    opt.Password.RequireNonAlphanumeric = false; 
});

builder.Services.AddScoped<IStateRepo, StateRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICityRepo, CityRepo>();
builder.Services.AddScoped<IBrandRepo, BrandRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@example.com";  // Change this
    string adminPassword = "Admin@123";      // Change this

    // Ensure "Admin" role exists
    if (!await roleManager.RoleExistsAsync(SD.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(SD.Admin));
        await roleManager.CreateAsync(new IdentityRole(SD.User));
    }

    // Ensure admin user exists
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true , CityId = 1 , StateId = 1 };
        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("Admin user created successfully!");
        }
        else
        {
            Console.WriteLine("Error creating admin user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}


app.Run();
