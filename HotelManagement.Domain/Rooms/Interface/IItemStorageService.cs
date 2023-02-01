using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IItemStorageService
    {
        Task<Response> Create(ItemStorageCreateModel model);
        Task<Response> GetById(Guid id);
        Task<Response> UpdateStatus(Guid id, string status);
        Task<Response> GetByCode(string code);
        Task<Response> Update(ItemStorageUpdateModel model, Guid id);
        Task<Response> GetAllAsync(ItemStorageQueryModel query);
        Task<Response> Delete(Guid id);
    }
}
