using System.Collections.Generic;
using DTOs.Classes;
using Microsoft.AspNetCore.JsonPatch;


namespace AppServices.Classes
{
    public interface IClassAppService
    {
         Task CreateManyClasses();

         Task<ClassResponseDto> CreateClassAsync(ClassCreateDto dto, int userId);
         Task DeleteClassAsync(int id);
         Task<List<ClassResponseDto>> GetAllClassesAsync();

         Task<ClassResponseDto> GetClassAsync(int id);
         Task<ClassResponseDto?> PatchClassAsync(int id, JsonPatchDocument<ClassPatchDto> patchDoc);
    }
}
