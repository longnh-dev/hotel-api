using HotelManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public interface IRegisterCodeService
    {
       
            Task<Response> GetFilter(RegisterCodeQueryModel filter);

            Task<Response> Create(RegisterCodeCreateModel model);
       
    }
}
