using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HotelManagement.Domain
{
    [Table("EmailVerification")]
    public class EmailVerification : BaseTableDefault
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string? Email { get; set; }

        [DefaultValue(false)]
        public bool Verified { get; set; }
        public DateTime? ValidDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? VerifyToken { get; set; }
        public string? Status { get; set; }
    }
}
