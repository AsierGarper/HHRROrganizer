using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HHRROrganizer.Models
{
    public class Employees
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Picture { get; set; }
        public DateTime StartDate { get; set; }
        public int GrossSalary { get; set; }
        public int NetSalary { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
