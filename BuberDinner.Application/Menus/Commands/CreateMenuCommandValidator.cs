using FluentValidation;

namespace BuberDinner.Application.Menus.Commands;

public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage("Sections are required.")
            .Must(sections => sections.All(section => section.Items.Any()))
            .WithMessage("Sections must contain at least one item.");
    }
}