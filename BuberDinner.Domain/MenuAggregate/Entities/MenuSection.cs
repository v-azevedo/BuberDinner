using BuberDinner.Domain.Common.Models;
using BuberDinner.Domain.MenuAggregate.ValueObjects;

namespace BuberDinner.Domain.MenuAggregate.Entities;

public sealed class MenuSection : Entity<MenuSectionId>
{
    public string Name { get; }
    public string Description { get; }
    public IReadOnlyList<MenuItem> Items { get; }

    private MenuSection(MenuSectionId menuSectionId, string name, string description, List<MenuItem> items)
        : base(menuSectionId)
    {
        Name = name;
        Description = description;
        Items = items;
    }

    public static MenuSection Create(
        string name,
        string description,
        List<MenuItem> items)
    {
        return new(
            MenuSectionId.CreateUnique(),
            name,
            description,
            items ?? new());
    }
}