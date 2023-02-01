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
    public class ItemService : IItemService
    {

        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ItemService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> Create(ItemCreateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var item = await _dbContext.Rooms.Where(x => x.Code == model.Code).AnyAsync();
                if (item) return new ResponseError(HttpStatusCode.BadRequest, "Equipment is existed!");

                var data = _mapper.Map<ItemCreateModel, Item>(model);
                return new Response(HttpStatusCode.OK, "Created!");
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
                var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");
                _dbContext.Remove(item);
                await _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK, "Xóa vật dụng thành công");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(ItemQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var data = new Pagination<Item>();
                data = await _dbContext.Items.Include(x => x.ItemStorage).Where(predicate).GetPageAsync(query);
                var result = _mapper.Map<Pagination<Item>, Pagination<ItemViewModel>>(data);

                return new ResponsePagination<ItemViewModel>(result);

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
                var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Code == code);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<Item, ItemViewModel>(item);

                return new Response<ItemViewModel>(data);
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
                var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<Item, ItemViewModel>(item);

                return new Response<ItemViewModel>(data);
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Update(ItemUpdateModel model, Guid id)
        {
            try
            {
                var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                var data = _mapper.Map<Item, ItemUpdateModel>(item);

                return new Response(HttpStatusCode.OK, "Cập nhật thông tin vật dụng thành công");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model {@model}, Id: {@id}",model, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> UpdateStatus(Guid id, string status)
        {
            throw new NotImplementedException();
        }
        private Expression<Func<Item, bool>> BuildQuery(ItemQueryModel query)
        {
            var predicate = PredicateBuilder.New<Item>(true);


            return predicate;
        }
    }
}
