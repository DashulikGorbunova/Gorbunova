using FluentValidation;
using WebApplication1.Models.DTO;

namespace WebApplication1.Validators;

public class FlowerCreateDtoValidator : AbstractValidator<FlowerCreateDto>
{
    public FlowerCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.Season)
            .MaximumLength(50).WithMessage("Season must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Season));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}

