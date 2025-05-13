using AppServices;
using AppServices.AuthServices;
using AppServices.Classes;
using AppServices.Courses;
using DataAccess;
using DataAccess.Repository;
using Entities.Classes;
using Entities.Courses;
using Entities.Teachers;
using Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YourNamespace.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Configurar base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>() // Solo ApplicationUser es personalizado
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configura JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

//Inyección de dependencias de Repository

builder.Services.AddScoped<IRepository<Class>, Repository<Class>>();
builder.Services.AddScoped<IRepository<Course>, Repository<Course>>();
builder.Services.AddScoped<IRepository<Teacher>, Repository<Teacher>>();

// Configurar CORS
var corsPolicy = "AllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//Inyección de dependencias de servicios
builder.Services.AddScoped<IClassAppService, ClassAppService>();
builder.Services.AddScoped<ICourseAppService, CourseAppService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(corsPolicy);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{

    await InsertUsersAndRoles(scope.ServiceProvider);

}


app.Run("http://0.0.0.0:80");

async Task InsertUsersAndRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1) Roles necesarios
    string[] roleNames = { "Teacher", "Student", "User", "Admin" };
    foreach (var roleName in roleNames)
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));

    // 2) Usuario administrador 
    const string adminEmail = "carlos@gmail.com";
    const string adminPassword = "Admin123!";
    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new ApplicationUser
        {
            UserName = "carloseliam",
            Email = adminEmail,
            EmailConfirmed = true,
            UserType = "Teacher"
        };
        var result = await userManager.CreateAsync(user, adminPassword);
        if (!result.Succeeded)
            throw new Exception("Error creando usuario admin: " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // 3) Asegurar roles del usuario
    if (!await userManager.IsInRoleAsync(user, "Teacher"))
        await userManager.AddToRoleAsync(user, "Teacher");
    if (!await userManager.IsInRoleAsync(user, "Admin"))
        await userManager.AddToRoleAsync(user, "Admin");

    // 4) Crear o comprobar el registro Teacher
    var teacher = await context.Teachers
        .FirstOrDefaultAsync(t => t.UserId == user.Id);

    if (teacher == null)
    {
        teacher = new Teacher
        {
            FirstName = "Carlos",
            LastName = "Elíam",
            Email = user.Email,
            HireDate = DateTime.UtcNow,
            UserId = user.Id
        };
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
    }
}




