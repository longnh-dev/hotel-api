using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRoomCategoryService
    {
        Task<Response> Create(RoomCategoryCreateModel model);
        Task<Response> GetById(Guid id);
        Task<Response> GetByCode(string code);
        Task<Response> Update(RoomCategoryUpdateModel model, Guid id);
        Task<Response> GetAllAsync(RoomCategoryQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
