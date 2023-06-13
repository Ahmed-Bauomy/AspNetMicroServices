using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly NpgsqlConnection connection;
        public DiscountRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            connection = new NpgsqlConnection(connectionString);
        }

        public async Task<Coupon> GetCoupon(string productName)
        {
            var getQuery = "Select * From Coupon Where ProductName = @ProductName";
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(getQuery,new { productName = productName });
            if (coupon == null)
                return new Coupon() { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }

        public async Task<bool> CreateCoupon(Coupon coupon)
        {
            var createQuery = "Insert Into Coupon(ProductName,Amount,Description) Values(@ProductName,@Amount,@Description)";
            var affected = await connection.ExecuteAsync(createQuery,new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description });
            return affected > 0;
        }
        public async Task<bool> UpdateCoupon(Coupon coupon)
        {
            var updateQuery = "Update Coupon set ProductName = @ProductName,Amount = @Amount,Description = @Description Where Id = @Id";
            var affected = await connection.ExecuteAsync(updateQuery, new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description, Id = coupon.Id });
            return affected > 0;
        }

        public async Task<bool> DeleteCoupon(string productName)
        {
            var deleteQuery = "Delete From Coupon where ProductName = @ProductName";
            var affected = await connection.ExecuteAsync(deleteQuery, new { ProductName = productName });
            return affected > 0;
        }

        
    }
}
