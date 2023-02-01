using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class RightCollection
    {
        private readonly IRightService _handler;
        public HashSet<RightModel> Collection;
        public static RightCollection Instance { get; } = new RightCollection();

        protected RightCollection()
        {
            _handler = new RightService();
            LoadToHashSet();
        }

        public void LoadToHashSet()
        {
            Collection = new HashSet<RightModel>();
            // Query to list
            var listResponse = _handler.GetAll();
            // Add to hashset
            if (listResponse.Code == HttpStatusCode.OK)
            {
                // Add to hashset
                if (listResponse is Response<List<RightModel>> listResponseData)
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

        public BaseRightModel GetModel(Guid id)
        {
            var result = Collection.FirstOrDefault(u => u.Id == id);
            return result;
        }

        public BaseRightModel GetModel(string userName)
        {
            var result = Collection.FirstOrDefault(u => u.Name == userName);
            return result;
        }

        public List<BaseRightModel> GetModel(Expression<Func<BaseRightModel, bool>> predicate)
        {
            var result = Collection.AsQueryable().Where(predicate);
            return result.ToList();
        }
    }
}
