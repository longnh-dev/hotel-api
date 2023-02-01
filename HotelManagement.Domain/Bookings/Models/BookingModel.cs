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
    public class BookingViewModel
    {

        public Guid Id { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StaffId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CititzenIdentification { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public RoomViewModel? Rooms { get; set; }

        public UserModel? Staff { get; set; }
        public UserModel? Customer { get; set; }
    }
    public class UserBookingModel
    {
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class BookingCreateModel
    {
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? RoomId { get; set; }
        public int NoOfPerson { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage="Invalid email address")]
        public string? Email { get; set; }
        public string? CititzenIdentification { get; set; }
        public string? PaymentType { get; set; }
    }
    public class BookingUpdateModel
    {
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public Guid? StaffId { get; set; }
        public string? CititzenIdentification { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
    }
    public class BookingQueryModel : PaginationRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? StaffId { get; set; }
    }
}
