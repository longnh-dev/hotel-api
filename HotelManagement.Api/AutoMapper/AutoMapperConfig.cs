using AutoMapper;
using HotelManagement.Domain;
using HotelManagement.Domain.Common;
using System.Data.Common;

namespace HotelManagement.Api
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DatabaseTableToViewModelMapping());
                cfg.AddProfile(new ViewModelToDatabaseTableMapping());
            });
        }
    }
    public class DatabaseTableToViewModelMapping : Profile
    {
        public DatabaseTableToViewModelMapping()
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));
            CreateMap<Parameter, ParameterModel>();
            CreateMap<HtUser, UserModel>();
            CreateMap<HtUser, UserHistoryModel>();
            CreateMap<RoomCategory, RoomCategoryViewModel>();
            CreateMap<Booking, BookingInHistoryModel>();
            CreateMap<Booking, BookingViewModel>().ForMember(dest => dest.Staff, x=>x.MapFrom(src=>src.Staff))
                .ForMember(dest => dest.Customer, x => x.MapFrom(src => src.Customer))
                .ForMember(dest=>dest.Rooms, x=>x.MapFrom(src => src.Room));
            CreateMap<Item, ItemViewModel>().ForMember(dest => dest.ItemStorage, x => x.MapFrom(src => src.ItemStorage)).ForMember(dest => dest.Room, x => x.MapFrom(src => src.Room));
            CreateMap<ItemStorage, ItemStorageViewModel>();

            CreateMap<Room, RoomViewModel>().ForMember(dest => dest.RoomCategory, x => x.MapFrom(src => src.RoomCategory));
            CreateMap<History, HistoryViewModel>().ForMember(dest=>dest.Customer, x=>x.MapFrom(src => src.Customer)).ForMember(dest=>dest.Booking, x=>x.MapFrom(src => src.Booking));
        }

    }
    public class ViewModelToDatabaseTableMapping : Profile
    {
        public ViewModelToDatabaseTableMapping()
        {
            CreateMap<RoomCreateModel, Room>();
            CreateMap<RoomUpdateModel, Room>();
            CreateMap<RoomCategoryCreateModel, RoomCategory>();
            CreateMap<RoomCategoryUpdateModel, RoomCategory>();
            CreateMap<ItemCreateModel, Item>();
            CreateMap<ItemUpdateModel, Item>();
            CreateMap<BookingCreateModel, Booking>();
            CreateMap<BookingUpdateModel, Booking>();
            CreateMap<ItemStorageCreateModel, ItemStorage>();
            CreateMap<ItemStorageUpdateModel, ItemStorage>();
            CreateMap<ContactCreateModel, Contact>();

        }
    }
}
