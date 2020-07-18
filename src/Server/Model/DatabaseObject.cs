using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class DatabaseObject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
