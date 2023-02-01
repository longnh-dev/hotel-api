using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Nest;
using Serilog;
using System.Linq.Expressions;
using System.Net;

namespace HotelManagement.Application
{
    public class RoomService : IRoomService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _roomStatus = {RoomStatusConstant.AvailableRoom, RoomStatusConstant.UnavailableRoom };
        public RoomService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response> Create(RoomCreateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var validRoom = await _dbContext.Rooms.Where(x => x.Code == model.Code).AnyAsync();
                if (validRoom) return new ResponseError(HttpStatusCode.BadRequest, "Phòng đã tồn tại");
                var category = await _dbContext.RoomCategories.FirstOrDefaultAsync(x => x.Id == model.RoomCategoryId);
                var room = _mapper.Map<RoomCreateModel, Room>(model);
                if (category == null)
                {
                    var roomCategory = new RoomCategory();
                    roomCategory.Id = Guid.NewGuid();
                    roomCategory.Code = model.Name?.ToUpper().First()+"-"+model.Code?.ToUpper();
                    roomCategory.Name = model.Name?.ToUpper().Split('-', StringSplitOptions.RemoveEmptyEntries).First();
                    roomCategory.Price = 0;
                    roomCategory.CreatedOnDate = DateTime.Now;
                    roomCategory.LastModifiedOnDate= DateTime.Now;
                    roomCategory.CreatedByUserId = currentUser.UserId;
                    room.RoomCategoryId = roomCategory.Id;
                    room.Status = RoomStatusConstant.AvailableRoom;
                    room.Size = model.Size;
                    room.Capacity = model.Capacity;

                    _dbContext.Add(roomCategory);
                }
                else
                {
                    room.Size = model.Size;
                    room.Capacity = model.Capacity;
                    room.Status = RoomStatusConstant.AvailableRoom;
                    room.RoomCategoryId = category.Id;
                }

                _dbContext.Rooms.Add(room);
                await _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK, "Created Room");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}",model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var room = await _dbContext.Rooms.FirstOrDefaultAsync(x=>x.Id.Equals(id));

                if(room== null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng không tồn tại!");
                _dbContext.Remove(room);
                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Xóa phòng thành công!");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(RoomQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.Rooms.Include(x => x.RoomCategory).Where(predicate);
                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<Room>, Pagination<RoomViewModel>>(data);

                foreach(var item in result.Content)
                {
                    item.CategoryRoom = item.RoomCategory.Name;
                    item.Price = item.RoomCategory.Price;
                }
                
                return new ResponsePagination<RoomViewModel>(result);
                
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
                var room = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Code.Equals(code));

                if (room == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng không tồn tại!");

                var data = _mapper.Map<Room, RoomViewModel>(room);

                return new Response<RoomViewModel>(data);
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
                var room = await _dbContext.Rooms.Include(x=>x.RoomCategory).FirstOrDefaultAsync(x => x.Id.Equals(id));

                if (room == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng không tồn tại!");

                var data = _mapper.Map<Room, RoomViewModel>(room);

                data.Price = room.RoomCategory.Price;
                data.CategoryRoom = room.RoomCategory.Name;

                return new Response<RoomViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Update(RoomUpdateModel model, Guid id)
        {
            try
            {
                var room = _dbContext.Rooms.FirstOrDefault(x=>x.Equals(id));

                if (room == null)
                    return new Response(HttpStatusCode.BadRequest, "Phòng không tồn tại");

                room = _mapper.Map<RoomUpdateModel, Room>(model);
                room.Status = RoomStatusConstant.AvailableRoom;
                var data = _mapper.Map<Room, RoomViewModel>(room);
                return new Response<RoomViewModel>(HttpStatusCode.OK,data,"Cập nhật thông tin phòng thành công");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}, Id: {@id}", model,id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateStatus(Guid id, string status)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var room = _dbContext.Rooms.FirstOrDefault(x => x.Equals(id));

                #region Valid role, status & existed
                if (currentUser.IsNormalUser && currentUser.IsAdmin)
                    return new Response(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập chức năng này");
                if (room == null)
                    return new Response(HttpStatusCode.BadRequest, "Phòng không tồn tại");
                if (!_roomStatus.Contains(status))
                    return new ResponseError(HttpStatusCode.BadRequest, "Trạng thái phòng không hợp lệ");
                #endregion

                room.Status = status;

                _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK, "Cập nhật trạng thái phòng thành công!");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<Room, bool>> BuildQuery(RoomQueryModel query)
        {
            var predicate = PredicateBuilder.New<Room>(true);

            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.FullTextSearch)
                                    || x.RoomCategory.Name.Contains(query.FullTextSearch)
                                                                     || x.Status.Contains(query.FullTextSearch) || x.Capacity.ToString().Contains(query.FullTextSearch)
                                                                     || x.RoomCategory.Price.ToString().Contains(query.FullTextSearch));
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                predicate.And(s => s.Name == query.Name);
            }
            if (!string.IsNullOrEmpty(query.Category))
            {
                predicate.And(s => s.RoomCategory.Name == query.Category);
            }
            if (!string.IsNullOrEmpty(query.Code))
            {
                predicate.And(s => s.Code == query.Code);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                predicate.And(s => s.Status == query.Status);
            }
            if (query.RoomCategoryId.HasValue)
            {
                predicate.And(s => s.RoomCategoryId == query.RoomCategoryId);
            }

            return predicate;
        }
    }
}
