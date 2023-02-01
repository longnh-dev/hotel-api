using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain;

public interface IUserService
{
    Task<Response> GetPageAsync(UserQueryModel query);

    Task<Response> RegisterAsync(UserRegisterRequestModel model);

    Task<Response> ResetPassword(string resetPasswordToken, string password);

    Task<Response> VerifyEmail(string email, string token);

    Task<Response> CreateAsync(UserCreateModel model);

    Task<Response> UpdateAsync(Guid id, UserUpdateModel model);

    Task<Response> UpdateInfoAsync(Guid id, UserInfoUpdateModel model);

    Task<Response> DeleteAsync(Guid id);

    Task<Response> DeleteRangeAsync(List<Guid> listId);

    Task<Response> FindAsync(Guid id);

    Task<Response> ForgotPassword(string email);

    Task<IActionResult> BuildToken(Guid userId, bool isRememberMe);
    Response Authentication(string userName, string password);

    Task<Response> ChangePasswordAsync(Guid id, string oldPassword, string password, string confirmPassword);
}
