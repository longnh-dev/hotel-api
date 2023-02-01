using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelManagement.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/rooms")]
    [ApiExplorerSettings(GroupName = "Room management")]
    [AllowAnonymous]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService service)
        {
            _service= service;
        }

        /// <summary>
        /// Lấy danh sách phòng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponsePagination<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<RoomQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _service.GetAllAsync(filterObject);

            return Helper.TransformData(result);
        }
        /// <summary>
        /// Thêm mới phòng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] RoomCreateModel model)
        {
            // Get Token Info
            //var requestInfo = Helper.GetRequestInfo(Request);
            //var actorId = requestInfo.UserId;

            // Call service
            var result = await _service.Create(model);
            // Hander response
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Lấy thông tin chi tiết phòng bằng Id
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        { 
            // Call service
            var result = await _service.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        } 
        
        /// <summary>
        /// Xóa phòng 
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, Allow(RoleConstants.AdminRoleCode), HttpDelete]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        { 
            // Call service
            var result = await _service.Delete(id);
            // Hander response
            return Helper.TransformData(result);
        } 
        
        /// <summary>
        /// Lấy thông tin chi tiết phòng bằng Code
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet("code/{code}")]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCode(string code)
        { 
            // Call service
            var result = await _service.GetByCode(code);
            // Hander response
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Cập nhật thông tin phòng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody]RoomUpdateModel model, Guid id)
        { 
            // Call service
            var result = await _service.Update(model, id);
            // Hander response
            return Helper.TransformData(result);
        }
        
        /// <summary>
        /// Cập nhật trạng thái phòng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Allow(RoleConstants.ManagerRoleCode, RoleConstants.StaffRoleCode), HttpPut("status")]
        [ProducesResponseType(typeof(Response<RoomViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, string status)
        { 
            // Call service
            var result = await _service.UpdateStatus(id, status);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}
