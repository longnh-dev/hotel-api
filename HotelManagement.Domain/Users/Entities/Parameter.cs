using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("Parameter")]
    public class Parameter : BaseTable<Parameter>
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(64)]
        public string? Name { get; set; }

        [StringLength(1024)]
        public string? Description { get; set; }

        public string? Value { get; set; }

        public bool IsSystem { get; set; }

        [StringLength(64)]
        public string? GroupCode { get; set; }
    }
}
