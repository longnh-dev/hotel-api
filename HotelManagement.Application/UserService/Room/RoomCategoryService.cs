using AutoMapper;
using Elasticsearch.Net;
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
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class RoomCategoryService : IRoomCategoryService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public RoomCategoryService(HotelDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response> Create(RoomCategoryCreateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                #region check existed
                var validCategoryRoom = await _dbContext.RoomCategories.FirstOrDefaultAsync(x => x.Code == model.Code); //.Include(x=>x.Rooms)
                if (validCategoryRoom != null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Loại phòng đã tồn tại, vui lòng kiểm tra lại thông tin!");
                #endregion

                var roomCategory = _mapper.Map<RoomCategoryCreateModel, RoomCategory>(model);
                roomCategory.CreatedByUserId = currentUser.UserId;
                if (roomCategory == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Đã xảy ra lỗi trong quá trình tạo loại phòng!");

                _dbContext.Add(roomCategory);
                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Tạo loại phòng thành công");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@Param}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var roomCategory = await _dbContext.RoomCategories.FirstOrDefaultAsync(x=>x.Id.Equals(id));

                if(roomCategory == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Loại phòng không tồn tại!");

                _dbContext.Remove(roomCategory);
                await _dbContext.SaveChangesAsync();

                return new ResponseError(HttpStatusCode.OK, "Xóa loại phòng thành công!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@Param}", id);
                return Utils.CreateExceptionResponseError(ex);

            }
        }

        public async Task<Response> GetAllAsync(RoomCategoryQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var predicate = BuildQuery(query);
                var data = new Pagination<RoomCategory>();
                data = await _dbContext.RoomCategories.Where(predicate).GetPageAsync(query);
                var result = _mapper.Map<Pagination<RoomCategory>, Pagination<RoomCategoryViewModel>>(data);

                if (result != null)
                {
                    return new ResponsePagination<RoomCategoryViewModel>(result);
                }

                return new ResponseError(HttpStatusCode.BadRequest, "Không tìm thấy dữ liệu");
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
                var roomCategory = await _dbContext.RoomCategories.FirstOrDefaultAsync(x => x.Code.Equals(code));

                if (roomCategory == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng không tồn tại!");

                var data = _mapper.Map<RoomCategory, RoomCategoryViewModel>(roomCategory);

                return new Response<RoomCategoryViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Code: {@code}", code);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var roomCategory = await _dbContext.RoomCategories.FirstOrDefaultAsync(x => x.Id.Equals(id));

                if (roomCategory == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng không tồn tại!");


                var data = _mapper.Map<RoomCategory, RoomCategoryViewModel>(roomCategory);

                return new Response<RoomCategoryViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Update(RoomCategoryUpdateModel model, Guid id)
        {
            try
            {
                var roomCategory = _dbContext.RoomCategories.FirstOrDefault(x => x.Equals(id));

                if (roomCategory == null)
                    return new Response(HttpStatusCode.BadRequest, "Phòng không tồn tại");

                roomCategory = _mapper.Map<RoomCategoryUpdateModel, RoomCategory>(model);
                var data = _mapper.Map<RoomCategory, RoomCategoryViewModel>(roomCategory);
                return new Response<RoomCategoryViewModel>(HttpStatusCode.OK, data, "Cập nhật thông tin loại phòng thành công");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}, Id: {@id}", model, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<RoomCategory, bool>> BuildQuery(RoomCategoryQueryModel query)
        {
            var predicate = PredicateBuilder.New<RoomCategory>(true);

            if (!string.IsNullOrEmpty(query.Name))
            {
                predicate.And(s => s.Name == query.Name);
            }
            if (!string.IsNullOrEmpty(query.Code))
            {
                predicate.And(s => s.Code == query.Code);
            }

            return predicate;
        }
    }
}
