using Entities.Courses;
using Entities.Enum;
using Entities.Teachers;
using Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace DTOs.Classes
{
    public class ClassCreateDto
    {

        [Required]
        [StringLength(50)]
        public string Schedule { get; set; } = string.Empty;


        [Required]
        [StringLength(50)]

        public string Room { get; set; } = string.Empty;


        [Required]
        public EnumPeriod EnumPeriod { get; set; }


        [Required]
        [Range(1, int.MaxValue)]

        public int CourseId { get; set; }

    }

    public class ClassPatchDto
    {
        [Required]
        [StringLength(50)]
        public string Schedule { get; set; } = string.Empty;


        [Required]
        [StringLength(50)]

        public string Room { get; set; } = string.Empty;


        [Required]
        public EnumPeriod EnumPeriod { get; set; }


        [Required]
        [Range(1, int.MaxValue)]

        public int CourseId { get; set; }

    }

    public class ClassResponseDto
    {

        public int Id { get; set; }

        public string Schedule { get; set; } = string.Empty;


        public string Room { get; set; } = string.Empty;

        public EnumPeriod EnumPeriod { get; set; }


        public int CourseId { get; set; }

        public int TeacherId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string TeacherName { get; set; }= string.Empty;

    }
}
