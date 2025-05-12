using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppServices.Courses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Dtos.Course;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseAppService _appService;

    public CourseController(ICourseAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _appService.GetAllCoursesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _appService.GetCourseAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Create(CourseCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _appService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Delete(int id)
    {
        await _appService.DeleteCourseAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Teacher")]

    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<CoursePatchDto> patchDoc)
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

        var updated = await _appService.PatchCourseAsync(id, patchDoc);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }
}
