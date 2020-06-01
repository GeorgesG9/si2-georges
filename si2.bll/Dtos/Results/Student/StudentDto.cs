using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace si2.bll.Dtos.Results.Student
{
    public class StudentDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]

        public string Code { get; set; }

       /*[Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; } */

        public byte[] RowVersion { get; set; }

        [Required]
        public string Name { get; set; }

        public override bool Equals(Object obj) => Equals(obj as StudentDto);

        public bool Equals(StudentDto obj)
        {
            return (this.Id == obj.Id
                && string.Equals(this.Code, obj.Code, StringComparison.OrdinalIgnoreCase)
                && string.Equals(this.Name, obj.Name, StringComparison.OrdinalIgnoreCase)
                && this.RowVersion.SequenceEqual(obj.RowVersion));
        }
    }
}
