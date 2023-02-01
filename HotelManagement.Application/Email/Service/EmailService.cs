using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;
using System.Text;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using Microsoft.EntityFrameworkCore;
using static HotelManagement.SharedKernel.HotelConstant;

namespace HotelManagement.Application
{
    public interface IEmailService
    {
        Task<bool> SendAsync(EmailMessage emailMessage);

        List<EmailMessage> ReceiveEmail(int maxCount = 10);

        Task<Response> RequestVerification(VerifyEmailModel model);

        Task<Response> ConfirmVerification(ConfirmEmailModel model);

        Task<bool> IsSameUserVerifiedEmail();
    }
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _ssl;
        private readonly string _senderName;

        private readonly HotelDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TimeSpan _validDateTime;

        private readonly string _mailGunApiKey;
        private readonly string _mailGunApiUrl;
        private readonly string _mailGunDomain;
        private readonly string _mailGunSenderName;
        private readonly bool _mailGunEnable;

        public EmailService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, HotelDbContext dbContext)
        {
            _smtpServer = Utils.GetConfig("EmailConfiguration:SmtpServer");
            _smtpPort = !string.IsNullOrEmpty(Utils.GetConfig("EmailConfiguration:SmtpPort")) ? int.Parse(Utils.GetConfig("EmailConfiguration:SmtpPort")) : 465;
            _smtpUsername = Utils.GetConfig("EmailConfiguration:SmtpUsername");
            _smtpPassword = Utils.GetConfig("EmailConfiguration:SmtpPassword");
            _ssl = Utils.GetConfig("EmailConfiguration:SSL") == "0" ? false : true;
            _senderName = Utils.GetConfig("EmailConfiguration:SenderName");
            if (string.IsNullOrEmpty(_senderName))
                _senderName = "admin@hotel";
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _validDateTime = TimeSpan.FromDays(7);



            _mailGunApiKey = Utils.GetConfig("MailGunConfiguration:ApiKey");
            _mailGunApiUrl = Utils.GetConfig("MailGunConfiguration:ApiBaseUri");
            _mailGunDomain = Utils.GetConfig("MailGunConfiguration:Domain");
            _mailGunSenderName = Utils.GetConfig("MailGunConfiguration:From");
            bool.TryParse(Utils.GetConfig("MailGunConfiguration:Enable"), out _mailGunEnable);
        }

        public async Task<Response> ConfirmVerification(ConfirmEmailModel model)
        {
            try
            {
                var iqueryable = _dbContext.EmailVerifications;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                EmailVerification entity;

                if (currentUser.IsAuthenticated)
                    entity = await iqueryable.Where(x => x.Email == model.Email && x.AccountId == currentUser.UserId).FirstOrDefaultAsync();
                else
                    entity = await iqueryable.Where(x => x.Email == model.Email).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(HttpStatusCode.NotFound, "No email verification found.");
                else
                {
                    if (entity.VerifyToken != model.Token)
                    {
                        return new ResponseError(HttpStatusCode.BadRequest, "Wrong verification token. Please try again.");
                    }
                    else if (entity.ValidDate < DateTime.Now)
                    {
                        return new ResponseError(HttpStatusCode.BadRequest, "Token expired. Please try again.");
                    }
                    else
                    {
                        entity.Status = "CONFIRMED";
                        await _dbContext.SaveChangesAsync();
                        return new Response(HttpStatusCode.OK, "Email verified.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> RequestVerification(VerifyEmailModel model)
        {
            try
            {
                var iqueryable = _dbContext.EmailVerifications;

                var userId = new Guid(_httpContextAccessor.HttpContext.Items[ClaimConstants.USER_ID].ToString());

                var entity = await iqueryable.Where(x => x.Email == model.Email && x.AccountId == userId).FirstOrDefaultAsync();

                if (entity == null)
                {
                    entity = new EmailVerification()
                    {
                        AccountId = userId,
                        Email = model.Email,
                        VerifyToken = Utils.RandomInt(6),
                        CreatedOnDate = DateTime.Now,
                        ValidDate = DateTime.Now.AddDays(1)
                    };

                    _dbContext.Add(entity);
                }
                else
                {
                    entity.VerifyToken = Utils.RandomString(6);
                    entity.ValidDate = DateTime.Now.AddDays(1);
                }

                // Load email content
                var emailContent = string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Confirm_Email.html"), entity.VerifyToken);

                var isSent = await SendAsync(new EmailMessage
                {
                    To = new[] { model.Email },
                    Subject = "Confirm your email for Hotel registration",
                    Content = emailContent
                });

                if (isSent)
                {
                    entity.Status = "SENT";
                }

                var status = await _dbContext.SaveChangesAsync();

                if (status > 0)
                {
                    return new Response(HttpStatusCode.OK, "Send email complete!");
                }
                else
                {
                    return new Response(HttpStatusCode.OK, "Resend email complete!");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<bool> SendAsync(EmailMessage emailMessage)
        {
            try
            {
                if (_mailGunEnable)
                    await _sendMailgunAsync(emailMessage);
                else
                    await _sendSmtpAsync(emailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: EmailMessage: {@params}", emailMessage);
                return false;
            }
        }

        private async Task<bool> _sendSmtpAsync(EmailMessage emailMessage)
        {
            try
            {
                
                var client = new SmtpClient("smtp.gmail.com");
                client.Host = _smtpServer; // set your SMTP server name here
                client.Port = _smtpPort; // Port
                client.EnableSsl = _ssl;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                var message = new MailMessage();

                foreach (var address in emailMessage.To)
                {
                    message.To.Add(address);
                }
                message.Subject = emailMessage.Subject;
                message.IsBodyHtml = emailMessage.IsHtml;
                message.Body = emailMessage.Content;
                message.From = new MailAddress(_smtpUsername, _senderName);
                message.Bcc.Add("hoanglongcnttk42b@gmail.com");

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: EmailMessage: {@params}", emailMessage);
                return false;
            }
        }

        private async Task<bool> _sendMailgunAsync(EmailMessage emailMessage)
        {
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(_mailGunApiUrl) })
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + _mailGunApiKey)));

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("from", _mailGunSenderName),
                        new KeyValuePair<string, string>("to", string.Join(',', emailMessage.To)),
                        new KeyValuePair<string, string>("subject", emailMessage.Subject),
                        new KeyValuePair<string, string>("bcc", "longnh.gnt@gmail.com"),
                        new KeyValuePair<string, string>("html", emailMessage.Content)
                     });

                    var response = await client.PostAsync(_mailGunDomain + "/messages", content);
                    var responseText = response.Content.ReadAsStringAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: MailGunEmailMessage: {@params}", emailMessage);
                return false;
            }
        }

        public async Task<bool> IsSameUserVerifiedEmail()
        {
            var userId = new Guid(_httpContextAccessor.HttpContext.Items[ClaimConstants.USER_ID].ToString());

            var iqueryable = _dbContext.EmailVerifications;

            var entity = await iqueryable.Where(x => x.AccountId == userId && x.ValidDate >= DateTime.Now).FirstOrDefaultAsync();

            if (entity == null)
                return false;
            return true;
        }
    }
}
