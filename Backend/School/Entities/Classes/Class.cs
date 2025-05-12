using Entities.Courses;
using Entities.Enum;
using Entities.Teachers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Classes
{
    public class Class
    {

        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        public string Schedule {  get; set; } = string.Empty;


        [Required]
        [StringLength(50)]

        public string Room { get; set; } = string.Empty;


        public EnumPeriod EnumPeriod { get; set; }


        public Teacher? Teacher { get; set; }


        public int TeacherId {  get; set; }


        public Course? Course { get; set; }

        public int CourseId { get; set; }

    }
}
