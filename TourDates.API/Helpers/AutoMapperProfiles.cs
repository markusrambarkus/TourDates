using System.Linq;
using AutoMapper;
using TourDates.API.Dtos;
using TourDates.API.Models;

namespace TourDates.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(source => source.DateOfBirth.CalculteAge());
                });

            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(source => source.DateOfBirth.CalculteAge());
                });


            CreateMap<Photo, PhotoForDetailDto>();
        }
    }
}