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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HotelManagement.Application
{
    public class UserMapRoleService : IUserMapRoleService
    {
        private IUnitOfWork unitOfWork;
        private readonly HotelDbContext _dbContext;

        public UserMapRoleService(IUnitOfWork _unitOfWork, HotelDbContext dbContext)
        {
            this.unitOfWork = _unitOfWork;
            _dbContext = dbContext;
        }
        public Task<Response> AddUserMapRoleAsync(Guid roleId, Guid userId, Guid actorId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> AddUserMapRoleAsync(Guid roleId, List<Guid> listUserId, Guid actorId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> AddUserMapRoleAsync(List<Guid> listRoleId, Guid userId, Guid actorId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    #region Check

                    var userRepo = unitOfWork.GetRepository<HtUser>();

                    var listCurrentMap = await unitOfWork.GetRepository<UserMapRole>().GetListAsync(s =>
                        s.UserId == userId && listRoleId.Contains(s.RoleId));
                    if (listCurrentMap.Count == listRoleId.Count)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng đã tồn tại trong tất cả nhóm !");

                    var userModel = await userRepo.GetAll().Where(x => x.Id.Equals(userId)).AnyAsync();
                    if (!userModel)
                        return new ResponseError(HttpStatusCode.BadRequest, "Người dùng không tồn tại");
                    var listRoleModel = RoleCollection.Instance.Collection.Where(s => listRoleId.Contains(s.Id))
                        .ToList();
                    if (listRoleModel.Count == 0)
                        return new ResponseError(HttpStatusCode.BadRequest, "Không nhóm người dùng nào tồn tại");

                    #endregion Check
                    var listUmrToAdd = new List<UserMapRole>();
                    foreach (var roleModel in listRoleModel)
                    {
                        var roleName = await _dbContext.Roles.Where(x => x.Id == roleModel.Id).FirstOrDefaultAsync();
                        var user = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

                        listUmrToAdd.Add(new UserMapRole
                        {
                            UserId = userId,
                            RoleId = roleModel.Id,
                            Rolename = roleName.Name,
                            Username = user.Name
                        }.InitCreate(actorId));
                    }
                        
                    if (listUmrToAdd.Any()) unitOfWork.GetRepository<UserMapRole>().AddRange(listUmrToAdd);
                    /*Thêm mới User mapp user của user kế thừa*/
                    var listRmuToAdd = new List<RightMapUser>();
                    //foreach (var roleId in listRoleId)
                    //{
                    //    var childRights = await unitOfWork.GetRepository<RightMapRole>()
                    //        .Get(s => s.RoleId == roleId).Select(s => s.RightId)
                    //        .ToListAsync();
                    //    var listCurrentRmu = await unitOfWork.GetRepository<RightMapUser>().GetListAsync(s =>
                    //        s.UserId == userId && childRights.Contains(s.RightId));
                    //    foreach (var currentRmu in listCurrentRmu)
                    //    {
                    //        currentRmu.InheritedFromRoles =
                    //            HotelHelper.AddRolesInherited(currentRmu.InheritedFromRoles, roleId);
                    //        currentRmu.Inherited = true;
                    //        currentRmu.Enable = true;
                    //        unitOfWork.GetRepository<RightMapUser>()
                    //            .Update(currentRmu.InitUpdate(actorId));
                    //    }

                    //    foreach (var rightId in childRights)
                    //        if (listCurrentRmu.All(s => s.RightId != rightId))
                    //            listRmuToAdd.Add(new RightMapUser
                    //            {
                    //                Enable = true,
                    //                InheritedFromRoles = HotelHelper.GenRolesInherited(roleId),
                    //                Inherited = true,
                    //                RightId = rightId,
                    //                UserId = userId,
                    //            }.InitCreate(actorId));
                    //}

                    if (listRmuToAdd.Any()) unitOfWork.GetRepository<RightMapUser>().AddRange(listRmuToAdd);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new Response(HttpStatusCode.OK, "Gán người dùng vào danh sách nhóm thành công");
                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ListRoleId: {@listRoleId}, UserIds: {@userId}, ActorIds: {@actorId}", listRoleId, userId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> CheckRoleMapUserAsync(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteUserMapRoleAsync(Guid roleId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteUserMapRoleAsync(Guid roleId, List<Guid> listUserId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteUserMapRoleAsync(List<Guid> listRoleId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> GetRoleMapUserAsync(Guid userId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var datas = from rmu in unitOfWork.GetRepository<UserMapRole>().GetAll()
                                join r in unitOfWork.GetRepository<Role>().GetAll()
                                    on rmu.RoleId equals r.Id
                                where rmu.UserId == userId 
                                select r;
                    var queryList = await datas.ToListAsync();
                    var resultData = AutoMapperUtils.AutoMap<Role, BaseRoleModel>(queryList);
                    return new Response<List<BaseRoleModel>>(resultData);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}", userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> GetUserMapRoleAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateSystemRoleMapAppsAsync(Guid userId, List<Guid> listRoleId)
        {
            throw new NotImplementedException();
        }
    }
}
