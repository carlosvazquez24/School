using System.Collections.Generic;
using Dtos.Course;
using Microsoft.AspNetCore.JsonPatch;


namespace AppServices.Courses
{
    public interface ICourseAppService
    {
         Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto dto);
         Task DeleteCourseAsync(int id);
         Task<List<CourseResponseDto>> GetAllCoursesAsync();
         Task<CourseResponseDto> GetCourseAsync(int id);
         Task<CourseResponseDto?> PatchCourseAsync(int id, JsonPatchDocument<CoursePatchDto> patchDoc);
    }
}
