using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("Room")]
    public class Room : BaseTableDefault
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public int? Capacity { get; set; }
        public int? Size { get; set; }
        public bool Breakfast { get; set; }
        public bool Pet { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Description { get; set; }
        public Guid? RoomCategoryId { get; set; }
        [ForeignKey("RoomCategoryId")]
        public RoomCategory? RoomCategory { get; set; }
    }
}
