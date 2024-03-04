using BuberDinner.Application.Menus.Commands;
using BuberDinner.Contracts.Menus;
using BuberDinner.Domain.MenuAggregate;

using Mapster;

using MenuItem = BuberDinner.Domain.MenuAggregate.Entities.MenuItem;
using MenuSection = BuberDinner.Domain.MenuAggregate.Entities.MenuSection;

namespace BuberDinner.Api.Common.Mapping;

public class MenuMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CreateMenuRequest Request, string HostId), CreateMenuCommand>()
            .Map(dest => dest.HostId, src => src.HostId)
            .Map(dest => dest, src => src.Request);

        config.NewConfig<Menu, MenuResponse>()
            .Map(dest => dest.Id, src => src.HostId.Value)
            .Map(
                dest => dest.AverageRating,
                src => src.AverageRating.NumRatings > 0 ? (float?)src.AverageRating.Value : null)
            .Map(dest => dest.HostId, src => src.HostId.Value)
            .Map(dest => dest.DinnerIds, src => src.DinnerIds.Select(dinnerId => dinnerId.Value).ToList())
            .Map(dest => dest.MenuReviewIds, src => src.MenuReviewIds.Select(menuReviewId => menuReviewId.Value).ToList());

        // Necessary so that Mapster knows how to map the MenuSectionCommand to the MenuSection entity when mapping from menu to CreateMenuResponse
        config.NewConfig<MenuSection, MenuSectionResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        // Necessary so that Mapster knows how to map the MenuItemCommand to the MenuItem entity when mapping from menu to CreateMenuResponse
        config.NewConfig<MenuItem, MenuItemResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);
    }
}