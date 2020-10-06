using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class TagItem : ModelItem
    {
        [Required]
        public string Name { get; set; }

        public int Color { get; set; }

        public int TasksCount { get; set; }

        public long TotalTimeSpent { get; set; }
    }
}
