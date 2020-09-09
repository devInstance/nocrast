using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Model
{
    public class InternalDatabaseObject
    {
        [Key]
        public Guid Id { get; set; }
    }
}
