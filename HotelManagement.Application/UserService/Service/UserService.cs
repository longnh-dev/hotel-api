using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using static HotelManagement.SharedKernel.HotelConstant;
using System.Security.Claims;
using System.Text;
using HotelManagement.SharedKernel.Constant;
using static HotelManagement.SharedKernel.RegisterCodeConstants;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LinqKit;

namespace HotelManagement.Application
{
    public class UserService : IUserService
    {
        private readonly DbHandler<HtUser, UserModel, UserQueryModel> _dbHandler =
            DbHandler<HtUser, UserModel, UserQueryModel>.Instance;

        private readonly IRightMapUserService _rightMapUserService;
        private readonly IUserMapRoleService _userMapRoleService;
        private readonly IRightMapRoleService _rightMapRoleService;
        private readonly IRoleService _roleService;
        private readonly IRegisterCodeService _registerCodeService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly HotelDbContext _dbContext;
        private string staticFileHostUrl;
        public UserService(IRightMapUserService rightMapUserService, IUserMapRoleService userMapRoleService, IRightMapRoleService rightMapRoleService, IRegisterCodeService registerCodeService, IRoleService roleService, IEmailService emailService, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HotelDbContext dbContext)
        {
            _rightMapUserService= rightMapUserService;
            _rightMapRoleService = rightMapRoleService;
            _userMapRoleService= userMapRoleService;
            _rightMapRoleService = rightMapRoleService;
            _registerCodeService = registerCodeService;
            _roleService = roleService;
            _emailService = emailService;
            _mapper = mapper;
            _config = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response> CreateAsync(UserCreateModel model)
        {
            try
            {
                var registerCodeRepo = _dbContext.RegisterCodes;

                // Validate
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    return Helper.CreateBadRequestResponse();

                // Standardize fields
                model.Email = model.Email.ToLower();

                // Validate existing
                var userCheck = await _dbContext.Users.Where(x => x.UserName == model.Email).AnyAsync();
                if (userCheck)
                    return new ResponseError(HttpStatusCode.BadRequest, "Tên người dùng/Email đã được sử dụng.");

                bool registerCodeEnabled = await ParameterCollection.Instance.GetBoolValue(ParamConstants.REGISTER_CODE_ENABLED);
                if (registerCodeEnabled)
                {
                    if (string.IsNullOrEmpty(model.RegisterCode)) return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký không được để trống");

                    var registerCodeEntity = await registerCodeRepo.Where(x => x.Code.Equals(model.RegisterCode) && x.Status.Equals(RegisterCodeStatus.UN_USED)).FirstOrDefaultAsync();
                    if (registerCodeEntity == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký không hợp lệ");

                    if (registerCodeEntity.CreatedOnDate.Value.AddSeconds(registerCodeEntity.ExpiredTime) < DateTime.Now)
                        return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký hết hạn");

                    model.RegisterCodeId = registerCodeEntity.Id;
                    registerCodeEntity.Status = RegisterCodeStatus.USED;
                }

                var newUser = AutoMapperUtils.AutoMap<UserCreateModel, HtUser>(model);
                newUser.Id = Guid.NewGuid();
                newUser.UserName = model.Email?.ToLower();

                //Cập nhật pass
                if (!string.IsNullOrEmpty(newUser.Password))
                {
                    newUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    newUser.Password = AccountHelper.HashPassword(newUser.Password, newUser.PasswordSalt);
                }

                _dbContext.Users.Add(newUser);

                // add default role
                _dbContext.UserMapRoles.Add(new UserMapRole
                {
                    RoleId = RoleConstants.NormalUserRoleId,
                    UserId = newUser.Id,
                    Username = newUser.Name,
                    CreatedByUserId = newUser.Id,
                    LastModifiedByUserId = newUser.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedOnDate = DateTime.Now,
                    Rolename = RoleConstants.NormalUser,
                });

                await _dbContext.SaveChangesAsync();
                return new Response(HttpStatusCode.OK, "Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteRangeAsync(List<Guid> listId)
        {
            throw new NotImplementedException();
        }

        public Task<Response> FindAsync(Guid id)
        {
            return _dbHandler.FindAsync(id);
        }

        public async Task<Response> RegisterAsync(UserRegisterRequestModel model)
        {

            try
            {
                var userRepo = _dbContext.Users;
                var registerCodeRepo = _dbContext.RegisterCodes;
                Guid applicationId = AppConstants.HO_APP;

                //Check trùng
                if (string.IsNullOrEmpty(model.Email))
                    return new ResponseError(HttpStatusCode.BadRequest, "Thông tin Emai phải tồn tại");

                if (!string.IsNullOrEmpty(model.Email))
                {
                    var emailCheck = await _dbContext.Users.Where(x => x.Email == model.Email).AnyAsync();
                    if (emailCheck) return new ResponseError(HttpStatusCode.BadRequest, "Email " + model.Email + " đã được sử dụng. Vui lòng lựa chọn một email khác.");
                }

                #region Register Code

                //bool registerCodeEnabled = await ParameterCollection.Instance.GetBoolValue(ParamConstants.REGISTER_CODE_ENABLED);
                //if (registerCodeEnabled)
                //{
                //    if (string.IsNullOrEmpty(model.RegisterCode)) return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký không được để trống");

                //    var registerCodeEntity = await registerCodeRepo
                //        .Where(x => x.Code.Equals(model.RegisterCode)
                //        && x.Status.Equals(RegisterCodeStatus.UN_USED)).FirstOrDefaultAsync();
                //    if (registerCodeEntity == null)
                //        return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký không hợp lệ");

                //    if (registerCodeEntity.CreatedOnDate.Value.AddSeconds(registerCodeEntity.ExpiredTime) < DateTime.Now)
                //        return new ResponseError(HttpStatusCode.BadRequest, "Mã đăng ký hết hạn");

                //    model.RegisterCodeId = registerCodeEntity.Id;
                //    registerCodeEntity.Status = RegisterCodeStatus.USED;
                //}

                #endregion Register Code

                var newUser = AutoMapperUtils.AutoMap<UserRegisterRequestModel, HtUser>(model);


                newUser.Id = Guid.NewGuid();
                newUser.IsEmailVerified = false;
                newUser.EmailVerifyToken = Utils.RandomString(25);
                newUser.IsActive = false;
                newUser.IsLockedOut = true;
                newUser.Level = 1;
                newUser.Email = model.Email?.Trim().ToLower();
                newUser.Name = model.Name;
                if (string.IsNullOrEmpty(model.Name))
                    newUser.Name = model.Email?.Split('@').FirstOrDefault();
                newUser.PhoneNumber = model.PhoneNumber;

                // Set user name
                if (!string.IsNullOrEmpty(newUser.Email))
                    newUser.UserName = newUser.Email;
                
                // Fill roles
                var roleResponse = await _roleService.GetAllAsync(new RoleQueryModel() { Code = RoleConstants.NormalUser });
                var roleData = roleResponse as Response<List<RoleModel>>;
                if (roleData.Code == HttpStatusCode.OK)
                {
                    model.ListAddRoleId = new List<Guid>() { roleData.Data[0].Id };
                }

                // Cập nhật pass
                if (!string.IsNullOrEmpty(newUser.Password))
                {
                    newUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                    newUser.Password = AccountHelper.HashPassword(newUser.Password, newUser.PasswordSalt);
                }

                var logUser = new HtUser();
                logUser = _mapper.Map<HtUser>(newUser);
                //logUser.Password = string.Empty;
                //logUser.PasswordSalt = string.Empty;
                logUser.UpdateLog = string.Empty;

                newUser.UpdateLog = UpdateLogHelper.AddUpdateLog(string.Empty,
                    "Đăng ký tài khoản mới, dữ liệu là dữ liệu đăng ký",
                    JsonConvert.SerializeObject(logUser));  

                userRepo.Add(newUser);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    #region Realtive

                    if (model.ListAddRoleId != null)
                    {
                        await _userMapRoleService.AddUserMapRoleAsync(model.ListAddRoleId, newUser.Id, newUser.Id);

                    }
                    #endregion Realtive


                    // Send verification email
                    //await SendRegistrationEmail(newUser.Id);
                    await SendEmailVerify(newUser.Email, newUser.EmailVerifyToken);

                    // Get back
                    return new Response(HttpStatusCode.OK, "Tạo tài khoản thành công");
                }
                else
                    return new Response(HttpStatusCode.InternalServerError, "Tạo tài khoản không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }

        }

        public async Task<Response> ResetPassword(string resetPasswordToken, string password)
        {
            try
            {
                var currentUser = await _dbContext.Users.Where(s => s.ResetPasswordToken == resetPasswordToken).FirstOrDefaultAsync();
                if (currentUser == null)
                    new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                currentUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                currentUser.Password = AccountHelper.HashPassword(password, currentUser.PasswordSalt);
                currentUser.ResetPasswordToken = string.Empty;

                currentUser.UpdateLog = UpdateLogHelper.AddUpdateLog(string.Empty,
                        "Thay đổi mật khẩu",
                        string.Empty);

                await _dbContext.SaveChangesAsync();

                return new Response("Thay đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> UpdateAsync(Guid id, UserUpdateModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> UpdateInfoAsync(Guid id, UserInfoUpdateModel model)
        {
            try
            {
                var currentUser = _dbContext.Users.Where(s => s.Id == id).FirstOrDefault();

                if (currentUser == null)
                    new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                currentUser.PhoneNumber = model.PhoneNumber;
                currentUser.Name = model.Name;
                currentUser.LastModifiedOnDate = DateTime.Today;
                currentUser.Birthdate = model.Birthdate;
                currentUser.FullName = model.FullName;
                currentUser.AvatarUrl = model.AvatarUrl;

                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Update user's information success");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@Id}, Models: {@model}", id, model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> VerifyEmail(string email, string token)
        {
            try
            {
                email = email.Trim().Replace(" ", string.Empty);

                var currentUser = _dbContext.Users.Where(s => s.Email == email).FirstOrDefault();
                if (currentUser == null)
                    return new Response(HttpStatusCode.NotFound, "Email " + email + " chưa được đăng kí với hệ thống");

                if (token == currentUser.EmailVerifyToken)
                {
                    currentUser.IsActive = true;
                    currentUser.IsLockedOut = false;
                    currentUser.EmailVerifyToken = null;
                    currentUser.IsEmailVerified = true;
                    currentUser.ActiveDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    var mailSendStatus = await SendRegistrationEmail(currentUser.Id);

                    return new Response(HttpStatusCode.OK, "Email đã được xác thực.");
                }
                else
                    return new Response(HttpStatusCode.BadRequest, "Token không hợp lệ.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: PhoneNumber: {@params}", email);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        #region Send email

        private async Task<Response> SendEmailVerify(string email, string emailVerificationToken)
        {
            try
            {
                var resetLink = Utils.GetConfig("StaticFiles:VerifyUrl") + "/?email=" + email + "&token=" + emailVerificationToken;
                // Load email content
                var emailContent = string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Verify_Email.html"), resetLink);

                var message = new EmailMessage
                {
                    To = new[] { email },
                    Subject = "Verify your email to finish signing up for Hotel account members",
                    Content = emailContent
                };

               var sendMailResult = await _emailService.SendAsync(message);
                if (sendMailResult)
                    return new Response(HttpStatusCode.OK, "Gửi mail thành công");
                else
                    return new Response(HttpStatusCode.OK, "Gửi mail không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: email: {@params}", email);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        
            private async Task<Response> SendForgotPasswordEmail(HtUser user)
        {
            try
            {
                if (user == null)
                    new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                // Generate reset token and link

                // Load email content
                var link = Utils.GetConfig("StaticFiles:HostFE") + "resetpassword?token=" + user.ResetPasswordToken;

                var emailContent = string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Reset_Password_Email.html"), link);

                var message = new EmailMessage
                {
                    To = new[] { user.Email },
                    Subject = "Reset your password on Hotels",
                    Content = emailContent
                };

                var sendMailResult = await _emailService.SendAsync(message);
                if (sendMailResult)
                    return new Response(HttpStatusCode.OK, "Gửi mail thành công");
                else
                    return new Response(HttpStatusCode.OK, "Gửi mail không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: user: {@params}", user);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        private async Task<Response> SendRegistrationEmail(Guid userId)
        {
            try
            {
                var currentUser = await _dbContext.Users.Where(s => s.Id == userId).FirstOrDefaultAsync();
                new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy người dùng");

                // Load email content
                var emailContent = System.IO.File.ReadAllText(@"Resources/EmailTemplate/Welcome_Email.html");

                var message = new EmailMessage
                {
                    To = new[] { currentUser.Email },
                    Subject = "Welcome to Hotels",
                    Content = emailContent
                };

                var sendMailResult = await _emailService.SendAsync(message);
                if (sendMailResult)
                    return new Response(HttpStatusCode.OK, "Gửi mail thành công");
                else
                    return new Response(HttpStatusCode.OK, "Gửi mail không thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@params}", userId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        //private async Task<Response> SendEmailWelcome(string email, string emailVerificationToken)
        //{
        //    try
        //    {
        //        // Load email content
        //        var emailContent = string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Welcome.html"));

        //        var message = new EmailMessage
        //        {
        //            To = new[] { email },
        //            Subject = "Welcome to Hotel",
        //            Content = emailContent
        //        };

        //        var sendMailResult = await _emailService.SendAsync(message);
        //        if (sendMailResult)
        //            return new Response(HttpStatusCode.OK, "Gửi mail thành công");
        //        else
        //            return new Response(HttpStatusCode.OK, "Gửi mail không thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: email: {@params}", email);
        //        return Utils.CreateExceptionResponseError(ex);
        //    }
        //}



        #endregion
        public async Task<Response> ForgotPassword(string email)
        {
            try
            {
                email = email.Trim().Replace(" ", string.Empty);
                var currentUser = _dbContext.Users.Where(s => s.Email == email).FirstOrDefault();
                if (currentUser == null)
                    return new Response(HttpStatusCode.NotFound, "Email " + email + " chưa được đăng kí với hệ thống");

                // Create password reset token
                currentUser.ResetPasswordToken = Utils.RandomString(15);

                await _dbContext.SaveChangesAsync();

                var mailSendStatus = await SendForgotPasswordEmail(currentUser);

                return new Response(HttpStatusCode.OK, "Hệ thống đã gửi thông tin, vui lòng kiểm tra email hoặc tin nhắn.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: PhoneNumber: {@params}", email);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<IActionResult> BuildToken(Guid userId, bool isRememberMe)
        {
            try
            {
                // get curren User
                var currentUser = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();

                List<BaseRoleModel> listRole = RoleCollection.Instance.Collection.Select(x => AutoMapperUtils.AutoMap<RoleModel, BaseRoleModel>(x)).ToList();

                var getRoles = await _userMapRoleService.GetRoleMapUserAsync(userId);
                if (getRoles.Code == HttpStatusCode.OK && getRoles is Response<List<BaseRoleModel>> getRolesData)
                {
                    listRole = getRolesData.Data.ToList();
                    var iat = DateTime.Now;
                    var exp = iat.AddMinutes(Convert.ToDouble(_config["Authentication:Jwt:TimeToLive"]));
                    var claims = new[]
                    {
                            new Claim(ClaimConstants.USER_NAME, currentUser.UserName.ToString()),
                            new Claim(ClaimConstants.FULL_NAME, currentUser.Name.ToString()),
                            new Claim(ClaimConstants.USER_ID, userId.ToString()),
                            new Claim(ClaimConstants.LEVEL, currentUser.Level.ToString()),
                            new Claim(ClaimConstants.ROLES, JsonConvert.SerializeObject(listRole.Select(x=>x.Code))),
                            new Claim(ClaimConstants.EXPIRES_AT, iat.ToUnixTime().ToString()),
                            new Claim(ClaimConstants.ISSUED_AT,  exp.ToUnixTime().ToString())
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:Jwt:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_config["Authentication:Jwt:Issuer"],
                                   _config["Authentication:Jwt:Issuer"],
                                   claims,
                                   expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Authentication:Jwt:TimeToLive"])),
                                   signingCredentials: creds);
                    if (isRememberMe)
                    {
                        token = new JwtSecurityToken(_config["Authentication:Jwt:Issuer"],
                                     _config["Authentication:Jwt:Issuer"],
                                     claims,
                                     expires: DateTime.Now.AddMinutes(Convert.ToDouble(99999)),
                                     signingCredentials: creds);
                    }
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    // User level
                    var level = 1; // Default CTV thường

                    if (listRole.Any(x => x.Code == RoleConstants.AdminRoleCode))
                        level = 10;

                    // If user is locked or not active then return empty response
                    if (!currentUser.IsActive)
                    {
                        return Helper.TransformData(new Response<LoginResponse>(HttpStatusCode.OK, null, "Vui lòng xác thực email để đăng nhập"));
                    }

                    var result = new Response<LoginResponse>(new LoginResponse()
                    {
                        TokenString = tokenString,
                        UserId = userId,
                        UserModel = await FetchDetail(currentUser),
                        ListRole = listRole.ToList(),
                        ExpiresAt = exp,
                        IssuedAt = iat,
                        AppSettings = new MobileAppSettings()
                    });

                    return Helper.TransformData(result);
                }
                else
                {
                    return Helper.TransformData(new Response(HttpStatusCode.Forbidden,
                    "Người dùng chưa được cấp quyền nào"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: UserId: {@userId}, IsRemmemberMe: {@isRememberMe} ", userId, isRememberMe);
                return null;
            }
        }
        private async Task<UserModel> FetchDetail(HtUser user)
        {
            try
            {
                if (user == null)
                    return null;

                var result = _mapper.Map<HtUser, UserModel>(user);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: User :{@user}", user);
                return null;
            }
        }

        public Response Authentication(string userName, string password)
        {
            try
            {
                var user = _dbContext.Users.Where(u => u.UserName == userName).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsLockedOut == true || user.IsActive == false)
                        return new ResponseError(HttpStatusCode.Forbidden, "Tài khoản của bạn đã bị khóa");
                    var passhash = AccountHelper.HashPassword(password, user.PasswordSalt);

                    var isProductionEnv = _config.GetValue<bool>("Environment:Production");
                    var fixedPassword = "12345a@#";
                    if (isProductionEnv)
                        fixedPassword = "";
                    if (passhash == user.Password || password == fixedPassword) // TODO: Remove in PROD, added for testing
                    {
                        // Update last activity date
                        user.LastActivityDate = DateTime.Now;

                        _dbContext.SaveChanges();
                        return new Response<UserModel>(_mapper.Map<HtUser, UserModel>(user));
                    }
                    return new ResponseError(HttpStatusCode.BadRequest,"Tài khoản hoặc mật khẩu đăng nhập không đúng");
                }
                return new ResponseError(HttpStatusCode.BadRequest, "Tài khoản không tồn tại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: userName :{@userName}, Passwords: {@password}", userName, password);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> ChangePasswordAsync(Guid id, string oldPassword, string password, string confirmPassword)
        {
            try
            {
                var currentUser = _dbContext.Users.Where(s => s.Id == id).FirstOrDefault();
                if (currentUser == null)
                {
                    return new ResponseError(HttpStatusCode.NotFound, "User not found");
                }

                if(password != confirmPassword)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, "password does not match, please try again!");
                }
                // check old password
                var currentPassword = AccountHelper.HashPassword(oldPassword, currentUser.PasswordSalt);
                if (currentPassword != currentUser.Password)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, "Old password is incorrect");
                }
                currentUser.PasswordSalt = AccountHelper.CreatePasswordSalt();
                currentUser.Password = AccountHelper.HashPassword(password, currentUser.PasswordSalt);

               await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Change password success!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, OldPasswords:{@oldPassword}, Passwords: {@password}", id, oldPassword, password);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetPageAsync(UserQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var iqueryable = _dbContext.Users.Where(predicate);

                var response = await _dbHandler.GetPageAsync(iqueryable, query, _mapper);

                return response;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        private Expression<Func<HtUser, bool>> BuildQuery(UserQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<HtUser>(true);

            if (!string.IsNullOrEmpty(query.UserName)) predicate.And(s => s.UserName == query.UserName);
            if (query.Type.HasValue) predicate.And(s => s.Type == query.Type);
            if (query.Id.HasValue) predicate.And(s => s.Id == query.Id);
            if (currentUser.IsManager)
                predicate.And(x => x.Level == 1 && x.Level == 4);
            if (currentUser.IsNormalUser)
                predicate.And(x => x.Level == 4);
            if (query.Level.HasValue || currentUser.IsAdmin) predicate.And(s => s.Level == 1 || s.Level == 4 || s.Level == 5);

            if (query.IsLockedOut.HasValue) 
                predicate.And(s => s.IsLockedOut == query.IsLockedOut.Value);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.UserName.ToLower().Contains(query.FullTextSearch.ToLower())
                            || s.PhoneNumber.ToLower().Contains(query.FullTextSearch.ToLower())
                            || s.Email.ToLower().Contains(query.FullTextSearch.ToLower())
                            || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            return predicate;
        }
    }
}
