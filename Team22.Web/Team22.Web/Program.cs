using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Data;
using Team22.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Validations.Rules;
using Team22.Web.Helpers;
using Team22.Web.Interfaces;
using Team22.Web.Models;
using Team22.Web.ViewModels;


var builder = WebApplication.CreateBuilder(args);

// ** ADD SERVICES AND CONTROLLERS **
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SponsorService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<PasswordChangeService>();
builder.Services.AddScoped<VerificationService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<Team22Context>().AddDefaultTokenProviders();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<PointService>();
//builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<Team22Context>().AddDefaultTokenProviders();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.Configure<UserOptions>(opt =>
{
    opt.RequireUniqueEmail = true;
});
    builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 5;
    opt.Password.RequiredUniqueChars = 1;
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
// **********************************

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// ** CONFIGURE SWAGGER/OPENAPI **
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(t => t.ToString()));
// *******************************

// ** ADD USER SECRETS/ENV VARS TO APP CONFIG **
builder.Host.ConfigureAppConfiguration((_, config) =>
{
    config.AddUserSecrets<Team22Context>(true);
    config.AddEnvironmentVariables();
});
// *********************************************

// ** CONFIGURE THE DB CONTEXT USING USER SECRETS **
var connectionString = builder.Configuration["Team22ConnectionString"];
builder.Services.AddDbContext<Team22Context>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
// *************************************************

var app = builder.Build();

// ** CONFIGURE HTTP REQUEST PIPELINE **
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// *************************************

// ** APPLY DATABASE MIGRATIONS **
using var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<Team22Context>();
ctx.Database.Migrate();
// *******************************

// ** SEED THE DB **
// if you're having errors with DB when running, comment this block out
using (var newScope = app.Services.CreateScope())
{
    var services = newScope.ServiceProvider;
    
    SeedData.Initialize(services);
}
// **********************************

app.Run();