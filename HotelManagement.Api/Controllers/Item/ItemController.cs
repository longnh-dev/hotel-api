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
    [Route("api/v{api-version:apiVersion}/items")]
    [ApiExplorerSettings(GroupName = "Item management")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;
        public ItemController(IItemService service)
        {
            _service = service;
        }

        /// <summary>
        /// Thêm mới vật dụng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost]
        [ProducesResponseType(typeof(Response<ItemViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ItemCreateModel model)
        {
            // Call service
            var result = await _service.Create(model);
            // Hander response
            return Helper.TransformData(result);
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
        //[Authorize]
        [ProducesResponseType(typeof(ResponsePagination<ItemViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "-CreatedOnDate")
        {
            var filterObject = JsonConvert.DeserializeObject<ItemQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _service.GetAllAsync(filterObject);

            return Helper.TransformData(result);
        }
        

        /// <summary>
        /// Lấy thông tin chi tiết vật dụng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<ItemViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromBody] Guid id)
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
        /// Lấy thông tin chi tiết vật dụng bằng Code
        /// </summary>
        /// <param name="code">Mã vật dụng</param>
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
        /// Cập nhật thông tin vật dụng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut]
        [ProducesResponseType(typeof(Response<ItemViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] ItemUpdateModel model, Guid id)
        {
            // Call service
            var result = await _service.Update(model, id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật trạng thái vật dụng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Allow(RoleConstants.ManagerRoleCode, RoleConstants.StaffRoleCode), HttpPut("status")]
        [ProducesResponseType(typeof(Response<ItemViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, string status)
        {
            // Call service
            var result = await _service.UpdateStatus(id, status);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}
