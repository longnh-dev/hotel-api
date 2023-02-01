using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class RoomViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public decimal Price{ get; set; }
        public int Capacity { get; set; }
        public int Size { get; set; }
        public bool Breakfast { get; set; }
        public bool Pet { get; set; }
        public string CategoryRoom { get; set; }
        public Guid? RoomCategoryId { get; set; }
        public RoomCategoryViewModel? RoomCategory { get; set; }
    }
    public class RoomCreateModel
    {

        public string? ThumbnailUrl { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Size{ get; set; }
        public int? Capacity{ get; set; }
        public Guid? RoomCategoryId { get; set; }
    }
    public class RoomUpdateModel
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class RoomQueryModel : PaginationRequest
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public Guid? RoomCategoryId { get; set; }
    }
}
