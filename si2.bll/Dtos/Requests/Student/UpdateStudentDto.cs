using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace si2.bll.Dtos.Requests.Student
{
    public class UpdateStudentDto 
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        public byte[] RowVersion { get; set; }
    }
}
