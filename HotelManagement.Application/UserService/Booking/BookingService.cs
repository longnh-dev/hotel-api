using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using HotelManagement.SharedKernel;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application
{
    public class BookingService : IBookingService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly string[] _paymentStatus = new[]
        {
            PaymentStatusConstant.UNSUCCESS,
            PaymentStatusConstant.SUCCESS
        };
        private readonly string[] _paymentMethod = new[]
        {
            PaymentConstant.InternetBanking,
            PaymentConstant.CreditCard,
            PaymentConstant.DebitCard,
            PaymentConstant.Cash
        };
        public BookingService(HotelDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IEmailService emailService )
        {
            _dbContext= dbContext;
            _mapper= mapper;
            _httpContextAccessor= httpContextAccessor;
            _emailService= emailService;
        }
        public async Task<Response> Create(BookingCreateModel model)
        {
            try
            {

                var booking = new Booking();
                
                //valid payment
                //if (!_paymentStatus.Contains(model.PaymentStatus)) return new ResponseError(HttpStatusCode.BadRequest, "Trạng thái thanh toán không hợp lệ!"); 

                if (!_paymentMethod.Contains(model.PaymentType)) return new ResponseError(HttpStatusCode.BadRequest, "Phương thức thanh toán không hợp lệ!");

                booking.Id = Guid.NewGuid();
                booking.StaffId = new Guid("D1BA08D5-0663-432C-9B17-A676DC051F3B");
                booking.NoOfPerson = model.NoOfPerson;
                booking.Checkin = model.Checkin;
                booking.Checkout = model.Checkout;
                booking.Status = BookingStatusConstant.Completed;
                booking.CreatedOnDate = DateTime.Now;
                booking.PaymentStatus = PaymentStatusConstant.UNSUCCESS;
                booking.PaymentType = model.PaymentType.ToUpper().Trim();
                booking.CititzenIdentification = model.CititzenIdentification;
                booking.FullName = model.FullName;
                booking.Email = model.Email;
                booking.PhoneNumber = model.PhoneNumber;
                booking.RoomId = model.RoomId;
                booking.CustomerId = model.CustomerId;
                var currentUser = await _dbContext.Users.Where(x => x.Id == booking.CustomerId).FirstOrDefaultAsync();
                var history = new History()
                {
                    BookingId = booking.Id,
                    CustomerId = booking.CustomerId,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedOnDate = DateTime.Now
                };

                if (currentUser.Level == RoleConstants.NormalUserLevel)
                {
                    booking.CustomerId = currentUser.Id;
                }
                else if(currentUser.Level == RoleConstants.StaffUserLevel)
                {
                    booking.CustomerId = model.CustomerId;
                    booking.StaffId = currentUser.Id;
                }

                var existedRoom = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == booking.RoomId);

                if (existedRoom?.Status != RoomStatusConstant.AvailableRoom)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, "Phòng hiện không có sẵn");
                }
                existedRoom.Status = RoomStatusConstant.UnavailableRoom;

                var bookingRoom = new RoomBooking()
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    RoomId = booking.RoomId,
                };

                #region send email thanks

                var emailContent = string.Format(System.IO.File.ReadAllText(@"Resources/EmailTemplate/Thanks.html"));

                var message = new EmailMessage
                {
                    To = new[] { booking.Email },
                    Subject = "THANKS FOR YOUR CHOOSING",
                    Content = emailContent
                };

                var sendMailResult = await _emailService.SendAsync(message);

                #endregion

                _dbContext.Add(bookingRoom);
                _dbContext.Add(booking);
                _dbContext.Add(history);
                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Tạo đặt phòng thành công");
            }
            catch(Exception ex) 
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}", model);
                return Utils.CreateExceptionResponseError(ex);
            } 
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id);

                if (booking == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Đặt phòng không tồn tại");

                _dbContext.Remove(booking); 
                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Xóa thành công đặt phòng");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(BookingQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var data = new Pagination<Booking>();
                data = await _dbContext.Bookings.Include(x => x.Customer).Include(x=>x.Staff).Include(x => x.Room).Where(predicate).GetPageAsync(query);

                var result = _mapper.Map<Pagination<Booking>, Pagination<BookingViewModel>>(data);

                if(result != null)
                {
                    foreach(var item in result.Content)
                    {
                        //fetch data
                        var bookingRoom = await _dbContext.RoomBookings.Where(x => x.BookingId == item.Id).ToListAsync();
                        foreach(var br in bookingRoom)
                        {
                            var existedRoom = await _dbContext.Rooms.Where(x => x.Id == br.RoomId).FirstOrDefaultAsync();

                            //var roomVm = new RoomViewModel()
                            //{
                            //    Id = existedRoom.Id,
                            //    Name = existedRoom.Name,
                            //    Code = existedRoom.Code,
                            //    RoomCategoryId = existedRoom.RoomCategoryId,
                            //    Status = existedRoom.Status
                            //};
                            //item.Rooms.Add(roomVm);
                        }
                    }
                }

                return new ResponsePagination<BookingViewModel>(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var booking = await _dbContext.Bookings.Include(x=>x.Customer).Include(x=>x.Staff).FirstOrDefaultAsync(x => x.Id == id);

                if (booking == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Đơn đặt phòng không tồn tại");

                var data = _mapper.Map< Booking, BookingViewModel >(booking);

                return new Response<BookingViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Update(BookingUpdateModel model, Guid id)
        {
            try
            {
                var booking = await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id);
                var staff = await _dbContext.Users.Where(x => x.Id == model.StaffId).FirstOrDefaultAsync();
                if (staff.Level != 4)
                    return new Response(HttpStatusCode.BadRequest, "Người dùng cập nhật không phải là nhân viên khách sạn");


                if (booking == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Vật dụng không tồn tại");

                booking = _mapper.Map<BookingUpdateModel, Booking>(model);

                return new Response(HttpStatusCode.OK, "cập nhật thông tin đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params:Model: {@model}, Id: {@id}", model, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdatePayment(Guid id)
        {
            try
            {
                var booking = await _dbContext.Bookings.Include(x => x.Customer).Include(x => x.Staff).FirstOrDefaultAsync(x => x.Id == id);
                var room = await _dbContext.Rooms.Where(x => x.Id == booking.RoomId).FirstOrDefaultAsync();

                if (booking == null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Đơn đặt phòng không tồn tại");

                if (booking.PaymentStatus == PaymentStatusConstant.SUCCESS)
                    return new Response(HttpStatusCode.BadRequest, "You have completed the previous payment");

                booking.PaymentStatus = PaymentStatusConstant.SUCCESS;

                //when user payment booking, room's status will return to Available
                room.Status = RoomStatusConstant.AvailableRoom;


                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Paid!");
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<Booking, bool>> BuildQuery(BookingQueryModel query)
        {
            var predicate = PredicateBuilder.New<Booking>(true);

            if (!query.CustomerId.HasValue)
            {
                predicate.And(x => x.CustomerId == query.CustomerId);
            }
            if (query.StaffId.HasValue)
            {
                predicate.And(x => x.StaffId == query.StaffId);
            }

            return predicate;
        }
    }
}
