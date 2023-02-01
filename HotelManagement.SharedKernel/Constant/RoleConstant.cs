using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.SharedKernel
{
    public class RoleConstants
    {
        public static Guid AdmRoleId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid ManagerRoleId => new Guid("00000000-0000-0000-0000-000000000002");
        public static Guid StaffRoleId => new Guid("00000000-0000-0000-0000-000000000003");
        public static Guid NormalUserRoleId => new Guid("a0aba7cc-4fe2-451d-8e6a-f285b5137b1b");
        public const string NormalUser = "USER";
        public const string StaffRoleCode = "STAFF";
        public const string ManagerRoleCode = "MANAGER";
        public const string AdminRoleCode = "ADMIN";
        public const int NormalUserLevel = 1;
        public const int StaffUserLevel = 4;
        public const int ManagerUserLevel = 5;
        public const int AdminLevel = 10;
    }
    public class RightConstants
    {
        public static Guid AccessAppId => new Guid("00000000-0000-0000-0000-000000000001");
        public static string AccessAppCode = "TRUY_CAP_HE_THONG";
        public static Guid DefaultAppId => new Guid("00000000-0000-0000-0000-000000000002");
        public static string DefaultAppCode = "TRUY_CAP_MAC_DINH";
        public static string VIEW = "VIEW";
    }
    //public class NavigationConstants
    //{
    //    // QUẢN TRỊ HỆ THỐNG
    //    public static Guid SystemNav => new Guid("00000000-0000-0000-0000-000000000001");

    //    // Quản lý nhóm
    //    public static Guid RoleNav => new Guid("00000000-0000-0000-0000-000000000011");

    //    // Quản lý quyền
    //    public static Guid RightNav => new Guid("00000000-0000-0000-0000-000000000021");

    //    // Quản lý người dùng
    //    public static Guid UserNav => new Guid("00000000-0000-0000-0000-000000000031");

    //    // PHÂN QUYỀN
    //    public static Guid PermissionNav => new Guid("00000000-0000-0000-0000-000000000002");

    //    // Phân quyền điều hướng
    //    public static Guid NavNav => new Guid("00000000-0000-0000-0000-000000000012");

    //    // Phân quyền người dùng
    //    public static Guid RMUNav => new Guid("00000000-0000-0000-0000-000000000022");
    //}
    public class AppConstants
    {
        public static string EnvironmentName = "production";
        public static Guid HO_APP => new Guid("00000000-0000-0000-0000-000000000001");
    }

    public class UserConstants
    {
        public static Guid AdministratorUserId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid GuestUserId => new Guid("00000000-0000-0000-0000-000000000002");
        public static Guid ManagerUserId => new Guid("00000000-0000-0000-0000-000000000003");
        public static Guid StaffUserId => new Guid("00000000-0000-0000-0000-000000000004");
    }
}
