using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRightMapUserService
    {
        /// <summary>
        ///     Xóa 1 quyền của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapUserAsync(Guid userId, Guid rightId);

        /// <summary>
        ///     Xóa 1 danh sách quyền của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listRightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapUserAsync(Guid userId, List<Guid> listRightId);

        /// <summary>
        ///     Xóa quyền của 1 danh sách các người dùng
        /// </summary>
        /// <param name="listUserId"></param>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> DeleteRightMapUserAsync(List<Guid> listUserId, Guid rightId);

        /// <summary>
        ///     Thêm 1 quyền cho người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapUserAsync(Guid userId, Guid rightId, Guid actorId);

        /// <summary>
        ///     Thêm 1 danh sách quyền cho người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listRightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapUserAsync(Guid userId, List<Guid> listRightId,Guid actorId);

        /// <summary>
        ///     Thêm 1 quyền cho danh sách người dùng
        /// </summary>
        /// <param name="listUserId"></param>
        /// <param name="rightId"></param>
        /// <param name="appId"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> AddRightMapUserAsync(List<Guid> listUserId, Guid rightId, Guid actorId);

        /// <summary>
        ///     Bật/Tắt quyền của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rightId"></param>
        /// <param name="enable"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> ToggleRightMapUserAsync(Guid userId, Guid rightId, bool enable, Guid actorId);

        /// <summary>
        ///     Bật/Tắt 1 danh sách quyền - người dùng
        /// </summary>
        /// <param name="listUserId"></param>
        /// <param name="listRightId"></param>
        /// <param name="enable"></param>
        /// <param name="actorId"></param>
        /// <returns></returns>
        Task<Response> ToggleRightMapUserAsync(List<Guid> listUserId, List<Guid> listRightId,
            bool enable, Guid actorId);

        /// <summary>
        ///     Lấy danh sách quyền của người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Response> GetRightMapUserAsync(Guid userId);

        /// <summary>
        ///     Lấy danh sách người dùng có quyền
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        Task<Response> GetUserMapRightAsync(Guid rightId);

        ///// <summary>
        /////     Kiểm tra quyền của người dùng
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="rightId"></param>
        ///// <param name="applicationId"></param>
        ///// <returns></returns>
        //Task<Response> CheckRightMapUserAsync(Guid userId, Guid rightId);
        //Task<Response> UpdateSystemRightMapAppsAsync(Guid userId, List<Guid> listRighId);
    }
}
