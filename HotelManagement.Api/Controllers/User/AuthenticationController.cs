using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HotelManagement.Api.Controllers.User
{
    /// <summary>
    /// JWT cho hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/authentication")]
    [ApiExplorerSettings(GroupName = "Authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IUserMapRoleService _userMapRoleService;
        private readonly IRightMapUserService _rightMapUserService;
        private readonly HotelDbContext _dbContext;
        private static readonly HttpClient Client = new HttpClient();

        public AuthenticationController(IConfiguration config, IUserService userService, IUserMapRoleService userMapRoleService, IRightMapUserService rightMapUserService, HotelDbContext dbContext)
        {
            _config= config;
            _userService= userService;
            _userMapRoleService= userMapRoleService;
            _rightMapUserService= rightMapUserService;  
            _dbContext= dbContext;
        }

        /// <summary>
        /// Lấy thông tin jwt
        /// </summary>
        /// <returns></returns>
        [Route("jwt/info")]
        [Authorize, HttpPost]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> JwtInfo()
        {
            var requestInfo = Helper.GetRequestInfo(Request);
            return await _userService.BuildToken(requestInfo.UserId, true);
        }

        /// <summary>
        /// Đăng nhập và lấy kết quả JWT token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [Route("jwt/login")]
        [AllowAnonymous, HttpPost]
        [ProducesResponseType(typeof(Response<LoginResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignInJwt([FromBody] LoginModel login)
        {
            IActionResult response = Unauthorized();

            //var accessToken = string.Empty;
            //// Remove access token
            //var getAccessToken = GetApiManagerAccessTokenAsync().Result;
            //if (getAccessToken != null) accessToken = getAccessToken.access_token;

            if (login.Username == _config["Authentication:AdmIdmUser"] &&
                 login.Password == _config["Authentication:AdminPassWord"])
            {
                return await _userService.BuildToken(UserConstants.AdministratorUserId, login.RememberMe);
            }
            if (login.Username == _config["Authentication:GuestUser"] && login.Password == _config["Authentication:GuestPassWord"])
            {
                return await _userService.BuildToken(UserConstants.GuestUserId, login.RememberMe);
            }
            var userAuthResponse = _userService.Authentication(login.Username, login.Password);
            if (userAuthResponse.Code == HttpStatusCode.OK && userAuthResponse is Response<UserModel> userData)
            {
                return await _userService.BuildToken(userData.Data.Id, login.RememberMe);
            }
            else
            {
                return Helper.TransformData(userAuthResponse);
            }
        }
        private async Task<AuthModel> GetApiManagerAccessTokenAsync()
        {
            try
            {
                string _apiGatewayRootUrl = _config["Authentication:WSO2APIManager:Uri"];
                string _clientId = _config["Authentication:WSO2APIManager:ClientId"];
                string _clientSecret = _config["Authentication:WSO2APIManager:ClientSecret"];

                Uri getAccessTokenUri = new Uri(_apiGatewayRootUrl + "/token")
                    .AddQuery("grant_type", "client_credentials");

                var getAccessTokenRequest = (HttpWebRequest)WebRequest.Create(getAccessTokenUri);
                getAccessTokenRequest.Method = "POST";
                string basicToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_clientId + ":" + _clientSecret));
                getAccessTokenRequest.ContentType = "application/x-www-form-urlencoded";
                getAccessTokenRequest.Accept = "application/json, text/javascript, */*";
                getAccessTokenRequest.Headers.Add("Authorization", "Basic " + basicToken);

                using (var getAccessTokenResponse = (HttpWebResponse)getAccessTokenRequest.GetResponse())
                {
                    var streamResponse = getAccessTokenResponse.GetResponseStream();
                    MemoryStream ms = new MemoryStream();
                    streamResponse.CopyTo(ms);
                    ms.Position = 0;
                    HttpResponseMessage resultResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(ms)
                    };
                    resultResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    string resultResponseStringData = await resultResponse.Content.ReadAsStringAsync();

                    var resultResponseJsonData = JsonConvert.DeserializeObject<AuthModel>(resultResponseStringData);
                    return resultResponseJsonData;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return null;
            }
        }
    }
}
