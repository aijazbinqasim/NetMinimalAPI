using NetMinimalAPI.Models;

namespace NetMinimalAPI.Data
{
    public static class CouponStore
    {
        public static IList<Coupon> Coupons = new List<Coupon>
        {
          new() { Id = 1, Name = "C001", Percent = 10, IsActive = true },
          new() { Id = 2, Name = "C002", Percent = 20, IsActive = false }
        };
    }
}
