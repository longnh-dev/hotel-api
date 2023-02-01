using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
using Microsoft.AspNetCore.Routing;
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
    public class RoleService : IRoleService
    {
        private readonly DbHandler<Role, RoleModel, RoleQueryModel> _dbHandler =
           DbHandler<Role, RoleModel, RoleQueryModel>.Instance;

        private readonly IRightMapRoleService _rightMapRoleService;
        private readonly IUserMapRoleService _userMapRoleService;

        public RoleService(IRightMapRoleService rightMapRoleService, IUserMapRoleService userMapRoleService)
        {
            _rightMapRoleService = rightMapRoleService;
            _userMapRoleService = userMapRoleService;
        }
        public RoleService()
        {

        }
        public async Task<Response> GetDetail(Guid id)
        {
            try
            {
                var model = await _dbHandler.FindAsync(id);

                if (model.Code == HttpStatusCode.OK)
                {
                    var modelData = model as Response<RoleModel>;
                    var result = AutoMapperUtils.AutoMap<RoleModel, RoleDetailModel>(modelData?.Data);
                    var listRightResponse = await _rightMapRoleService.GetRightMapRoleAsync(id);
                    var listUserResponse = await _userMapRoleService.GetUserMapRoleAsync(id);
                    if (listRightResponse.Code == HttpStatusCode.OK && listUserResponse.Code == HttpStatusCode.OK)
                    {
                        var listRightResponseData = listRightResponse as Response<List<BaseRightModel>>;
                        var listUserResponseData = listUserResponse as Response<List<BaseUserModel>>;
                        if (listRightResponseData != null) result.ListRight = listRightResponseData.Data;
                        if (listUserResponseData != null) result.ListUser = listUserResponseData.Data;
                        return new Response<RoleDetailModel>(result);
                    }

                    return new ResponseError(HttpStatusCode.BadRequest, "Không thể lấy dữ liệu  chi tiết");
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetPageAsync(RoleQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                return await _dbHandler.GetPageAsync(predicate, query);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(RoleQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                return await _dbHandler.GetAllAsync(predicate, query.Sort);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync()
        {
            try
            {
                var predicate = PredicateBuilder.New<Role>(true);
                return await _dbHandler.GetAllAsync(predicate);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Response GetAll()
        {
            return _dbHandler.GetAll();
        }

        private static Expression<Func<Role, bool>> BuildQuery(RoleQueryModel query)
        {
            var predicate = PredicateBuilder.New<Role>(true);
            if (!string.IsNullOrEmpty(query.Code)) predicate.And(s => s.Code == query.Code);
            if (!string.IsNullOrEmpty(query.Name)) predicate.And(s => s.Name == query.Name);
            if (query.Id.HasValue) predicate.And(s => s.Id == query.Id);
            if (query.ListId != null) predicate.And(s => query.ListId.Contains(s.Id));

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s =>
                    s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                    s.Description.ToLower().Contains(query.FullTextSearch.ToLower()));
            return predicate;
        }

        #region CRUD

        public async Task<Response> CreateAsync(RoleCreateModel model)
        {
            try
            {
                var request = AutoMapperUtils.AutoMap<RoleCreateModel, Role>(model);
                request.Id = Guid.NewGuid();
                var result = await _dbHandler.CreateAsync(request, "Name", "Code");
                //if (result.Code == HttpStatusCode.OK)
                //{
                //    RoleCollection.Instance.LoadToHashSet();

                //    #region Realtive

                //    if (model.ListAddRightId != null)
                //        await _rightMapRoleService.AddRightMapRoleAsync(request.Id, model.ListAddRightId, actorId);
                //    if (model.ListAddUserId != null)
                //        await _userMapRoleService.AddUserMapRoleAsync(request.Id, model.ListAddUserId, actorId);

                //    #endregion Realtive
                //}

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateAsync(Guid id, RoleUpdateModel model, Guid actorId)
        {
            try
            {
                var request = AutoMapperUtils.AutoMap<RoleUpdateModel, Role>(model);
                request.Id = id;
                var result = await _dbHandler.UpdateAsync(id, request, "Name", "Code");
                //if (result.Code == HttpStatusCode.OK)
                //{
                //    RoleCollection.Instance.LoadToHashSet();

                //    #region Realtive

                //    if (model.ListAddRightId != null)
                //        await _rightMapRoleService.AddRightMapRoleAsync(id, model.ListAddRightId, actorId);
                //    if (model.ListAddUserId != null)
                //        await _userMapRoleService.AddUserMapRoleAsync(id, model.ListAddUserId,  actorId);
                //    if (model.ListDeleteRightId != null)
                //        await _rightMapRoleService.DeleteRightMapRoleAsync(id, model.ListDeleteRightId);
                //    if (model.ListDeleteUserId != null)
                //        await _userMapRoleService.DeleteUserMapRoleAsync(id, model.ListDeleteUserId);

                //    #endregion Realtive
                //}

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params},Models: {@Models} AppIds: {@AppIds}, ActorIds: {@ActorIds}", id, model, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var currentRole = await unitOfWork.GetRepository<Role>().FindAsync(id);
                    if (currentRole == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Nhóm người dùng không tồn tại", null);
                    //Lấy về danh sách các kế thừa
                    var listItemDelete = new List<RightMapUser>();
                    var idString = id.ToString();
                    var currentRmu = await unitOfWork.GetRepository<RightMapUser>()
                        .GetListAsync(s => s.InheritedFromRoles.Contains(idString));
                    foreach (var mapItem in currentRmu)
                    {
                        //Nếu InheritedFromRoles chỉ chứa mình quyền đó =>> xóa
                        //Nếu không khai trừ role ra khỏi InheritedFromRoles
                        var listRole = HotelHelper.LoadRolesInherited(mapItem.InheritedFromRoles);
                        if (listRole.Count > 1)
                        {
                            if (listRole.Contains(HotelHelper.MakeIndependentPermission())
                            ) //nếu có 2 inherist với 1 cái là self
                                mapItem.Inherited = false;
                            mapItem.InheritedFromRoles = HotelHelper.RemoveRolesInherited(mapItem.InheritedFromRoles, id);
                            unitOfWork.GetRepository<RightMapUser>().Update(mapItem);
                        }
                        else
                        {
                            listItemDelete.Add(mapItem);
                        }
                    }

                    if (listItemDelete.Count == 0)
                        unitOfWork.GetRepository<RightMapUser>().DeleteRange(listItemDelete);
                    unitOfWork.GetRepository<Role>().Delete(currentRole);
                    if (await unitOfWork.SaveAsync() > 0)
                    {
                        RoleCollection.Instance.LoadToHashSet();
                        return new ResponseDelete(id, currentRole.Name);
                    }

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteRangeAsync(List<Guid> listId)
        {
            try
            {
                var resultData = new List<ResponseDelete>();
                foreach (var id in listId)
                {
                    var model = await DeleteAsync(id) as ResponseDelete;
                    resultData.Add(model);
                }

                var result = new ResponseDeleteMulti(resultData);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", listId);
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
