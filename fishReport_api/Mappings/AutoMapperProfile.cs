using AutoMapper;
using FishReportApi.DTOs;
using FishReportApi.Models;

namespace FishReportApi.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Species, SpeciesDTO>().ReverseMap();


            CreateMap<FishMarket, FishMarketDTO>()
                .ForMember(dest => dest.Species, opt => opt.MapFrom(src =>
                    src.FishMarketInventory.Select(fmi => fmi.Species)))
                .ReverseMap();

            CreateMap<Species, SpeciesInventoryDTO>();
            CreateMap<FishMarket, MarketInventoryDTO>()
                .ForMember(dest => dest.Species, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.FishMarketInventory
                        .Where(fmi => fmi.Species != null)
                        .Select(fmi => context.Mapper.Map<SpeciesInventoryDTO>(fmi.Species))
                        .ToList()));
        }
    }
}