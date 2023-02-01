using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class ItemStorageService : IItemStorageService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ItemStorageService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<Response> Create(ItemStorageCreateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var validRoom = await _dbContext.Items.Where(x => x.Code == model.Code).AnyAsync();
                if (validRoom) return new ResponseError(HttpStatusCode.BadRequest, "Equipment category is existed");

                return new Response(HttpStatusCode.OK, "Created Equipment category");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var itemStorage = await _dbContext.ItemStorages.FirstOrDefaultAsync(x => x.Id == id);

                if (itemStorage == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");
                _dbContext.Remove(itemStorage);
                await _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK, "Xóa vật dụng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(ItemStorageQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var data = new Pagination<ItemStorage>();
                data = await _dbContext.ItemStorages.Where(predicate).GetPageAsync(query);
                var result = _mapper.Map<Pagination<ItemStorage>, Pagination<ItemStorageViewModel>>(data);

                return new ResponsePagination<ItemStorageViewModel>(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetByCode(string code)
        {
            try
            {
                var item = await _dbContext.ItemStorages.FirstOrDefaultAsync(x => x.Code == code);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<ItemStorage, ItemStorageViewModel>(item);

                return new Response<ItemStorageViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Code: {@code}", code);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var item = await _dbContext.ItemStorages.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<ItemStorage, ItemStorageViewModel>(item);

                return new Response<ItemStorageViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Update(ItemStorageUpdateModel model, Guid id)
        {
            try
            {
                var item = await _dbContext.ItemStorages.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<ItemStorage, ItemStorageUpdateModel>(item);

                return new Response(HttpStatusCode.OK, "Cập nhật thông tin vật dụng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model {@model}, Id: {@id}", model, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> UpdateStatus(Guid id, string status)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<ItemStorage, bool>> BuildQuery(ItemStorageQueryModel query)
        {
            var predicate = PredicateBuilder.New<ItemStorage>(true);


            return predicate;
        }
    }
}
