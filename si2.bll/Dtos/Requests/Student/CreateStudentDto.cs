using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace si2.bll.Dtos.Requests.Student
{
    public class CreateStudentDto
    {
        [Required]
        public virtual string Code { get; set; }

        [Required]
        public virtual string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }

        public override bool Equals(Object obj) => Equals(obj as CreateStudentDto);

        public bool Equals(CreateStudentDto obj)
        {
            return (
                string.Equals(this.Code, obj.Code, StringComparison.OrdinalIgnoreCase)
                && string.Equals(this.LastName, obj.LastName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(this.FirstName, obj.FirstName, StringComparison.OrdinalIgnoreCase)

                );
        }
    }
}
