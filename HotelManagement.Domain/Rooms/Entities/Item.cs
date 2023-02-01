using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("Item")]
    public class Item : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
        public string? Type { get; set; }
        public Guid ItemStorageId { get; set; }
        [ForeignKey("ItemStorageId")]
        public ItemStorage? ItemStorage { get; set; }
    }
}
