using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Domain
{
    [Table("Booking")]
    public class Booking : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int NoOfPerson { get; set; }
        public string? Status { get; set; }
        public string? CititzenIdentification { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public HtUser? Customer { get; set; }
        public Guid? StaffId { get; set; }
        [ForeignKey("StaffId")]
        public HtUser? Staff { get; set; }
        public Guid? RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
    }
}
