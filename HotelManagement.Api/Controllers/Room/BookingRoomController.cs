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
    [Route("api/v{api-version:apiVersion}/bookings")]
    [ApiExplorerSettings(GroupName = "Booking")]
    public class BookingRoomController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingRoomController(IBookingService service)
        {
            _service= service;
        }

        /// <summary>
        /// Thêm mới phòng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost]
        [ProducesResponseType(typeof(Response<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] BookingCreateModel model)
        {
            // Call service
            var result = await _service.Create(model);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách đặt phòng
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponsePagination<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "-CreatedOnDate")
        {
            var filterObject = JsonConvert.DeserializeObject<BookingQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _service.GetAllAsync(filterObject);

            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết đặt phòng bằng Id
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Call service
            var result = await _service.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPut("status/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id)
        {
            // Call service
            var result = await _service.UpdatePayment(id);
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
        [ProducesResponseType(typeof(Response<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Call service
            var result = await _service.Delete(id);
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
        [ProducesResponseType(typeof(Response<BookingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] BookingUpdateModel model, Guid id)
        {
            // Call service
            var result = await _service.Update(model, id);
            // Hander response
            return Helper.TransformData(result);
        }

    }
}
