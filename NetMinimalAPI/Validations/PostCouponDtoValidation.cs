using FluentValidation;
using NetMinimalAPI.Models.Dtos;

namespace NetMinimalAPI.Validations
{
    public class PostCouponDtoValidation : AbstractValidator<PostCouponDto>
    {
        public PostCouponDtoValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).ExclusiveBetween(1, 100);
        }
    }
}
