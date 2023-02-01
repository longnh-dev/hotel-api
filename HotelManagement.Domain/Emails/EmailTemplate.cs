using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("EmailTemplate")]
    public class EmailTemplate : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(128)]
        public String? Name { get; set; }

        [MaxLength(64)]
        public string? Code { get; set; }

        [MaxLength(256)]
        public string? Description { get; set; }
    }
}
