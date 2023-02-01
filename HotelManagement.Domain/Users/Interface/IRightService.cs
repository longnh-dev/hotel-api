using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRightService
    {
        Task<Response> CreateAsync(RightCreateModel model, Guid actorId);

        Task<Response> CreateAsync(Guid id, RightCreateModel model, Guid actorId);

        Task<Response> UpdateAsync(Guid id, RightUpdateModel model, Guid actorId);

        Task<Response> DeleteAsync(Guid id);

        Task<Response> DeleteRangeAsync(List<Guid> listId);

        Task<Response> FindAsync(Guid id);

        Task<Response> GetAllAsync();

        Task<Response> GetAllAsync(RightQueryModel query);

        Task<Response> GetPageAsync(RightQueryModel query);

        Task<Response> GetDetail(Guid rightId);

        Response GetAll();
    }
}
