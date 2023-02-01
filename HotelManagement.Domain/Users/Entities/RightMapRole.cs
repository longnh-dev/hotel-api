using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("RightMapRole")]
    public class RightMapRole : BaseTable<RightMapRole>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set;}
        [Column(Order = 1), ForeignKey("Role")]
        public Guid RoleId { get; set; }
        [Column(Order = 2), ForeignKey("Right")]
        public Guid RightId { get; set; }

        public virtual Right Right { get; set; }
        public virtual Role Role { get; set; }
    }
}
