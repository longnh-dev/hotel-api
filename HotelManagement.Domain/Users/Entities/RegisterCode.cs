using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("RegisterCode")]
    public class RegisterCode : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Status { get; set; }
        public int ExpiredTime { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public string LastmodifiedByUser { get; set; }
        public string CreatedByUser { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}
