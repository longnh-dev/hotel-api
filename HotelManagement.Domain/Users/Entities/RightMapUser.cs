using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("RightMapUser")]
    public class RightMapUser : BaseTable<RightMapUser>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(Order = 1), ForeignKey("User")]
        public Guid UserId { get; set; }

        [Column(Order = 3), ForeignKey("Right")]
        public Guid RightId { get; set; }

        [StringLength(1024)]
        public string InheritedFromRoles { get; set; }

        public bool Inherited { get; set; }

        public bool Enable { get; set; }

        public virtual Right Right { get; set; }

        public virtual HtUser User { get; set; }
    }
}
