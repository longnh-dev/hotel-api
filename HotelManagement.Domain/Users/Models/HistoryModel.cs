using HotelManagement.Domain.Common;
using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class HistoryViewModel
    {
        public Guid Id { get; set; }
        public BookingInHistoryModel? Booking { get; set; }
        public UserHistoryModel? Customer { get; set; }
    }

    public class UserHistoryModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
    public class BookingInHistoryModel
    {
        public Guid Id { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public Guid? StaffId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CititzenIdentification { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
    }
    public class HistoryQueryModel : PaginationRequest
    {
        public Guid? BookingId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
