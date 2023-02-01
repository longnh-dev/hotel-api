using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class ItemViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public Guid RoomId { get; set; }
        public RoomViewModel? Room { get; set; }
        public string? Type { get; set; }
        public Guid ItemStorageId { get; set; }
        public ItemStorageViewModel? ItemStorage { get; set; }
    }
    public class ItemCreateModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public Guid? RoomId { get; set; }
        public string? Type { get; set; }
        public Guid? ItemStorageId { get; set; }
    }
    public class ItemUpdateModel
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public Guid? RoomId { get; set; }
        public string? Type { get; set; }
        public Guid? ItemStorageId { get; set; }
    }
    public class ItemQueryModel: PaginationRequest
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? ItemStorageId { get; set; }
    }
}
