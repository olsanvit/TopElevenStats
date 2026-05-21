using FluentValidation;
using SharedServices.Models.TopEleven;

namespace BlazorTopEleven.Web.Validators;

public class TopElevenPlayerValidator : AbstractValidator<TopElevenPlayer>
{
    public TopElevenPlayerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Jméno je povinné.")
            .MaximumLength(128).WithMessage("Jméno může mít nejvýše 128 znaků.");

        RuleFor(x => x.Ovr)
            .InclusiveBetween(1, 100).WithMessage("OVR musí být mezi 1 a 100.");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(0).WithMessage("Věk musí být kladné číslo.");

        RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("Role jsou povinné.")
            .MaximumLength(64).WithMessage("Role mohou mít nejvýše 64 znaků.");

        RuleFor(x => x.Citizenship)
            .MaximumLength(64).WithMessage("Občanství může mít nejvýše 64 znaků.")
            .When(x => x.Citizenship is not null);

        RuleFor(x => x.SpecialAbility)
            .MaximumLength(128).WithMessage("Spec. schopnost může mít nejvýše 128 znaků.")
            .When(x => x.SpecialAbility is not null);
    }
}
