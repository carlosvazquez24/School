using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppServices.Classes;
using DTOs.Classes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly IClassAppService _appService;

    public ClassController(IClassAppService appService)
    {
        _appService = appService;
    }

    [HttpPost("CreateManyClasses")]

    public async Task<IActionResult> CreateManyClasses()
    {
        await _appService.CreateManyClasses();

        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _appService.GetAllClassesAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _appService.GetClassAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Create(ClassCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
       ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized("ID de usuario inválido o no autenticado");
        }



        var created = await _appService.CreateClassAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Delete(int id)
    {
        await _appService.DeleteClassAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Patch(int id, JsonPatchDocument<ClassPatchDto> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("El documento de parche es nulo.");
        }

        // Validar que todas las operaciones sean del tipo permitido (solo Replace en este ejemplo)
        foreach (var op in patchDoc.Operations)
        {
            if (op.OperationType != OperationType.Replace)
            {
                ModelState.AddModelError(op.path, $"La operación '{op.op}' en la ruta '{op.path}' no está permitida.");
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _appService.PatchClassAsync(id, patchDoc);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }
}
