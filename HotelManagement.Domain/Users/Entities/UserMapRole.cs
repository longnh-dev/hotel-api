using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("UserMapRole")]
    public class UserMapRole : BaseTable<UserMapRole>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(Order = 1), ForeignKey("User")]
        public Guid UserId { get; set; }

        [Column(Order = 2), ForeignKey("Role")]
        public Guid RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual HtUser User { get; set; }

        [MaxLength(256)]
        public string? Username { get; set; }

        [MaxLength(256)]
        public string? Rolename { get; set; }
    }
}
