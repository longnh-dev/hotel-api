using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IParameterService
    {
        Task<Response> FindByNameAsync(string name);

        Task<Response> FindAsync(Guid id);

        Task<Response> CreateAsync(ParameterCreateModel model, Guid userId);

        Task<Response> UpdateAsync(Guid id, ParameterUpdateModel model, Guid userId);

        Task<Response> DeleteAsync(Guid id);

        Task<Response> DeleteRangeAsync(List<Guid> listId);

        Task<Response> GetAllAsync();

        Task<Response> GetAllAsync(ParameterQueryModel query);

        Task<Response> GetPageAsync(ParameterQueryModel query);

        Response GetAll();
        Task SaveEntity(ParameterModel model);
    }
}
