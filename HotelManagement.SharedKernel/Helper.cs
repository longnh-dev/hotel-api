using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HotelManagement.SharedKernel.HotelConstant;

namespace HotelManagement.SharedKernel
{
    public static class HotelHelper
    {
        /// <summary>
        ///     Tạo Id quyền riêng
        /// </summary>
        /// <returns></returns>
        public static Guid MakeIndependentPermission()
        {
            return new Guid("00000000-0000-0000-0000-000000000000");
        }

        /// <summary>
        ///     Lấy về danh sách RoleId đã kế thừa từ string
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <returns></returns>
        public static List<Guid> LoadRolesInherited(string currInheritedFromRoles)
        {
            try
            {
                if (string.IsNullOrEmpty(currInheritedFromRoles))
                {
                    var result = new List<Guid>();
                    return result;
                }
                var jsonObject = JsonConvert.DeserializeObject<List<Guid>>(currInheritedFromRoles);

                var newlist = new List<Guid>();

                foreach (var item in jsonObject)
                    if (!newlist.Contains(item))
                        newlist.Add(item);
                jsonObject = jsonObject.GroupBy(role => role)
                    .Select(g => g.First())
                    .ToList();

                return jsonObject;
            }
            catch (Exception)
            {
                var result = new List<Guid>();
                return result;
            }
        }

        /// <summary>
        ///     Sinh ra chuỗi Json kế thừa từ 1 danh sách RoleId lưu vào DB
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string GenRolesInherited(List<Guid> listRoleId)
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(listRoleId);
                return jsonStr;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        ///     Sinh ra chuỗi Json kế thừa từ 1 RoleId lưu vào DB
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GenRolesInherited(Guid roleId)
        {
            var roles = new List<Guid>
            {
                roleId
            };
            return GenRolesInherited(roles);
        }

        /// <summary>
        ///     Bỏ 1 RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string RemoveRolesInherited(string currInheritedFromRoles, Guid roleId)
        {
            var roles = LoadRolesInherited(currInheritedFromRoles) ?? new List<Guid>();
            if (roles.Contains(roleId)) roles.Remove(roleId);
            var result = GenRolesInherited(roles);
            return result;
        }

        /// <summary>
        ///     Bỏ 1 danh sách RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string RemoveRolesInherited(string currInheritedFromRoles, List<Guid> listRoleId)
        {
            var result = currInheritedFromRoles;
            if (listRoleId == null)
                listRoleId = new List<Guid>();
            foreach (var role in listRoleId) result = RemoveRolesInherited(result, role);
            return result;
        }

        /// <summary>
        ///     Thêm 1 RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string AddRolesInherited(string currInheritedFromRoles, Guid roleId)
        {
            //check RolesInherited truoc
            var roles = LoadRolesInherited(currInheritedFromRoles) ?? new List<Guid>();
            if (!roles.Contains(roleId))
                roles.Add(roleId);
            var result = GenRolesInherited(roles);
            return result;
        }

        /// <summary>
        ///     Thêm 1 danh sách RoleId kế thừa
        /// </summary>
        /// <param name="currInheritedFromRoles"></param>
        /// <param name="listRoleId"></param>
        /// <returns></returns>
        public static string AddRolesInherited(string currInheritedFromRoles, List<Guid> listRoleId)
        {
            var result = currInheritedFromRoles;
            if (listRoleId == null)
                listRoleId = new List<Guid>();
            foreach (var role in listRoleId) result = AddRolesInherited(result, role);
            return result;
        }
    }
    public class Helper
    {
        /// <summary>
        /// Transform data to http response
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ActionResult TransformData(Response data)
        {
            var result = new ObjectResult(data) { StatusCode = (int)data.Code };
            return result;
        }

        /// <summary>
        /// Get user info in token and header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RequestUser GetRequestInfo(HttpRequest request)
        {
            try
            {
                var result = new RequestUser
                {
                    IsAuthenticated = false,
                    UserId = Guid.Empty,
                    UserName = "",
                };

                request.Headers.TryGetValue("X-Permission", out StringValues currentToken);
                if (string.IsNullOrEmpty(currentToken))
                {
                    var token = request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var tokenString = token.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (tokenString.Length > 1)
                        {
                            currentToken = tokenString[1]?.Trim();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(currentToken))
                {
                    string secret = Utils.GetConfig("Authentication:Jwt:Key");
                    var key = Encoding.ASCII.GetBytes(secret);
                    var handler = new JwtSecurityTokenHandler();
                    var validations = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Utils.GetConfig("Authentication:Jwt:Issuer"),
                        ValidAudience = Utils.GetConfig("Authentication:Jwt:Issuer"),
                    };
                    var currentUser = handler.ValidateToken(currentToken, validations, out var tokenSecure);

                    //UserId
                    if (currentUser.HasClaim(c => c.Type == ClaimConstants.USER_ID))
                    {
                        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.USER_ID)?.Value;
                        if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                        {
                            result.UserId = new Guid(userId);
                        }
                    }
                    else
                    {
                        request.Headers.TryGetValue("X-UserId", out StringValues userId);
                        if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                        {
                            result.UserId = new Guid(userId);
                        }
                    }

                    //UserName
                    if (currentUser.HasClaim(c => c.Type == ClaimConstants.USER_NAME))
                    {
                        var userName = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.USER_NAME)?.Value;
                        if (!string.IsNullOrEmpty(userName))
                        {
                            result.UserName = userName;
                        }
                    }

                    //LoginName
                    if (currentUser.HasClaim(c => c.Type == ClaimConstants.FULL_NAME))
                    {
                        var fullName = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.FULL_NAME)?.Value;
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            result.UserName = fullName;
                        }
                    }

                    //PhoneNumber
                    if (currentUser.HasClaim(c => c.Type == ClaimConstants.PHONE))
                    {
                        var phoneNumber = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.PHONE)?.Value;
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            result.PhoneNumber = phoneNumber;
                        }
                    }
                    else
                    {
                        request.Headers.TryGetValue("X-phone", out StringValues phoneNumber);
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            result.PhoneNumber = phoneNumber;
                        }
                    }

                    // }

                    //ListRoles
                    var listRoles = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ROLES)?.Value;
                    if (!string.IsNullOrEmpty(listRoles))
                    {
                        result.ListRoles = JsonConvert.DeserializeObject<List<string>>(listRoles);
                    }
                    //ListRights
                    var listRights = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.RIGHTS)?.Value;
                    if (!string.IsNullOrEmpty(listRights))
                    {
                        result.ListRights = JsonConvert.DeserializeObject<List<string>>(listRights);
                    }

                    // Level
                    var levelClaim = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimConstants.LEVEL);
                    result.Level = int.Parse(levelClaim != null ? levelClaim.Value : "1");

                    
                }
                else
                {
                   
                    request.Headers.TryGetValue("X-UserId", out StringValues userId);
                    if (!string.IsNullOrEmpty(userId) && Utils.IsGuid(userId))
                    {
                        result.UserId = new Guid(userId);
                    }
                    request.Headers.TryGetValue("X-UserName", out StringValues userName);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        result.UserName = userName;
                    }
                }

                result.IsNormalUser = false;
                result.IsNormalUser = result.Level == 1;
                result.IsStaff = false;
                result.IsStaff = result.Level == 4;
                result.IsManager = false;
                result.IsManager = result.Level == 5;
                result.IsAdmin = false;
                result.IsAdmin = result.Level == 10;
                
                if (result.UserId != Guid.Empty)
                    result.IsAuthenticated = true;

                return result;
            }
            catch (Exception exx)
            {
                Log.Error(exx, string.Empty);
                throw;
            }
        }

        /// <summary>
        /// Get user info using a HttpContext
        /// </summary>
        /// <param name="_httpContextAccessor"></param>
        /// <returns></returns>
        public static RequestUser GetRequestInfo(IHttpContextAccessor _httpContextAccessor)
        {
            try
            {
                if (_httpContextAccessor != null)
                {
                    var requestInfo = GetRequestInfo(_httpContextAccessor.HttpContext.Request);
                    return requestInfo;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return null;
            }
        }

        /// <summary>
        /// Create a generic not found response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateNotFoundResponse<T>()
        {
            return new Response<T>(HttpStatusCode.NotFound, default(T), "The requested resource doesn't exist.");
        }

        public static Response<T> CreateNotFoundResponse<T>(string message)
        {
            return new Response<T>(HttpStatusCode.NotFound, default(T), "The requested resource doesn't exist. " + message);
        }

        /// <summary>
        /// Create a generic not found response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateExceptionResponse<T>(Exception ex)
        {
            return new Response<T>(HttpStatusCode.InternalServerError, default(T), Utils.CreateExceptionMessage(ex));
        }

        public static Response<T> CreateExceptionResponse<T>(string message, Exception ex)
        {
            return new Response<T>(HttpStatusCode.InternalServerError, default(T), "Error: " + message + " / " + Utils.CreateExceptionMessage(ex));
        }

        /// <summary>
        /// Create a generic forbidden response to resource with class type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateForbiddenResponse<T>()
        {
            return new Response<T>(HttpStatusCode.Forbidden, default(T), "You don't have access to this resource.");
        }

        /// <summary>
        /// Create a generic forbidden response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateForbiddenResponse()
        {
            return new Response(HttpStatusCode.Forbidden, "You don't have access to this resource.");
        }

        /// <summary>
        /// Create a generic forbidden response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateForbiddenResponse(string message)
        {
            return new Response(HttpStatusCode.Forbidden, "You don't have access to this resource. " + message);
        }

        /// <summary>
        /// Create a generic Bad Request response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Response<T> CreateBadRequestResponse<T>()
        {
            return new Response<T>(HttpStatusCode.BadRequest, default(T), "Your requested data is invalid or malformed.");
        }

        /// <summary>
        /// Create a generic bad request response to resource
        /// </summary>
        /// <returns></returns>
        public static Response CreateBadRequestResponse()
        {
            return new Response(HttpStatusCode.BadRequest, "Your requested data is invalid or malformed.");
        }

        public static Response CreateBadRequestResponse(string message)
        {
            return new Response(HttpStatusCode.BadRequest, "Your requested data is invalid or malformed. " + message);
        }

        public class RequestUser
        {
            public bool IsAuthenticated { get; set; }
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public Guid ApplicationId { get; set; }
            public List<string> ListApps { get; set; }
            public List<string> ListRoles { get; set; }
            public List<string> ListRights { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsNormalUser { get; set; }
            public bool IsStaff{ get; set; }
            public bool IsManager{ get; set; }
            public int Level { get; set; }

        }
    }
}
