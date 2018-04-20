namespace ShoppingCart.ShoppingCart
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;

    public class ShoppingCartStore : IShoppingCartStore
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=ShoppingCart;
Integrated Security=True";

        private const string readItemsSql = @"select sci.ProductCatalogId
, ProductName
, sci.ProductDescription
, Amount
, Currency  
from ShoppingCart sc, ShoppingCartItems sci
where sci.ShoppingCartId = sc.ID
and sc.UserId=@UserId";

        public async Task<ShoppingCart> Get(int userId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    var items = await conn.QueryAsync<ShoppingCartItem>(readItemsSql, new { UserId = userId });
                    return new ShoppingCart(userId, items);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return new ShoppingCart(userId, new List<ShoppingCartItem>());
            }
        }

        private const string deleteAllForShoppingCartSql = @"delete item from ShoppingCartItems item
inner join ShoppingCart cart on item.ShoppingCartId = cart.ID
and cart.UserId=@UserId";

        private const string addAllForShoppingCartSql = @"insert into ShoppingCartItems 
(ShoppingCartId, ProductCatalogId, ProductName, ProductDescription, Amount, Currency)
values 
(5, @ProductCatalogId, @ProductName, @ProductDescription, @Amount, @Currency)";

        public async Task Save(ShoppingCart shoppingCart)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
              //  using (var tx = conn.BeginTransaction())
                {
                    await conn.ExecuteAsync(deleteAllForShoppingCartSql, new { UserId = shoppingCart.UserId })
                        .ConfigureAwait(false);
                    await conn.ExecuteAsync(addAllForShoppingCartSql, shoppingCart.Items).ConfigureAwait(false);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}