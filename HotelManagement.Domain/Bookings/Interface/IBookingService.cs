using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IBookingService
    {
        Task<Response> Create(BookingCreateModel model);
        Task<Response> GetById(Guid id);
        Task<Response> Update(BookingUpdateModel model, Guid id);
        Task<Response> UpdatePayment(Guid id);
        Task<Response> GetAllAsync(BookingQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
