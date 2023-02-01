using HotelManagement.SharedKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class VerifyEmailModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
    }

    public class ConfirmEmailModel
    {
        [StringLength(6)]
        public string? Token { get; set; }
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [JsonIgnore]
        public Guid ByUserId { get; set; }
    }
    public class EmailTemplateViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        [JsonIgnore]
        public string? TitleTemplate { get; set; }

        [JsonIgnore]
        public string? BodyTemplate { get; set; }
    }
    public class EmailTemplateCreateUpdateModel
    {
        [JsonIgnore]
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
    public class EmailTemplateQueryModel : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Language { get; set; }
        public string? Name { get; set; }
    }
    
}
