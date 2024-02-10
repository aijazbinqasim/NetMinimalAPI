using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using NetMinimalAPI;
using NetMinimalAPI.Data;
using NetMinimalAPI.Models;
using NetMinimalAPI.Models.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all coupons");
    return Results.Ok(CouponStore.Coupons);

}).WithName("GetCouponList").Produces<IEnumerable<Coupon>>(200);

app.MapGet("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));

}).WithName("GetCouponById").Produces<Coupon>(200);

app.MapPost("/api/coupon", async (IMapper _mapper,
    IValidator<PostCouponDto> _validation, PostCouponDto postCouponDto) =>
{
    var vresult = await _validation.ValidateAsync(postCouponDto);

    if (!vresult.IsValid)
    {
        //return Results.BadRequest(vresult.Errors.FirstOrDefault().ToString()); // get first error msg
        return Results.BadRequest(vresult.Errors); // get all error msgs
    }

    if (CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == postCouponDto.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name already Exists!");
    }

    var coupon = _mapper.Map<Coupon>(postCouponDto);

    coupon.Id = CouponStore.Coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    CouponStore.Coupons.Add(coupon);

    var getCouponDto = _mapper.Map<GetCouponDto>(coupon);

    return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id }, getCouponDto);

}).WithName("CreateCoupon")
  .Accepts<Coupon>("application/json")
  .Produces<Coupon>(201)
  .Produces(400);


app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{

});

app.Run();
