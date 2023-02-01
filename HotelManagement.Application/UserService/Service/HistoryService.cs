using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Linq.Expressions;

namespace HotelManagement.Application
{
    public class HistoryService : IHistoryService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HistoryService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext= dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response> GetAllAsync(HistoryQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.Historys.Where(x => x.CustomerId == query.CustomerId).Include(x => x.Booking).Where(predicate);
                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<History>, Pagination<HistoryViewModel>>(data);

                return new ResponsePagination<HistoryViewModel>(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        private Expression<Func<History, bool>> BuildQuery(HistoryQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<History>(true);

            if (!query.CustomerId.HasValue && currentUser.IsNormalUser)
            {
                predicate.And(x => x.CustomerId == currentUser.UserId);
            }

            if (query.BookingId.HasValue)
            {
                predicate.And(x => x.BookingId == query.BookingId);
            }

            return predicate;
        }
    }
}
