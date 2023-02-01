using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using static HotelManagement.SharedKernel.Helper;

namespace HotelManagement.SharedKernel
{
    public class AllowAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _roles = null;

        public AllowAttribute(string roles)
        {
            _roles = roles;
        }

        public AllowAttribute(string role1, string role2)
        {
            _roles = role1;
            if (!string.IsNullOrEmpty(role2))
                _roles = _roles + "," + role2;
        }

        public AllowAttribute(string role1, string role2, string role3)
        {
            _roles = role1;
            if (!string.IsNullOrEmpty(role2))
                _roles = _roles + "," + role2;

            if (!string.IsNullOrEmpty(role3))
                _roles = _roles + "," + role3;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            RequestUser currentClaim = Helper.GetRequestInfo(context.HttpContext.Request);

            var currentRole = HotelConstant.RoleLevelDict.Where(x => x.Value == currentClaim.Level)
                .Select(x => x.Key)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(currentRole))
            {
                var listRole = _roles.Split(',');
                if (listRole.Contains(currentRole))
                {
                    return;
                }
            };

            if (string.IsNullOrEmpty(_roles))
            {
                return;
            }

            if (currentClaim.Level == 100)
            {
                return;
            }
            var result = new Response(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập chức năng này.");

            context.Result = Helper.TransformData(result);
        }
    }
}
