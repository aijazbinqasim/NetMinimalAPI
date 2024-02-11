using FluentValidation;
using NetMinimalAPI.Models.Dtos;

namespace NetMinimalAPI.Validations
{
    public class PutCouponDtoValidation : AbstractValidator<PutCouponDto>
    {
        public PutCouponDtoValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).ExclusiveBetween(1, 100);
        }
    }
}
