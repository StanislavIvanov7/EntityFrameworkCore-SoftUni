﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(10)]
        [MinLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }
        public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
    }
}