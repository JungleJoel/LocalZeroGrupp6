using backend.Models.DTOs;
using backend.Models.Entities;
using Mapster;

namespace backend.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserDTO>()
                .Map(dest => dest.EcoPoints,
                     src => src.EcoPointTransactions.Sum(t => t.Amount))
                .Map(dest => dest.IsCommunityManager,
                     src => src.CommunityResidents
                               .Where(r => r.UserId == src.Id)
                               .Select(r => (bool?)r.IsManager)
                               .FirstOrDefault())
                .Map(dest => dest.Community,
                     src => src.CommunityResidents
                         .Select(cr => cr.Community)
                         .FirstOrDefault());

            config.NewConfig<Community, CommunityDTO>()
                .Map(dest => dest.EcoPoints,
                     src => src.EcoPointTransactions.Sum(t => t.Amount))
                .Map(dest => dest.ResidentsCount,
                     src => src.CommunityResidents.Count);
        }
    }
}
