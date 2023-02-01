using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class ContactService : IContactService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContactService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> Create(ContactCreateModel model)
        {
            try
            {
               
                var data = _mapper.Map<ContactCreateModel, Contact>(model);

                _dbContext.Add(data);
                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Send feedback successfully");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
    }
}
