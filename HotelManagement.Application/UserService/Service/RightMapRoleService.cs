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
using static System.Net.Mime.MediaTypeNames;

namespace HotelManagement.Application
{
    public class RightMapRoleService : IRightMapRoleService
    {
        public RightMapRoleService()
        {

        }

        public async Task<Response> AddRightMapRoleAsync(Guid roleId, Guid rightId, Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var currentMap = await unitOfWork.GetRepository<RightMapRole>().FindAsync(s =>
                        s.RoleId == roleId && s.RightId == rightId);
                    if (currentMap != null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Nhóm người dùng đã có quyền");
                    var roleModel = RoleCollection.Instance.GetModel(roleId);
                    if (roleModel == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Nhóm người dùng không tồn tại!");
                    var rightModel = RightCollection.Instance.GetModel(rightId);
                    if (rightModel == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Quyền không tồn tại!");

                    #endregion Check

                    unitOfWork.GetRepository<RightMapRole>().Add(new RightMapRole
                    {
                        RightId = rightId,
                        RoleId = roleId,
                    }.InitCreate(actorId));
                    /*Thêm mới right mapp user của user kế thừa*/
                    var childUsers = await unitOfWork.GetRepository<UserMapRole>()
                        .Get(s => s.RoleId == roleId).Select(s => s.UserId)
                        .ToListAsync();
                    var listCurrentRmu = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                        childUsers.Contains(s.UserId) && s.RightId == rightId);
                    var listRmuToAdd = new List<RightMapUser>();
                    foreach (var currentRmu in listCurrentRmu)
                    {
                        currentRmu.InheritedFromRoles =
                            HotelHelper.AddRolesInherited(currentRmu.InheritedFromRoles, roleId);
                        currentRmu.Inherited = true;
                        currentRmu.Enable = true;
                        unitOfWork.GetRepository<RightMapUser>().Update(currentRmu.InitUpdate(actorId));
                    }

                    foreach (var userId in childUsers)
                        if (listCurrentRmu.All(s => s.UserId != userId))
                            listRmuToAdd.Add(new RightMapUser
                            {
                                Enable = true,
                                InheritedFromRoles = HotelHelper.GenRolesInherited(roleId),
                                Inherited = true,
                                RightId = rightId,
                                UserId = userId
                            }.InitCreate( actorId));
                    if (listRmuToAdd.Any()) unitOfWork.GetRepository<RightMapUser>().AddRange(listRmuToAdd);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Gán quyền cho nhóm người dùng thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: RoleId: {@roleId}, RightIds: {@rightId}", roleId, rightId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> AddRightMapRoleAsync(Guid roleId, List<Guid> listRightId, Guid actorId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> AddRightMapRoleAsync(List<Guid> listRoleId, Guid rightId, Guid actorId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> CheckRightMapRoleAsync(Guid roleId, Guid rightId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteRightMapRoleAsync(Guid roleId, Guid rightId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteRightMapRoleAsync(Guid roleId, List<Guid> listRightId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteRightMapRoleAsync(List<Guid> listRoleId, Guid rightId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> GetRightMapRoleAsync(Guid roleId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var datas = await(from rmu in unitOfWork.GetRepository<RightMapRole>().GetAll()
                                      join r in unitOfWork.GetRepository<Right>().GetAll()
                    on rmu.RightId equals r.Id
                                      where rmu.RoleId == roleId
                                      select r).ToListAsync();

                    var resultData = AutoMapperUtils.AutoMap<Right, BaseRightModel>(datas);
                    return new Response<List<BaseRightModel>>(resultData);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: RoleId: {@roleId}", roleId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> GetRoleMapRightAsync(Guid rightId)
        {
            throw new NotImplementedException();
        }
    }
}
