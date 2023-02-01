using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IUserMapRoleService
    {
        /// <summary>
        /// Xóa 1 người dùng trong nhóm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> DeleteUserMapRoleAsync(Guid roleId, Guid userId);

        /// <summary>
        /// Xóa 1 danh sách người dùng trong nhóm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="listUserId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> DeleteUserMapRoleAsync(Guid roleId, List<Guid> listUserId);

        /// <summary>
        /// Xóa 1 danh sách nhóm của người dùng
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> DeleteUserMapRoleAsync(List<Guid> listRoleId, Guid userId);

        /// <summary>
        /// Thêm 1 người dùng trong nhóm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddUserMapRoleAsync(Guid roleId, Guid userId, Guid actorId);

        /// <summary>
        /// Thêm 1 danh sách người dùng trong nhóm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="listUserId"></param>
        /// <param name="applicationId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddUserMapRoleAsync(Guid roleId, List<Guid> listUserId, Guid actorId);

        /// <summary>
        /// Thêm một danh sách nhóm cho người dùng
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddUserMapRoleAsync(List<Guid> listRoleId, Guid userId, Guid actorId);

        /// <summary>
        /// Lấy danh sách người dùng nằm trong nhóm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> GetUserMapRoleAsync(Guid roleId);

        /// <summary>
        /// Lấy danh sach các nhóm của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> GetRoleMapUserAsync(Guid userId);

        /// <summary>
        /// Kiểm tra người dùng thuộc nhóm
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<Response> CheckRoleMapUserAsync(Guid userId, Guid roleId);

        Task<Response> UpdateSystemRoleMapAppsAsync(Guid userId, List<Guid> listRoleId);
    }
}
