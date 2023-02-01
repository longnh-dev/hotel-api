using HotelManagement.Domain.Common;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public interface IEmailHandler
    {
        Task<Response> GetById(Guid id);

        Task<Response> GetTemplateByCode(string code);

        Task<Response> Delete(Guid ịd);

        Task<Response> CreateEmailTemplate(EmailTemplateCreateUpdateModel request);

        Task<Response> UpdateEmailTemplate(EmailTemplateCreateUpdateModel request, Guid id);

        Task<Response> GetListPageAsync(EmailTemplateQueryModel query);

    }
}