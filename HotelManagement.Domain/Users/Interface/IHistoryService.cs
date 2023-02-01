using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IHistoryService
    {
        Task<Response> GetAllAsync(HistoryQueryModel query);

    }
}
