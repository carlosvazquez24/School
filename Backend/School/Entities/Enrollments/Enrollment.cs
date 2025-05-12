using Entities.Classes;
using Entities.Students;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Enrollments
{
    public class Enrollment
    {


        public int Id { get; set; }


        [Column(TypeName = "decimal(10,3)")]

        public decimal? FinalGrade { get; set; }


        public Student? Student { get; set; }

        public int StudentId {  get; set; }


        public Class? Class { get; set; }

        public int ClassId { get; set; }
    }
}
