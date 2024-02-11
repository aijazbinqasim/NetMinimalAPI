using AutoMapper;
using FluentValidation;
using NetMinimalAPI;
using NetMinimalAPI.Data;
using NetMinimalAPI.Models;
using NetMinimalAPI.Models.Dtos;
using System.Net;

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
    var response = new APIResponse()
    {
        IsSuccess = true,
        StatusCode = HttpStatusCode.OK,
        Result = CouponStore.Coupons
    };

    return Results.Ok(response);

}).WithName("GetCouponList").Produces<APIResponse>(200);

app.MapGet("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    var response = new APIResponse()
    {
        IsSuccess = true,
        StatusCode = HttpStatusCode.OK,
        Result = CouponStore.Coupons.FirstOrDefault(c => c.Id == id)
    };

    return Results.Ok(response);

}).WithName("GetCouponById").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (IMapper _mapper,
    IValidator<PostCouponDto> _validation, PostCouponDto postCouponDto) =>
{
    var response = new APIResponse()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    var vresult = await _validation.ValidateAsync(postCouponDto);

    if (!vresult.IsValid)
    {
        response.ErrorMsgs.Add(vresult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response); 
    }

    if (CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == postCouponDto.Name.ToLower()) != null)
    {
        response.ErrorMsgs.Add("Coupon Name already Exists!");
        return Results.BadRequest(response);
    }

    var coupon = _mapper.Map<Coupon>(postCouponDto);

    coupon.Id = CouponStore.Coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    CouponStore.Coupons.Add(coupon);

    var getCouponDto = _mapper.Map<GetCouponDto>(coupon);

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    response.Result = getCouponDto;

    return Results.Ok(response);

    //return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id }, getCouponDto);

}).WithName("CreateCoupon")
  .Accepts<PostCouponDto>("application/json")
  .Produces<APIResponse>(201)
  .Produces(400);


app.MapPut("/api/coupon", async (IMapper _mapper,
    IValidator<PutCouponDto> _validation, PutCouponDto putCouponDto) =>
{
    var response = new APIResponse()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    var vresult = await _validation.ValidateAsync(putCouponDto);

    if (!vresult.IsValid)
    {
        response.ErrorMsgs.Add(vresult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    var couponFromStore = CouponStore.Coupons.FirstOrDefault(c => c.Id == putCouponDto.Id);
    couponFromStore.Name = putCouponDto.Name;
    couponFromStore.Percent = putCouponDto.Percent;
    couponFromStore.IsActive = putCouponDto.IsActive;
    couponFromStore.LastUpdated = DateTime.Now;

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    response.Result = _mapper.Map<GetCouponDto>(couponFromStore);

    return Results.Ok(response);
}).WithName("UpdateCoupon")
  .Accepts<PutCouponDto>("application/json")
  .Produces<APIResponse>(200)
  .Produces(400);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    var response = new APIResponse()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    var couponFromStore = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);

    if (couponFromStore != null)
    {
        CouponStore.Coupons.Remove(couponFromStore);
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMsgs.Add("Invalid Id");
        return Results.BadRequest(response);
    }

}).WithName("DeleteCoupon").Produces<APIResponse>(204).Produces(400);

app.Run();
