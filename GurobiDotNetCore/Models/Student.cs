using System;
using System.Collections.Generic;
using System.Text;

namespace GurobiDotNetCore.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<double> Scores { get; set; }
    }
}
