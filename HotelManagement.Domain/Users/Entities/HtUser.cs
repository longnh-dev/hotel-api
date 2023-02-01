using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Domain
{ 
    [Table("ht_User")]
    public class HtUser : BaseTableDefault
    {
        public HtUser()
        {
            UserMapRole = new HashSet<UserMapRole>();
        }
        [Key]
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        [StringLength(256)]
        public string? Email { get; set; }

        [StringLength(1024)]
        public string? AvatarUrl { get; set; }

        [StringLength(1024)]
        public string? Password { get; set; }

        [StringLength(1024)]
        public string? PasswordSalt { get; set; }
        public string? UpdateLog { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public int Level { get; set; }
        public int Type { get; set; }
        public string? ResetPasswordToken { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActiveDate { get; set; }
        [StringLength(30)]
        public string? EmailVerifyToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public ICollection<UserMapRole> UserMapRole { get; set; }
    }
}
