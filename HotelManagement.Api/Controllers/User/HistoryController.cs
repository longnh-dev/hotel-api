using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotelManagement.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Module nhóm người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/history")]
    [ApiExplorerSettings(GroupName = "History")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _service;
        public HistoryController(IHistoryService service)
        {
            _service = service;
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
        [ProducesResponseType(typeof(ResponsePagination<HistoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilter([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var filterObject = JsonConvert.DeserializeObject<HistoryQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _service.GetAllAsync(filterObject);

            return Helper.TransformData(result);
        }
    }
}
