using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRoleService
    {
        Task<Response> CreateAsync(RoleCreateModel mode);

        Task<Response> UpdateAsync(Guid id, RoleUpdateModel model, Guid actorId);

        Task<Response> DeleteAsync(Guid id);

        Task<Response> DeleteRangeAsync(List<Guid> listId);

        Task<Response> FindAsync(Guid id);

        Task<Response> GetDetail(Guid id);

        Task<Response> GetPageAsync(RoleQueryModel query);

        Task<Response> GetAllAsync(RoleQueryModel query);

        Task<Response> GetAllAsync();

        Response GetAll();
    }
}
