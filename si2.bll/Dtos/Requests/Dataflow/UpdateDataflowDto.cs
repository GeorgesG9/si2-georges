using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace si2.bll.Dtos.Requests.Dataflow
{
    public class UpdateDataflowDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Tag { get; set; }

        //[Required]
        public byte[] RowVersion { get; set; }
    }
}
