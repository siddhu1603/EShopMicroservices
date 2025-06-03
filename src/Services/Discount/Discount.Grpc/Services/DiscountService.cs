using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService
        (DiscountContext dbContext, ILogger<DiscountService> logger) 
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon  = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon == null)
                coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Description"};

            logger.LogInformation("Disocunt is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();

            //It is not directly request.Adapt<Coupon> because req is a CreateDiscountRequest obj which wraps a CouponModel
            //So we need to first access the CouponModel within the req obj by using request.Coupon;
            //in CreateDiscountRequest the field name is coupon(lower) but in the auto-generated files the prop name is Coupon(upper)
            //Thus request.coupon gives error and request.Coupon is right.


            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

            dbContext.Add(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Disocunt is Successfully created for ProductName : {productName}", coupon.ProductName);

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
            
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Product Name {request.ProductName} not found"));

            dbContext.Remove(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Disocunt is Successfully Deleted for ProductName : {productName}", request.ProductName);

            return new DeleteDiscountResponse { Success = true };
        }

        public async override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            //if we dont mention the Id in req body then it is treated as a NEW CREATION
            //when Id is mentioned the object is uniquely identified and UPDATED
            //If any other parameter like productname, description or amount is missing it is still treated as UPDATE
            //The skipped fields are just put as empty values.

            if (request.Coupon.Id == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must be provided for an update."));
            }

            var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Requested Coupon was Not Found."));

            dbContext.Update(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Disocunt is Successfully Updated for Product : {ProductName}", coupon.ProductName);

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
    }
}
