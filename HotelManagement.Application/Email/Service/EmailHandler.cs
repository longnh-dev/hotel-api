using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HotelManagement.Application
{
    public class EmailHandler : IEmailHandler
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
      

        public EmailHandler(IConfiguration configuration, HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var eTemplate = await _dbContext.EmailTemplates.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (eTemplate == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");
                _dbContext.EmailTemplates.Remove(eTemplate);

                var status = await _dbContext.SaveChangesAsync();
                if (status > 0)
                {
                    return new ResponseDelete(HttpStatusCode.OK, "Đã xóa thành công", id, "");
                }

                return new ResponseError(HttpStatusCode.BadRequest, "Xoá bản ghi thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetTemplateByCode(string code)
        {
            try
            {
                var query = _dbContext.EmailTemplates?.Where(x => x.Code == code);
               
                var eTemplate = await query.FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                var result = _mapper.Map<EmailTemplate, EmailTemplateViewModel>(eTemplate);
                return new Response<EmailTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Code: {@params}", code);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var query = _dbContext.EmailTemplates?.Where(x => x.Id == id);
                
                var eTemplate = await query?.FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new Response(HttpStatusCode.NotFound, null);

                var result = _mapper.Map<EmailTemplate, EmailTemplateViewModel>(eTemplate);

                return new Response<EmailTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> CreateEmailTemplate(EmailTemplateCreateUpdateModel request)
        {
            try
            {
                var emailTemplate = await _dbContext.EmailTemplates.Where(a => a.Code.ToLower() == request.Code.ToLower()).FirstOrDefaultAsync();

                if (emailTemplate != null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Record existed!");

                emailTemplate = new EmailTemplate();
                emailTemplate.Id = Guid.NewGuid();
                emailTemplate.Name = request.Name;
                emailTemplate.Code = request.Code;
                emailTemplate.Description = request.Description;
                emailTemplate.CreatedOnDate = DateTime.Now;

                _dbContext.Add(emailTemplate);

                var status = await _dbContext.SaveChangesAsync();
                var data = _mapper.Map<EmailTemplate, EmailTemplateViewModel>(emailTemplate);
                return new Response<EmailTemplateViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Request: {@params}", request);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateEmailTemplate(EmailTemplateCreateUpdateModel request, Guid id)
        {
            try
            {
                var eTemplate = await _dbContext.EmailTemplates.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Email Template not found");

                // Update
                eTemplate.Code = request.Code;
                eTemplate.Name = request.Name;
                eTemplate.Description = request.Description;

                await _dbContext.SaveChangesAsync();

                var data = _mapper.Map<EmailTemplate, EmailTemplateViewModel>(eTemplate);
                return new Response<EmailTemplateViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Request: {@request}, Id: {@Id}", request, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetListPageAsync(EmailTemplateQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.EmailTemplates.Where(predicate);

                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<EmailTemplate>, Pagination<EmailTemplateViewModel>>(data);

                
                return new ResponsePagination<EmailTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<EmailTemplate, bool>> BuildQuery(EmailTemplateQueryModel query)
        {
            var predicate = PredicateBuilder.New<EmailTemplate>(true);

            if (query.FromDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate >= query.FromDate.Value);
            }
            if (query.ToDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate <= query.ToDate.Value);
            }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.FullTextSearch.ToLower()));
            }
            return predicate;
        }
    }
}