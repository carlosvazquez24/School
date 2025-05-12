
using AppServices.AuthServices;
using DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var message = await _authService.RegisterAsync(model);
        return Ok(new { message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = await _authService.LoginAsync(model);
        return Ok(new { token });
    }

    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleModel model)
    {
        var message = await _authService.AddRoleToUserAsync(model);
        return Ok(new { message });
    }
}


