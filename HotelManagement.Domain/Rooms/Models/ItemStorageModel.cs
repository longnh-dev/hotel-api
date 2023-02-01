using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class ItemStorageViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Quantity { get; set; }
        public int? AvailableCount { get; set; }
        public int? UnavailableCount { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
    public class ItemStorageCreateModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
    public class ItemStorageUpdateModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
    public class ItemStorageQueryModel : PaginationRequest
    {
    }
}
