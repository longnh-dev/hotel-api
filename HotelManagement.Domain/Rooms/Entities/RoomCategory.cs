using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("RoomCategory ")]
    public class RoomCategory : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public decimal Price { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
