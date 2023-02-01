﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class AuthModel
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public decimal expires_in { get; set; }
        public string refresh_token { get; set; }
    }

    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public BaseUserModel UserModel { get; set; }
        public string TokenString { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<BaseRoleModel> ListRole { get; set; }

        public MobileAppSettings AppSettings { get; set; }
    }

    public class MobileAppSettings
    {
        public MobileAppSettings()
        {
            this.ProductListStyle = "style1";
        }

        public string[] ReloadOrderListAfterOrderActions { get; set; }
        public string[] ReloadMenuAfterOrderActions { get; set; }
        public string ProductListStyle { get; set; }
    }
}
