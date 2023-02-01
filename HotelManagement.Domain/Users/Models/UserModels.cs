using HotelManagement.SharedKernel;
using System.Text.Json.Serialization;

namespace HotelManagement.Domain
{
    public class BaseUserListModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string PhoneNumber { get; set; }
       
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        
        [JsonIgnore]
        public string TimeZone { get; set; }
    }
    public class BaseUserModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? IdentificationNumber { get; set; }

        public int? Type { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string PasswordSalt { get; set; }

        public DateTime? Birthdate { get; set; }
        public DateTime LastActivityDate { get; set; }

        public int Level { get; set; }
        public bool IsLockedOut { get; set; }
    }

    public class UserModel : BaseUserModel
    {
        public string FullTextAddress { get; internal set; }

        //public DateTime? LocalTime
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(this.TimeZone))
        //            return DateTime.UtcNow;
        //        else
        //        {
        //            var timeZone = TimezoneCollection.Instance.AllTimeZones.Find(x => x.Abbr.ToLower() == this.TimeZone.ToLower());
        //            if (timeZone != null)
        //                return DateTime.UtcNow.AddHours(timeZone.Offset);
        //            else return DateTime.UtcNow;
        //        }
        //    }
        //}
    }

    public class UserDetailModel : UserModel
    {
        public List<BaseRightModelOfUser> ListRight { get; set; }
        public List<BaseRoleModel> ListRole { get; set; }
    }

    public class UserQueryModel : PaginationRequest
    {
        public string Team { get; set; }
        public bool? IsLockedOut { get; set; }
        public int? Level { get; set; }
        public int? Type { get; set; }
        public string UserName { get; set; }
        public List<Guid> ListRoleId { get; set; }
        public int? ToPoint { get; set; }
    }

    public class UserCreateModel
    {
        /// <summary>
        /// preset
        /// </summary>
        public Guid Id { get; set; }

        //public string UserName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public int? Type { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Birthdate { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền thêm
        /// </summary>
        public List<Guid> ListAddRightId { get; set; } = new List<Guid>();

        /// <summary>
        /// Danh sách nhóm người dùng thêm
        /// </summary>
        public List<Guid> ListAddRoleId { get; set; } = new List<Guid>();

        public string RegisterCode { get; set; }

        [JsonIgnore]
        public Guid? RegisterCodeId { get; set; }
    }

    public class UserRegisterRequestModel
    {
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public string? RegisterCode { get; set; }

        [JsonIgnore]
        public List<Guid>? ListAddRightId { get; set; } = new List<Guid>();

        [JsonIgnore]
        public List<Guid>? ListAddRoleId { get; set; } = new List<Guid>();

        [JsonIgnore]
        public Guid? RegisterCodeId { get; set; }

    }

    public class UserUpdateModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? Type { get; set; }
        public string Password { get; set; }
        public DateTime? Birthdate { get; set; }
        public bool? Locked { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền thêm
        /// </summary>
        public List<Guid> ListAddRightId { get; set; }

        /// <summary>
        /// Danh sách nhóm người dùng thêm
        /// </summary>
        public List<Guid> ListAddRoleId { get; set; }

        //Plus
        /// <summary>
        /// Danh sách quyền xóa
        /// </summary>
        public List<Guid> ListDeleteRightId { get; set; }

        /// <summary>
        /// Danh sách nhóm người dùng xóa
        /// </summary>
        public List<Guid> ListDeleteRoleId { get; set; }
        public Guid? AddressId { get; set; }
        public string PlainTextPwd { get; set; }
    }

    public class UserInfoUpdateModel
    {
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string? AvatarUrl { get; set; }

    }

    public class CapUserLevelUpdateModel
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
    }

    public class CapUserInfoUpdateModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Level { get; set; }
        public bool? IsLockedOut { get; set; }
    }

    public class CapUserLockedUpdateModel
    {
        public Guid Id { get; set; }
        public bool Locked { get; set; }
    }

    public class UserResetPasswordModel
    {
        public string? ResetPasswordToken { get; set; }
        public string? Password { get; set; }
    }

    public class UserVerifyEmailModel
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
    }
}
