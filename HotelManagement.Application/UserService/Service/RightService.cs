using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
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
    public class RightService : IRightService
    {
        private readonly DbHandler<Right, RightModel, RightQueryModel> _dbHandler =
            DbHandler<Right, RightModel, RightQueryModel>.Instance;

        private readonly IRightMapRoleService _rightMapRoleService;
        private readonly IRightMapUserService _rightMapUserService;

        public RightService(IRightMapRoleService rightMapRoleService, IRightMapUserService rightMapUserService)
        {
            _rightMapRoleService = rightMapRoleService;
            _rightMapUserService = rightMapUserService;
        }

        public RightService()
        {
        }

        public Task<Response> GetAllAsync()
        {
            var predicate = PredicateBuilder.New<Right>(true);
            return _dbHandler.GetAllAsync(predicate);
        }

        public Task<Response> GetAllAsync(RightQueryModel query)
        {
            var predicate = BuildQuery(query);
            return _dbHandler.GetAllAsync(predicate, query.Sort);
        }

        public Response GetAll()
        {
            return _dbHandler.GetAll();
        }

        public Task<Response> GetPageAsync(RightQueryModel query)
        {
            var predicate = BuildQuery(query);
            return _dbHandler.GetPageAsync(predicate, query);
        }

        public async Task<Response> GetDetail(Guid rightId)
        {
            try
            {
                var model = await _dbHandler.FindAsync(rightId);
                var modelData = model as Response<RightModel>;
                if (model.Code == HttpStatusCode.OK)
                {
                    if (modelData != null)
                    {
                        var result = AutoMapperUtils.AutoMap<RightModel, RightDetailModel>(modelData.Data);
                        var listRoleResponse = await _rightMapRoleService.GetRightMapRoleAsync(rightId);
                        var listUserResponse = await _rightMapUserService.GetUserMapRightAsync(rightId);
                        if (listRoleResponse.Code == HttpStatusCode.OK && listUserResponse.Code == HttpStatusCode.OK)
                        {
                            var listRoleResponseData = listRoleResponse as Response<List<BaseRoleModel>>;
                            var listUserResponseData = listUserResponse as Response<List<BaseUserModel>>;
                            if (listRoleResponseData != null) result.ListRole = listRoleResponseData.Data;
                            if (listUserResponseData != null) result.ListUser = listUserResponseData.Data;
                            return new Response<RightDetailModel>(result);
                        }
                    }

                    return new ResponseError(HttpStatusCode.BadRequest, "Không thể lấy dữ liệu  chi tiết");
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: RightId {@rightId}, ApplicationIds: {@applicationId}", rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private static Expression<Func<Right, bool>> BuildQuery(RightQueryModel query)
        {
            var predicate = PredicateBuilder.New<Right>(true);
            if (!string.IsNullOrEmpty(query.Code)) predicate.And(s => s.Code == query.Code);
            if (!string.IsNullOrEmpty(query.Name)) predicate.And(s => s.Name == query.Name);
            if (query.Id.HasValue) predicate.And(s => s.Id == query.Id);
            if (query.ListId != null) predicate.And(s => query.ListId.Contains(s.Id));

            // if(query.ApplicationId.HasValue&& !query.SearchAllApp){
            //     predicate.And(s => s.ApplicationId == query.ApplicationId.Value);
            // }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s =>
                    s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                    s.Description.ToLower().Contains(query.FullTextSearch.ToLower()));
            if (!string.IsNullOrEmpty(query.GroupCode))
                predicate.And(s => s.GroupCode == query.GroupCode);

            return predicate;
        }

        #region CRUD

        public async Task<Response> CreateAsync(RightCreateModel model, Guid actorId)
        {
            var id = Guid.NewGuid();
            var result = await CreateAsync(id, model, actorId);
            return result;
        }

        public async Task<Response> CreateAsync(Guid id, RightCreateModel model, Guid actorId)
        {
            try
            {
                var request = AutoMapperUtils.AutoMap<RightCreateModel, Right>(model);
                request.Id = id;
                var result = await _dbHandler.CreateAsync(request);
                if (result.Code == HttpStatusCode.OK)
                {
                    RightCollection.Instance.LoadToHashSet();

                    #region Realtive

                    if (model.ListAddUserId != null)
                        await _rightMapUserService.AddRightMapUserAsync(model.ListAddUserId, request.Id, 
                             actorId);
                    if (model.ListAddRoleId != null)
                        await _rightMapRoleService.AddRightMapRoleAsync(model.ListAddRoleId, request.Id, 
                             actorId);

                    #endregion Realtive
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id {@id}, Models: {@model}, AppIds: {@appId}, ActorIds: {@actorId}", id, model, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateAsync(Guid id, RightUpdateModel model, Guid actorId)
        {
            try
            {
                var request = AutoMapperUtils.AutoMap<RightUpdateModel, Right>(model);
                request.Id = id;
                var result = await _dbHandler.UpdateAsync(id, request);
                if (result.Code == HttpStatusCode.OK)
                {
                    RightCollection.Instance.LoadToHashSet();

                    #region Realtive

                    if (model.ListAddUserId != null)
                        await _rightMapUserService.AddRightMapUserAsync(model.ListAddUserId, id,
                            actorId);
                    if (model.ListAddRoleId != null)
                        await _rightMapRoleService.AddRightMapRoleAsync(model.ListAddRoleId, id,
                            actorId);
                    if (model.ListDeleteRoleId != null)
                        await _rightMapUserService.DeleteRightMapUserAsync(model.ListAddUserId, id);
                    if (model.ListDeleteUserId != null)
                        await _rightMapRoleService.DeleteRightMapRoleAsync(model.ListDeleteUserId, id);

                    #endregion Realtive
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Models: {@model}, AppIds: {@appId}, ActorIds: {@actorId}", id, model, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                //Casade
                var result = await _dbHandler.DeleteAsync(id);
                RightCollection.Instance.LoadToHashSet();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteRangeAsync(List<Guid> listId)
        {
            try
            {
                var result = await _dbHandler.DeleteRangeAsync(listId, "Id");

                RightCollection.Instance.LoadToHashSet();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ListId: {@params}", listId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> FindAsync(Guid id)
        {
            return _dbHandler.FindAsync(id);
        }

        #endregion CRUD
    }
}
