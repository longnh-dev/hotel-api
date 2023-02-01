using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class RoleCollection
    {
        private readonly IRoleService _handler;
        public HashSet<RoleModel> Collection;
        public static RoleCollection Instance { get; } = new RoleCollection();

        protected RoleCollection()
        {
            _handler = new RoleService();
            LoadToHashSet();
        }

        public void LoadToHashSet()
        {
            Collection = new HashSet<RoleModel>();
            // Query to list
            var listResponse = _handler.GetAll();
            // Add to hashset
            if (listResponse.Code == HttpStatusCode.OK)
            {
                // Add to hashset
                if (listResponse is Response<List<RoleModel>> listResponseData)
                    foreach (var response in listResponseData.Data)
                    {
                        Collection.Add(response);
                    }
            }
        }

        public string GetName(Guid id)
        {
            var result = Collection.FirstOrDefault(u => u.Id == id);
            return result?.Name;
        }

        public BaseRoleModel GetModel(Guid id)
        {
            var result = Collection.FirstOrDefault(u => u.Id == id);
            return result;
        }

        public BaseRoleModel GetModel(string userName)
        {
            var result = Collection.FirstOrDefault(u => u.Name == userName);
            return result;
        }

        public List<BaseRoleModel> GetModel(Expression<Func<BaseRoleModel, bool>> predicate)
        {
            var result = Collection.AsQueryable().Where(predicate);
            return result.ToList();
        }

        public BaseRoleModel GetCode(string cONTRIBUTOR_LV2_Code)
        {
            var result = Collection.FirstOrDefault(u => u.Code == cONTRIBUTOR_LV2_Code);
            return result;
        }
    }
}
