using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRightMapRoleService
    {
        /// <summary>
        /// Xóa một quyền trong nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapRoleAsync(Guid roleId, Guid rightId);

        /// <summary>
        /// Xóa nhiều quyền trong nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="listRightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapRoleAsync(Guid roleId, List<Guid> listRightId);

        /// <summary>
        /// Xóa nhiều nhóm chứa quyền
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapRoleAsync(List<Guid> listRoleId, Guid rightId);

        /// <summary>
        /// Thêm một quyền trong nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapRoleAsync(Guid roleId, Guid rightId, Guid actorId);

        /// <summary>
        /// Thêm nhiều quyền trong nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="listRightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapRoleAsync(Guid roleId, List<Guid> listRightId, Guid actorId);

        /// <summary>
        /// Thêm nhiều nhóm chứa quyền
        /// </summary>
        /// <param name="listRoleId"></param>
        /// <param name="rightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapRoleAsync(List<Guid> listRoleId, Guid rightId, Guid actorId);

        /// <summary>
        /// Lấy sanh sách quyền của 1 nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Response> GetRightMapRoleAsync(Guid roleId);

        /// <summary>
        /// Lấy danh sách nhóm người dùng chứa quyền
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> GetRoleMapRightAsync(Guid rightId);

        /// <summary>
        /// Kiểm tra nhóm người dùng có quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> CheckRightMapRoleAsync(Guid roleId, Guid rightId);
    }
}
