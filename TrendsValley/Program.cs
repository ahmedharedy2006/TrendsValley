using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using TrendsValley.DataAccess.Data;
using TrendsValley.DataAccess.Repository;
using TrendsValley.DataAccess.Repository.Interfaces;
using TrendsValley.Models.Models;
using TrendsValley.Services;
using TrendsValley.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = builder.Configuration.GetSection("Microsoft:ClientId").Value;
    microsoftOptions.ClientSecret = builder.Configuration.GetSection("Microsoft:ClientSecret").Value;
});
builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
{
    facebookOptions.ClientId = builder.Configuration.GetSection("Facebook:ClientId").Value;
    facebookOptions.ClientSecret = builder.Configuration.GetSection("Facebook:ClientSecret").Value;
});
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration.GetSection("Google:ClientId").Value;
    googleOptions.ClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Value;
});

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true; 
    options.Password.RequireDigit = false; 
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
    options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultProvider;
    options.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultProvider;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultProvider;
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Admin/Auth/SignIn";  // Identity Login Path
    options.AccessDeniedPath = "/Customer/Home/Index";
})
.AddCookie("MyCustomAuth", options =>
{
    options.LoginPath = "/Customer/Auth/SignIn"; // Custom login path
    options.AccessDeniedPath = "/Customer/Home/Index";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewCategory", policy => policy.RequireClaim("View Category"));
    options.AddPolicy("AddCategory", policy => policy.RequireClaim("Add Category"));
    options.AddPolicy("EditCategory", policy => policy.RequireClaim("Edit Category"));
    options.AddPolicy("DeleteCategory", policy => policy.RequireClaim("Delete Category"));

    options.AddPolicy("ViewBrand", policy => policy.RequireClaim("View Brand"));
    options.AddPolicy("AddBrand", policy => policy.RequireClaim("Add Brand"));
    options.AddPolicy("EditBrand", policy => policy.RequireClaim("Edit Brand"));
    options.AddPolicy("DeleteBrand", policy => policy.RequireClaim("Delete Brand"));

    options.AddPolicy("ViewCity", policy => policy.RequireClaim("View City"));
    options.AddPolicy("AddCity", policy => policy.RequireClaim("Add City"));
    options.AddPolicy("EditCity", policy => policy.RequireClaim("Edit City"));
    options.AddPolicy("DeleteCity", policy => policy.RequireClaim("Delete City"));

    options.AddPolicy("ViewAdminUser", policy => policy.RequireClaim("View Admin"));
    options.AddPolicy("AddAdminUser", policy => policy.RequireClaim("Add Admin"));
    options.AddPolicy("EditAdminUser", policy => policy.RequireClaim("Edit Admin"));
    options.AddPolicy("DeleteAdminUser", policy => policy.RequireClaim("Delete Admin"));

    options.AddPolicy("ViewCustomer", policy => policy.RequireClaim("View User"));
    options.AddPolicy("AddCustomer", policy => policy.RequireClaim("Add User"));
    options.AddPolicy("EditCustomer", policy => policy.RequireClaim("Edit User"));
    options.AddPolicy("DeleteCustomer", policy => policy.RequireClaim("Delete User"));

    options.AddPolicy("ViewState", policy => policy.RequireClaim("View State"));
    options.AddPolicy("AddState", policy => policy.RequireClaim("Add State"));
    options.AddPolicy("EditState", policy => policy.RequireClaim("Edit State"));
    options.AddPolicy("DeleteState", policy => policy.RequireClaim("Delete State"));

    options.AddPolicy("ViewProduct", policy => policy.RequireClaim("View Product"));
    options.AddPolicy("AddProduct", policy => policy.RequireClaim("Add Product"));
    options.AddPolicy("EditProduct", policy => policy.RequireClaim("Edit Product"));
    options.AddPolicy("DeleteProduct", policy => policy.RequireClaim("Delete Product"));


});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IClaimsTransformation, RoleClaimsTransformation>();


builder.Services.AddScoped<IStateRepo, StateRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICityRepo, CityRepo>();
builder.Services.AddScoped<IBrandRepo, BrandRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShoppingCartRepo, ShoppingCartRepo>();
builder.Services.AddScoped<IOrderHeaderRepo, OrderHeaderRepo>();
builder.Services.AddScoped<IOrderDetailsRepo, OrderDetailsRepo>();
builder.Services.AddScoped<IAppUserRepo, AppUserRepo>();

builder.Services.AddScoped<UserService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();
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
        adminUser = new AppUser {
            UserName = adminEmail,
            Email = adminEmail, 
            EmailConfirmed = true,
            Fname = "Admin",
            Lname = "Admin",
            CityId = 2, 
            StateId = 1 };
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
