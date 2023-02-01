using HotelManagement.SharedKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class RegisterCodeViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public int ExpiredTime { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public bool IsExpired { get; set; }
        public string Message { get; set; }
    }

    public class RegisterCodeCreateModel
    {
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }

        [JsonIgnore]
        public string CreatedByUser { get; set; }
    }

    public class RegisterCodeQueryModel : PaginationRequest
    {
        public string Status { get; set; }
        public DateTime? CreatedOnDate { get; set; }
    }
}
