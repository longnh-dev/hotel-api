using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class RoomCategoryViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public decimal Price { get; set; }
        public Guid? CreatedByUserId { get; set; }

    }
    public class RoomCategoryCreateModel
    {
        public string? Name { get; set; }
        public string? Code { get; set; }

        public decimal Price { get; set; }
    } 
    public class RoomCategoryUpdateModel
    {
    } 
    public class RoomCategoryQueryModel : PaginationRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
