using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly NpgsqlConnection _connection;

        public DiscountRepository(IConfiguration configuration)
        {
            _connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }

        public async Task<Coupon> GetCoupon(string productName)
        {
            var getQuery = "Select * From Coupon where ProductName=@ProductName";
            var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>(getQuery);
            if (coupon == null)
                return new Coupon() { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            return coupon;
        }

        public async Task<bool> CreateCoupon(Coupon coupon)
        {
            var CreateQuery = "Insert Into Coupon(ProductName,Amount,Description) Values(@ProductName,@Amount,@Description)";
            var affected = await _connection.ExecuteAsync(CreateQuery, new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description });
            return affected > 0;
        }

        public async Task<bool> UpdateCoupon(Coupon coupon)
        {
            var updateQuery = "Update Coupon set ProductName = @ProductName,Amount = @Amount,Description = @Description Where Id = @Id";
            var affected = await _connection.ExecuteAsync(updateQuery, new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description, Id = coupon.Id });
            return affected > 0;
        }

        public async Task<bool> DeleteCoupon(string productName)
        {
            var deleteQuery = "Delete From Coupon where ProductName = @ProductName";
            var affected = await _connection.ExecuteAsync(deleteQuery, new { ProductName = productName });
            return affected > 0;
        }
    }
}
