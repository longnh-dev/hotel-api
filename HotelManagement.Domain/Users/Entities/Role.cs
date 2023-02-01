using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("ht_Role")]
    public class Role : BaseTableDefault
    {
        public Role()
        {
            UserMapRole = new HashSet<UserMapRole>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsSystem { get; set; }
        public ICollection<UserMapRole> UserMapRole { get; set; }
    }
}
