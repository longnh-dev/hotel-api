using HotelManagement.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class BaseRoleModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }

    public class RoleModel : BaseRoleModel
    {
        public string Description { get; set; }
    }

    public class RoleDetailModel : RoleModel
    {
        public List<BaseRightModel> ListRight { get; set; }
        public List<BaseUserModel> ListUser { get; set; }
    }

    public class RoleQueryModel : PaginationRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class RoleCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        ////Plus
        ///// <summary>
        ///// Danh sách quyền thêm
        ///// </summary>
        //[JsonIgnore]
        //public List<Guid> ListAddRightId { get; set; }

        ///// <summary>
        ///// Danh sách người dùng thêm
        ///// </summary>
        //[JsonIgnore]
        //public List<Guid> ListAddUserId { get; set; }
    }

    public class RoleUpdateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        ////Plus
        ///// <summary>
        ///// Danh sách quyền thêm
        ///// </summary>
        //public List<Guid> ListAddRightId { get; set; }

        ///// <summary>
        ///// Danh sách người dùng thêm
        ///// </summary>
        //public List<Guid> ListAddUserId { get; set; }

        ////Plus
        ///// <summary>
        ///// Danh sách quyền xóa
        ///// </summary>
        //public List<Guid> ListDeleteRightId { get; set; }

        ///// <summary>
        ///// Danh sách người dùng xóa
        ///// </summary>
        //public List<Guid> ListDeleteUserId { get; set; }
    }
}
