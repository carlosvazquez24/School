using DTOs.Classes;
using Entities.Classes;
using System.ComponentModel.DataAnnotations;

namespace Dtos.Course
{
    public class CourseCreateDto
    {


        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }


        [Required]
        public int Credits { get; set; }
    }

    public class CoursePatchDto
    {

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }


        [Required]
        public int Credits { get; set; }
    }

    public class CourseResponseDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }


        [Required]
        public int Credits { get; set; }

        public ICollection<ClassResponseDto> Classes { get; set; }

    }
}
