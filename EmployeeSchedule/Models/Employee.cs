using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeSchedule.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required, EmailAddress, Display(Name = "Email"), Index(IsUnique = true), StringLength(200), Column(TypeName = "varchar")]
        public string Email { get; set; }
    }
}