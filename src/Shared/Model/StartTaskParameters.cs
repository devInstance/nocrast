using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class UpdateTaskParameters
    {
        [Required]
        public TaskItem Task { get; set; }

        [Required]
        public TimeLogItem Log { get; set; }
    }
}
