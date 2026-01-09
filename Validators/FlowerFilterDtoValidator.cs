using FluentValidation;
using WebApplication1.Models.DTO;

namespace WebApplication1.Validators;

public class FlowerFilterDtoValidator : AbstractValidator<FlowerFilterDto>
{
    public FlowerFilterDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
    }
}

