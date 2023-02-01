using Autofac.Core;
using HotelManagement.Domain.Common;
using HotelManagement.Domain;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelManagement.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/roomsCategory")]
    [ApiExplorerSettings(GroupName = "Room Category management")]
    public class RoomCategoryController : ControllerBase
    {
        private readonly IRoomCategoryService _service;

        public RoomCategoryController(IRoomCategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách loại phòng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponsePagination<RoomCategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<RoomCategoryQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _service.GetAllAsync(filterObject);

            return Helper.TransformData(result);
        }
        /// <summary>
        /// Thêm mới loại phòng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost]
        [ProducesResponseType(typeof(Response<RoomCategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] RoomCategoryCreateModel model)
        {
            // Call service
            var result = await _service.Create(model);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết loại phòng bằng Id
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<RoomCategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Call service
            var result = await _service.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa loại phòng 
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, Allow(RoleConstants.AdminRoleCode), HttpDelete]
        [ProducesResponseType(typeof(Response<RoomCategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Call service
            var result = await _service.Delete(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết loại phòng bằng Code
        /// </summary>
        /// <param name="code">Mã loại phòng</param>
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
        /// <param name="id">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<RoomCategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] RoomCategoryUpdateModel model, Guid id)
        {
            // Call service
            var result = await _service.Update(model, id);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}
