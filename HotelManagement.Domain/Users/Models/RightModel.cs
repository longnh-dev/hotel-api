using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class BaseRightModel : BaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class RightModel : BaseRightModel
    {
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public bool Status { get; set; }
        public bool IsSystem { get; set; }
        public int Order { get; set; }
    }

    public class RightDetailModel : RightModel
    {
        public List<BaseUserModel> ListUser { get; set; }
        public List<BaseRoleModel> ListRole { get; set; }
    }

    public class RightQueryModel : PaginationRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string GroupCode { get; set; }
        public bool? Status { get; set; }
    }

    public class RightCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? UserId { get; set; }
        public string GroupCode { get; set; }
        public bool Status { get; set; }
        public bool IsSystem { get; set; }
        public int Order { get; set; }

        public Guid ApplicatonId { get; set; }

        //Plus
        /// <summary>
        ///     Danh sách nhóm người dùng thêm
        /// </summary>
        public List<Guid> ListAddRoleId { get; set; }

        /// <summary>
        ///     Danh sách người dùng thêm
        /// </summary>
        public List<Guid> ListAddUserId { get; set; }
    }

    public class RightUpdateModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupCode { get; set; }
        public bool Status { get; set; }
        public bool IsSystem { get; set; }
        public int Order { get; set; }
        public Guid? UserId { get; set; }

        public Guid ApplicatonId { get; set; }

        //Plus
        /// <summary>
        ///     Danh sách nhóm người dùng thêm
        /// </summary>
        public List<Guid> ListAddRoleId { get; set; }

        /// <summary>
        ///     Danh sách người dùng thêm
        /// </summary>
        public List<Guid> ListAddUserId { get; set; }

        /// <summary>
        ///     Danh sách nhóm người dùng xóa
        /// </summary>
        public List<Guid> ListDeleteRoleId { get; set; }

        /// <summary>
        ///     Danh sách người dùng xóa
        /// </summary>
        public List<Guid> ListDeleteUserId { get; set; }
    }

    public class BaseRightModelOfUser : BaseRightModel
    {
        public bool Enable { get; set; }
        public bool Inherited { get; set; }
        public string InheritedFromRoles { get; set; }
        public List<BaseRoleModel> ListRole { get; set; }
        public List<Guid> ListRoleId { get; set; }
    }

    public class RightInfo
    {
        public List<BaseUserModel> ListUser { get; set; }
        public List<BaseRoleModel> ListRole { get; set; }
    }
}
