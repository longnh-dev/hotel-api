using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    [Table("History")]
    public class History : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? BookingId { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual HtUser? Customer { get; set; }

    }
}
