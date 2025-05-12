using DataAccess;
using DTOs.Auth;
using Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new InvalidOperationException("El correo electrónico ya está registrado.");

            // Iniciar transacción manual
            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                    throw new InvalidOperationException("No se pudo asignar el rol 'User' al usuario.");

                // Commit de la transacción: solo si todo fue exitoso
                await transaction.CommitAsync();

                return "Usuario creado exitosamente";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<string> LoginAsync(LoginModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "El modelo de login no puede ser nulo.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas");
            }

            // Crea los claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Agrega los roles del usuario a los claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Configuración JWT
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

            // Crear el token JWT
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.UtcNow.AddDays(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public async Task<string> AddRoleToUserAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return "Rol agregado al usuario correctamente";
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        public async Task<bool> AccountExists(string email)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(email);

            //Si la cuenta no existe, devolver false
            if (existingUser != null)
                return false;
            else
            {
                return true;
            }

        }
    }


}

