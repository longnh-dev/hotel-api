using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("ItemStorage")]
    public class ItemStorage : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Quantity { get; set; }
        public int? AvailableCount { get; set; }
        public int? UnavailableCount { get; set; }
        public Guid CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public HtUser? User { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
