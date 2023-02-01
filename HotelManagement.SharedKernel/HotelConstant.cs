using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.SharedKernel
{
    public class HotelConstant
    {
        public static class ClaimConstants
        {
            public static string USER_ID = "x-userId";
            public static string APP_ID = "x-appId";
            public static string USER_NAME = "x-userName";
            public static string FULL_NAME = "x-fullName";
            public static string ROLES = "x-roles";
            public static string RIGHTS = "x-roles";
            public static string LEVEL = "x-level";
            public static string ISSUED_AT = "x-iat";
            public static string EXPIRES_AT = "x-exp";
            public static string CHANNEL = "x-channel";
            public static string PHONE = "x-phone";
        }
        public static Dictionary<string, int> RoleLevelDict = new Dictionary<string, int>()
        {
            { RoleConstants.AdminRoleCode, RoleConstants.AdminLevel },
            { RoleConstants.NormalUser, RoleConstants.NormalUserLevel}
        };
    }
    public static class RoomStatusConstant
    {
        public const string AvailableRoom = "AVAILABLE";
        public const string UnavailableRoom = "UNAVAILABLE";
    }
    public static class ItemStatusConstant
    {
        public const string AvailableRoom = "AVAILABLE";
        public const string UnavailableRoom = "UNAVAILABLE";
    }
    public static class BookingStatusConstant
    {
        public const string Cancel = "CANCELED";
        public const string Operational = "OPERATIONAL";
        public const string Completed = "COMPLETED";
        public const string Confirmed = "CONFIRMED";
    }

    public static class PaymentConstant
    {
        public const string Cash = "CASH";
        public const string DebitCard = "DEBIT_CARDS";
        public const string CreditCard = "CREDIT_CARDS";
        public const string InternetBanking =  "INTERNETBANKING";
    }
    public static class PaymentStatusConstant
    {
        public const string SUCCESS = "SUCCESS";
        public const string UNSUCCESS = "UNSUCCESS";
        
    }
}
