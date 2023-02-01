using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("RoomBooking")]
    public class RoomBooking
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? BookingId { get; set; }
    }
}
