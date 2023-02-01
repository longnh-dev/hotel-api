using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace HotelManagement.Api.Controllers.User
{
    /// <inheritdoc />
    /// <summary>
    /// Module nhóm người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/roles")]
    [ApiExplorerSettings(GroupName = "Admin Role")]
    [AllowAnonymous]//Allow(RoleConstants.AdminRoleCode)
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IRightMapRoleService _rightMapRoleService;
        private readonly IUserMapRoleService _userMapRoleService;
        public RoleController(IRoleService roleService, IRightMapRoleService rightMapRoleService, IUserMapRoleService userMapRoleService)
        {
            _roleService = roleService;
            _rightMapRoleService = rightMapRoleService; 
            _userMapRoleService= userMapRoleService;
        }
        #region CRUD

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] RoleCreateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            //var actorId = requestInfo.UserId;

            // Call service
            var result = await _roleService.CreateAsync(model);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="model">Dữ liệu</param>
        /// <param name="applicationId"></param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] RoleUpdateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _roleService.UpdateAsync(id, model, actorId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseDelete), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // Call service
            var result = await _roleService.DeleteAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa danh sách
        /// </summary>
        /// <param name="listId">Danh sách id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("")]
        [ProducesResponseType(typeof(ResponseDeleteMulti), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRangeAsync([FromBody] List<Guid> listId)
        {
            // Call service
            var result = await _roleService.DeleteRangeAsync(listId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo Id
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            // Call service
            var result = await _roleService.FindAsync(id);
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
        [ProducesResponseType(typeof(ResponsePagination<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "-")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<RoleQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
          
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _roleService.GetPageAsync(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về tất cả
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("all")]
        [ProducesResponseType(typeof(Response<List<RoleModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery] string filter = "{}")
        {
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var filterObject = JsonConvert.DeserializeObject<RoleQueryModel>(filter);
            var result = await _roleService.GetAllAsync(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        #endregion CRUD

        /// <summary>
        /// Lấy về chi tiết
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{id}/detail")]
        [ProducesResponseType(typeof(Response<List<RoleDetailModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var result = await _roleService.GetDetail(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về danh sách người dùng thuộc nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{id}/user")]
        [ProducesResponseType(typeof(Response<List<BaseUserModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMapRoleAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var result = await _userMapRoleService.GetUserMapRoleAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về danh sách quyền thuộc nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{id}/right")]
        [ProducesResponseType(typeof(Response<List<BaseRightModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRightMapRoleAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _rightMapRoleService.GetRightMapRoleAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gán quyền vào nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="listRightId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("{id}/right")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRightMapRoleAsync([FromBody] List<Guid> listRightId, Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _rightMapRoleService.AddRightMapRoleAsync(id, listRightId, actorId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gán người dùng vào nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="listUserId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("{id}/user")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserMapRoleAsync([FromBody] List<Guid> listUserId, Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _userMapRoleService.AddUserMapRoleAsync(id, listUserId, actorId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gỡ quyền khỏi nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="listRightId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("{id}/right")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRightMapRoleAsync([FromBody] List<Guid> listRightId, Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _rightMapRoleService.DeleteRightMapRoleAsync(id, listRightId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gỡ người dùng khỏi nhóm
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="listUserId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("{id}/user")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserMapRoleAsync([FromBody] List<Guid> listUserId, Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var result = await _userMapRoleService.DeleteUserMapRoleAsync(id, listUserId);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}
