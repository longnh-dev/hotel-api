using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class RightMapUserService : IRightMapUserService
    {
        public RightMapUserService()
        {
        }
        
        public async Task<Response> DeleteRightMapUserAsync(Guid userId, Guid rightId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var currentMap = await unitOfWork.GetRepository<RightMapUser>().FindAsync(s =>
                        s.UserId == userId && s.RightId == rightId);
                    if (currentMap == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng không có quyền !");

                    #endregion Check

                    var self = HotelHelper.MakeIndependentPermission();
                    var selfString = HotelHelper.GenRolesInherited(self);
                    if (currentMap.InheritedFromRoles == selfString ||
                        string.IsNullOrEmpty(currentMap.InheritedFromRoles))
                    {
                        unitOfWork.GetRepository<RightMapUser>().Delete(currentMap);
                    }
                    else
                    {
                        currentMap.Enable = false;
                        unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                    }

                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Loại quyền của người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, RightIds: {@rightId}, ApplicationIds: {@applicationId}", userId, rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteRightMapUserAsync(Guid userId, List<Guid> listRightId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var listCurrenMap = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        s.UserId == userId && listRightId.Contains(s.RightId));
                    if (listCurrenMap.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng không có quyền nào!");

                    #endregion Check

                    var self = HotelHelper.MakeIndependentPermission();
                    var selfString = HotelHelper.GenRolesInherited(self);
                    var listRmuToDelete = new List<RightMapUser>();
                    foreach (var currentMap in listCurrenMap)
                        if (currentMap.InheritedFromRoles == selfString ||
                            string.IsNullOrEmpty(currentMap.InheritedFromRoles))
                        {
                            listRmuToDelete.Add(currentMap);
                        }
                        else
                        {
                            currentMap.Enable = false;
                            unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                        }

                    unitOfWork.GetRepository<RightMapUser>().DeleteRange(listRmuToDelete);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Loại danh sách quyền của người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, ListRightIds: {@listRightId}", userId, listRightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteRightMapUserAsync(List<Guid> listUserId, Guid rightId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var listCurrenMap = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        s.RightId == rightId && listUserId.Contains(s.UserId));
                    if (listCurrenMap.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Không người dùng nào có quyền!");

                    #endregion Check

                    var self = HotelHelper.MakeIndependentPermission();
                    var selfString = HotelHelper.GenRolesInherited(self);
                    var listRmuToDelete = new List<RightMapUser>();
                    foreach (var currentMap in listCurrenMap)
                        if (currentMap.InheritedFromRoles == selfString ||
                            string.IsNullOrEmpty(currentMap.InheritedFromRoles))
                        {
                            listRmuToDelete.Add(currentMap);
                        }
                        else
                        {
                            currentMap.Enable = false;
                            unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                            //var listRolesDepens = String.Join(", ", InHelper.LoadRolesInherited(currentMap.InheritedFromRoles).Select(s => s));
                            //return new Response(ResponseCode.VALIDATE_ERROR, "Cant delete mapping UserId: " + currentMap.UserId.ToString() + "- RightId: " + currentMap.RightId + ", depend on role:" + listRolesDepens, false);
                        }

                    unitOfWork.GetRepository<RightMapUser>().DeleteRange(listRmuToDelete);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Loại danh sách quyền của người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ListUserId: {@listUserId}, RightIds: {@rightId}", listUserId, rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> AddRightMapUserAsync(Guid userId, Guid rightId,
            Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var userRepo = unitOfWork.GetRepository<HtUser>();
                    var userModel = await userRepo.GetAll().Where(x => x.Id.Equals(userId)).AnyAsync();
                    if (!userModel) return new ResponseError(HttpStatusCode.BadRequest, "Người dùng không tồn tại");
                    var rightModel = RightCollection.Instance.GetModel(rightId);
                    if (rightModel == null) return new ResponseError(HttpStatusCode.BadRequest, "Quyền không tồn tại");

                    #endregion Check

                    var currentMap = await unitOfWork.GetRepository<RightMapUser>().FindAsync(s =>
                        s.UserId == userId && s.RightId == rightId );
                    //Tạo role kế thừa . Quyền riêng
                    var role = HotelHelper.MakeIndependentPermission();
                    if (currentMap != null)
                    {
                        currentMap.InheritedFromRoles =
                            HotelHelper.AddRolesInherited(currentMap.InheritedFromRoles, role);
                        currentMap.Enable = true; //Auto enable
                        unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                    }
                    else
                    {
                        var model = new RightMapUser
                        {
                            UserId = userId,
                            RightId = rightId,
                            InheritedFromRoles = HotelHelper.GenRolesInherited(role),
                            Enable = true,
                            Inherited = false,
                        }.InitCreate(actorId);
                        unitOfWork.GetRepository<RightMapUser>().Add(model);
                    }

                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Gán quyền cho người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, RightIds: {@rightId}, ActorIds: {@actorId}", userId, rightId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> AddRightMapUserAsync(Guid userId, List<Guid> listRightId, Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var userRepo = unitOfWork.GetRepository<HtUser>();
                    var userModel = await userRepo.GetAll().Where(x => x.Id.Equals(userId)).AnyAsync();
                    if (!userModel)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng không tồn tại");

                    var listRightModel = RightCollection.Instance.Collection.Where(s => listRightId.Contains(s.Id))
                        .ToList();
                    if (listRightModel.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Không quyền nào không tồn tại");

                    #endregion Check

                    var listCurrentMap = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        s.UserId == userId && listRightId.Contains(s.RightId));
                    //Tạo role kế thừa . Quyền riêng
                    var role = HotelHelper.MakeIndependentPermission();
                    var listRmuToAdd = new List<RightMapUser>();
                    foreach (var currentMap in listCurrentMap)
                    {
                        currentMap.InheritedFromRoles =
                            HotelHelper.AddRolesInherited(currentMap.InheritedFromRoles, role);
                        currentMap.Enable = true; //Auto enable
                        unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                    }

                    foreach (var rightId in listRightId)
                        if (listCurrentMap.All(s => s.RightId != rightId))
                            listRmuToAdd.Add(new RightMapUser
                            {
                                UserId = userId,
                                RightId = rightId,
                                InheritedFromRoles = HotelHelper.GenRolesInherited(role),
                                Enable = true,
                                Inherited = false,
                            }.InitCreate(actorId));
                    if (listRmuToAdd.Any()) unitOfWork.GetRepository<RightMapUser>().AddRange(listRmuToAdd);

                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Gám danh sách quyền cho người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, ListRightIds: {@listRightId}, ActorIds: {@actorId}", userId, listRightId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> AddRightMapUserAsync(List<Guid> listUserId, Guid rightId, Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var userRepo = unitOfWork.GetRepository<HtUser>();
                    var listUserModel = await userRepo.GetAll().Where(x => listUserId.Contains(x.Id)).ToListAsync();
                    if (listUserModel.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Không người dùng nào không tồn tại");
                    var rightModel = RightCollection.Instance.GetModel(rightId);
                    if (rightModel == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Quyền không tồn tại");

                    #endregion Check

                    var listCurrentMap = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        s.RightId == rightId && listUserId.Contains(s.UserId));
                    //Tạo role kế thừa . Quyền riêng
                    var role = HotelHelper.MakeIndependentPermission();
                    var listRmuToAdd = new List<RightMapUser>();
                    foreach (var currentMap in listCurrentMap)
                    {
                        currentMap.InheritedFromRoles =
                            HotelHelper.AddRolesInherited(currentMap.InheritedFromRoles, role);
                        currentMap.Enable = true; //Auto enable
                        unitOfWork.GetRepository<RightMapUser>().Update(currentMap);
                    }

                    foreach (var userId in listUserId)
                        if (listCurrentMap.All(s => s.UserId != userId))
                            listRmuToAdd.Add(new RightMapUser
                            {
                                UserId = userId,
                                RightId = rightId,
                                InheritedFromRoles = HotelHelper.GenRolesInherited(role),
                                Enable = true,
                                Inherited = false,
                            }.InitCreate(actorId));
                    if (listRmuToAdd.Any()) unitOfWork.GetRepository<RightMapUser>().AddRange(listRmuToAdd);

                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Gán quyền cho danh sách người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ListUserId: {@listUserId}, RightIds: {@rightId}, ActorIds: {@actorId}", listUserId, rightId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ToggleRightMapUserAsync(Guid userId, Guid rightId, bool enable,
            Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var currentMap = await unitOfWork.GetRepository<RightMapUser>().FindAsync(s =>
                        s.UserId == userId && s.RightId == rightId);
                    if (currentMap == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng chưa có quyền");

                    #endregion Check

                    currentMap.Enable = enable;
                    unitOfWork.GetRepository<RightMapUser>().Update(currentMap.InitUpdate(actorId));
                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, enable ? "Bật" : "Tắt" + " quyền của người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, RightIds: {@rightId}},Enables: {@enable}, ActorIds: {@actorId}", userId, rightId, enable, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ToggleRightMapUserAsync(List<Guid> listUserId, List<Guid> listRightId,
             bool enable, Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var listCurrenMap = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        listUserId.Contains(s.UserId) && listRightId.Contains(s.RightId)
                        );
                    if (listCurrenMap.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Không người dùng nào có quyền");

                    #endregion Check

                    foreach (var currentMap in listCurrenMap)
                    {
                        currentMap.Enable = enable;
                        unitOfWork.GetRepository<RightMapUser>().Update(currentMap.InitUpdate(actorId));
                    }

                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, enable ? "Bật" : "Tắt" + " quyền của người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ListUserId {@listUserId}, ListRightIds: {@listRightId},Enables: {@enable}, ActorIds: {@actorId}", listUserId, listRightId, enable, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetRightMapUserAsync(Guid userId)

        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var datas = from rmu in unitOfWork.GetRepository<RightMapUser>().GetAll()
                                join r in unitOfWork.GetRepository<Right>().GetAll()
                                    on rmu.RightId equals r.Id
                                where rmu.UserId == userId 
                                select new BaseRightModelOfUser
                                {
                                    Enable = rmu.Enable,
                                    InheritedFromRoles = rmu.InheritedFromRoles,
                                    Inherited = rmu.Inherited,
                                    Id = r.Id,
                                    Code = r.Code,
                                    Name = r.Name,
                                    LastModifiedOnDate = rmu.LastModifiedOnDate,
                                    LastModifiedByUserId = rmu.LastModifiedByUserId,
                                    CreatedOnDate = rmu.CreatedOnDate,
                                    CreatedByUserId = rmu.CreatedByUserId
                                };

                    var queryList = await datas.ToListAsync();
                    var listRoleId = new List<Guid>();
                    foreach (var data in queryList)
                    {
                        data.ListRoleId = HotelHelper.LoadRolesInherited(data.InheritedFromRoles);
                        listRoleId.AddRange(data.ListRoleId);
                    }

                    var listRoleModel = await unitOfWork.GetRepository<Role>()
                        .GetListAsync(s => listRoleId.Contains(s.Id));
                    foreach (var data in queryList)
                        if (data.ListRoleId != null)
                        {
                            var roleData = listRoleModel.Where(s => data.ListRoleId.Contains(s.Id)).ToList();
                            if (roleData.Any())
                                data.ListRole = AutoMapperUtils.AutoMap<Role, BaseRoleModel>(roleData);
                        }

                    var resultData = queryList;
                    return new Response<List<BaseRightModelOfUser>>(resultData);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}", userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetUserMapRightAsync(Guid rightId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var userRepo = unitOfWork.GetRepository<HtUser>();
                    var listUserId = await (from rmu in unitOfWork.GetRepository<RightMapUser>().GetAll()
                                            where rmu.RightId == rightId
                                            select rmu.UserId).ToListAsync();
                    var resultData = await userRepo.GetAll().Where(x => listUserId.Contains(x.Id)).ToListAsync();
                    var result = AutoMapperUtils.AutoMap<HtUser, BaseUserModel>(resultData);
                    return new Response<List<BaseUserModel>>(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: RightId: {@rightId}", rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> CheckRightMapUserAsync(Guid userId, Guid rightId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var currentMap = await unitOfWork.GetRepository<RightMapUser>().FindAsync(s =>
                        s.UserId == userId && s.RightId == rightId);
                    if (currentMap != null && currentMap.Enable)
                        return new Response<bool>(true);
                    return new Response<bool>(false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, RightIds: {@rightId}", userId, rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        // Lưu ý chỉ áp dụng với các quyền hệ thống mang tính global :  PemissionCode
        //public async Task<Response> CheckRightMapUserAsync(Guid userId, Guid rightId)
        //{
        //    try
        //    {
        //        using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
        //        {
        //            var currentMap = await unitOfWork.GetRepository<RightMapUser>().FindAsync(s =>
        //                s.UserId == userId && s.RightId == rightId && s.Enable);
        //            if (currentMap != null)
        //                return new Response<bool>(true);
        //            return new Response<bool>(false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: UserId: {@userId}, RightIds: {@rightId}", userId, rightId);
        //        return Utils.CreateExceptionResponseError(ex);
        //    }
        //}

        // Lưu ý chỉ nhằm phục vụ thêm quyền truy cập hệ thống cho các ứng dụng
        //public async Task<Response> UpdateSystemRightMapAppsAsync(Guid userId, List<Guid> listRighId)
        //{
        //    try
        //    {
        //        using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
        //        {
        //            // Delete current Right
        //            unitOfWork.GetRepository<RightMapUser>().DeleteRange(s => s.UserId == userId && listRighId.Contains(s.RightId));
        //            var listAddRight = new List<RightMapUser>();
        //            foreach (var applicationId in listApplicationId)
        //            {
        //                foreach (var rightId in listRighId)
        //                {
        //                    var model = new RightMapUser()
        //                    {
        //                        Enable = true,
        //                        RightId = rightId,
        //                        UserId = userId
        //                    }.InitCreate(userId);
        //                    listAddRight.Add(model);
        //                }
        //            }
        //            unitOfWork.GetRepository<RightMapUser>().AddRange(listAddRight);
        //            if (await unitOfWork.SaveAsync() > 0)
        //                return new Response(HttpStatusCode.OK, "Gán quyền cho danh sách người dùng thành công");
        //            Log.Error("The sql statement is not executed!");
        //            return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: UserId: {@userId}, ListRightIds: {@listRightId}", userId,  listRighId);
        //        return Utils.CreateExceptionResponseError(ex);
        //    }
        //}
    }
}
