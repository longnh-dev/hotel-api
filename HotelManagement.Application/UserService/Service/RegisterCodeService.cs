using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel.Constant;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HotelManagement.SharedKernel.RegisterCodeConstants;

namespace HotelManagement.Application.User.Service
{
    public class RegisterCodeService : IRegisterCodeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private readonly DbHandler<RegisterCode, RegisterCodeViewModel, RegisterCodeQueryModel> _dbHandler =
          DbHandler<RegisterCode, RegisterCodeViewModel, RegisterCodeQueryModel>.Instance;

        public RegisterCodeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> GetFilter(RegisterCodeQueryModel filter)
        {
            try
            {
                var iqueryable = _unitOfWork.GetRepository<RegisterCode>().GetAll();

                // Build query
                var predicate = BuildQuery(filter);

                iqueryable = iqueryable.Where(predicate);

                // Select Model
                var query = iqueryable.Select(x => new RegisterCodeViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Status = x.Status,
                    ExpiredTime = x.ExpiredTime,
                    CreatedOnDate = x.CreatedOnDate,
                    IsExpired = x.CreatedOnDate.GetValueOrDefault().AddSeconds(x.ExpiredTime) <= DateTime.Now
                });

                var result = await _dbHandler.GetPageAsyncCustom(query, filter);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Filter: {@params}", filter);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Create(RegisterCodeCreateModel model)
        {
            try
            {
                var registerCodeTimeout = await ParameterCollection.Instance.GetIntValue(ParamConstants.REGISTER_CODE_TIMEOUT);

                //Repo
                var registerCodeRepo = _unitOfWork.GetRepository<RegisterCode>();

                var code = "";

                do
                {
                    code = Utils.RandomString(6);
                }
                while (await registerCodeRepo.GetAll().Where(x => x.Code.Equals(code) && x.Status.Equals(RegisterCodeStatus.UN_USED)).AnyAsync() == true);

                var entityModel = new RegisterCode
                {
                    Code = code,
                    ExpiredTime = registerCodeTimeout,
                    CreatedByUser = model.CreatedByUser,
                    CreatedByUserId = model.CreatedByUserId,
                    Status = RegisterCodeStatus.UN_USED
                };

                registerCodeRepo.Add(entityModel);

                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    var data = _mapper.Map<RegisterCode, RegisterCodeViewModel>(entityModel);
                    data.Message = $"Mã {entityModel.Code} sẽ hết hạn sau " + entityModel.ExpiredTime + " giây tại " + DateTime.Now.AddSeconds(entityModel.ExpiredTime).ToString("HH:mm:ss dd/MM/yyyy");
                    return new Response<RegisterCodeViewModel>(data);
                }

                return new ResponseError(HttpStatusCode.NotFound, "Không tạo được mã đăng ký user!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        //query build
        private Expression<Func<RegisterCode, bool>> BuildQuery(RegisterCodeQueryModel query)
        {
            var predicate = PredicateBuilder.New<RegisterCode>(true);

            if (!string.IsNullOrEmpty(query.Status)) predicate.And(x => x.Status.Equals(query.Status));

            if (query.CreatedOnDate != null) predicate.And(x => x.CreatedOnDate > query.CreatedOnDate);
            return predicate;
        }
    }
}
