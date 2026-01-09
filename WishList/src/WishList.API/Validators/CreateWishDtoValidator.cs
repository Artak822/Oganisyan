using FluentValidation;
using WishList.API.Data.Models.DTO;

namespace WishList.API.Validators;

public class CreateWishDtoValidator : AbstractValidator<CreateWishDto>
{
    public CreateWishDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Link)
            .MaximumLength(500).WithMessage("Link must not exceed 500 characters")
            .Must(BeValidUrl).WithMessage("Link must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.Link));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0")
            .When(x => x.Price.HasValue);
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

