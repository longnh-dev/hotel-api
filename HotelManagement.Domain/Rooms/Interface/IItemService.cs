using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IItemService
    {
        Task<Response> Create(ItemCreateModel model);
        Task<Response> GetById(Guid id);
        Task<Response> UpdateStatus(Guid id, string status);
        Task<Response> GetByCode(string code);
        Task<Response> Update(ItemUpdateModel model, Guid id);
        Task<Response> GetAllAsync(ItemQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
