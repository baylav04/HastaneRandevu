using AutoMapper;
using HastaneRandevu.Models;
using HastaneRandevu.DTOs;

namespace HastaneRandevu.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Hasta, HastaDTO>(); // Model → DTO eşleştirmesi
        }
    }
}