using HotelManagement.Application;
using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace HotelManagement.Api.Controllers
{
    /// <summary>
    ///     Core - Upload
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/email")]
    [ApiExplorerSettings(GroupName = "Email ")]
    public class EmailController : ControllerBase
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public EmailController(IHostEnvironment env, IConfiguration configuration, IEmailService emailService)
        {
            _env = env;
            _configuration = configuration;
            
            _emailService = emailService;
        }

        /// <summary>
        /// Email verify
        /// </summary>
        /// <param name="model">email request</param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost]
        [Route("verification/request")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Request(VerifyEmailModel model)
        {
            var response = await _emailService.RequestVerification(model);

            return Helper.TransformData(response);
            // Call service

        }

        /// <summary>
        /// Email verify
        /// </summary>
        /// <param name="model">email request</param>
        /// <returns></returns>
        [Authorize, HttpPost]
        [Route("verification/confirm")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Confirm(ConfirmEmailModel model)
        {
            var response = await _emailService.ConfirmVerification(model);

            return Helper.TransformData(response);
            // Call service

        }
    }
}
