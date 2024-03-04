using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.MenuAggregate;

namespace BuberDinner.Infrastructure.Persistence;

public class MenuRepository : IMenuRepository
{
    private readonly List<Menu> _menus = new();

    public void Create(Menu menu)
    {
        _menus.Add(menu);
    }
}