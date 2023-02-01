using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static HotelManagement.SharedKernel.HotelConstant;

namespace HotelManagement.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Module người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/users")]
    [ApiExplorerSettings(GroupName = "Admin User")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRightMapUserService _rightMapUserService;
        private readonly IUserMapRoleService _userMapRoleService;

        public UserController(IUserService userService, IRightMapUserService rightMapUserService, IUserMapRoleService userMapRoleService)
        {
            _userService = userService;
            _rightMapUserService = rightMapUserService;
            _userMapRoleService = userMapRoleService;
        }

        /// <summary>
        /// Thêm mới với role người dùng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpPost, Route("register")]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequestModel model)
        {

            var result = await _userService.RegisterAsync(model);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="oldPassword">Mật khẩu cũ</param>
        /// <param name="password">Mật khẩu mới</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPut, Route("{id}/changepassword")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePasswordAsync(Guid id, [FromQuery] string oldPassword, [FromQuery] string password, [FromQuery] string confirmPassword)
        {
            // Call service
            var result = await _userService.ChangePasswordAsync(id, oldPassword, password, confirmPassword);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        ///  Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous,HttpPut, Route("info/{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInfoAsync(Guid id, [FromBody] UserInfoUpdateModel model)
        {
            // Get Token Info
            //var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userService.UpdateInfoAsync(id, model);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindAsync(Guid id)
        {
            // Call service
            var result = await _userService.FindAsync(id);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Verify email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPut, Route("verify-email")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmail(string email,string token)
        {
            // Call service
            var result = await _userService.VerifyEmail(email, token);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPut, Route("forgot-password/{email}")]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Call service
            var result = await _userService.ForgotPassword(email);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPut, Route("reset-password")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordModel model)
        {

            // Call service
            var result = await _userService.ResetPassword(model.ResetPasswordToken, model.Password);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo bộ loc
        /// </summary>
        /// <param name="page">Số thứ tự trang tìm kiếm</param>
        /// <param name="size">Số bản ghi giới hạn một trang</param>
        /// <param name="filter">Thông tin lọc nâng cao (Object Json)</param>
        /// <param name="sort">Thông tin sắp xếp (Array Json)</param>
        /// <returns></returns>
        /// <remarks>
        ///  *filter*
        ///  ....
        ///  *sort*
        ///  ....
        /// </remarks>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var filterObject = JsonConvert.DeserializeObject<UserQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _userService.GetPageAsync(filterObject);

            // Hander response
            return Helper.TransformData(result);
        }
    }
}
