namespace ShoppingCart.ShoppingCart
{
    using System.Collections.Generic;
    using System.Linq;
    using global::ShoppingCart.EventFeed;

    public class ShoppingCart
    {
        private HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();

        public int UserId { get; }

        public IEnumerable<ShoppingCartItem> Items
        {
            get { return Items1; }
        }

        public HashSet<ShoppingCartItem> Items1
        {
            get => Items2;
            set => Items2 = value;
        }

        public HashSet<ShoppingCartItem> Items2
        {
            get => items;
            set => items = value;
        }

        public ShoppingCart(int userId)
        {
            this.UserId = userId;
        }

        public ShoppingCart(int userId, IEnumerable<ShoppingCartItem> items)
        {
            this.UserId = userId;
            foreach (var item in items)
            {
                this.Items1.Add(item);
            }
        }

        public void AddItems(
            IEnumerable<ShoppingCartItem> shoppingCartItems,
            IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
                if (this.Items1.Add(item))
                    eventStore.Raise(
                        "ShoppingCartItemAdded",
                        new {UserId, item});
        }

        public void RemoveItems(
            int[] productCatalogueIds,
            IEventStore eventStore)
        {
            Items1.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogId));
        }
    }

    public class ShoppingCartItem
    {
        public int ShoppingCartId { get; }
        public int ProductCatalogId { get; }
        public string ProductName { get; }
        public string ProductDescription { get; }
        public decimal Amount { get; }
        public string Currency { get; }

        public ShoppingCartItem()
        {
        }

        public ShoppingCartItem(
            int productCatalogId,
            string productName,
            string productDescription,
            decimal amount,
            string currency)
        {
            this.ProductCatalogId = productCatalogId;
            this.ProductName = productName;
            this.ProductDescription = productDescription;
            this.Amount = amount;
            this.Currency = currency;
        }

        public ShoppingCartItem(
            int shoppingCartId,
            int productCatalogId,
            string productName,
            string productDescription,
            decimal amount,
            string currency)
        {
            this.ShoppingCartId = shoppingCartId;
            this.ProductCatalogId = productCatalogId;
            this.ProductName = productName;
            this.ProductDescription = productDescription;
            this.Amount = amount;
            this.Currency = currency;
        }

//        public ShoppingCartItem(
//            int productCatalogueId,
//            string productName,
//            string description,
//            Money price)
//        {
//            this.ProductCatalogueId = productCatalogueId;
//            this.ProductName = productName;
//            this.Desscription = description;
//            this.Price = price;
//        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = obj as ShoppingCartItem;
            return this.ProductCatalogId.Equals(that.ProductCatalogId);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.ProductCatalogId.GetHashCode();
        }
    }

    public class Money
    {
        public string Currency { get; }
        public decimal Amount { get; }

        public Money(string currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;
        }
    }
}
