using Entities.Classes;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Courses
{
    public class Course
    {


        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        public string Name { get; set; }  = string.Empty;


        public string? Description { get; set; }


        [Required]
        public int Credits { get; set; }



        public ICollection<Class> Classes { get; set; }


    }


}
