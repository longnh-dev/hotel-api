using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.Contact
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/contact")]
    [ApiExplorerSettings(GroupName = "Contact")]
    public class ContactController
    {

        private readonly IContactService _service;
        public ContactController(IContactService service)
        {
            _service = service;
        }

        /// <summary>
        /// Thêm mới vật dụng
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Response<>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ContactCreateModel model)
        {
            // Call service
            var result = await _service.Create(model);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}
