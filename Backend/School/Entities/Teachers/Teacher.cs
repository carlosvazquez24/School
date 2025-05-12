using Entities.Classes;
using Entities.Courses;
using Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Teachers
{
    public class Teacher
    {

        public int Id {  get; set; }


        [Required]
        [StringLength(50)]

        public string FirstName { get; set; } = string.Empty;


        [Required]
        [StringLength(50)]

        public string LastName { get; set; } = string.Empty;


        [Required]
        [StringLength(50)]

        public string Email { get; set; } = string.Empty;


        [StringLength(50)]

        public string? Phone { get; set; }


        [Required]

        public DateTime HireDate { get; set; }

        [Required]
        public int UserId { get; set; }

        public ApplicationUser? User { get; set; }



    }
}
