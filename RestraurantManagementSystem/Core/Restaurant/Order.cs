
namespace RestaurantManagementSystem.Core.Restaurant
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderedItems { get; set; } 
        public Customer Customer { get; set; }
        public bool HasChefStartedCooking { get; set; } = false;
        public bool IsCooked { get; set; } = false;

        public bool IsServed { get; set; } = false;
        public bool HasEaten { get; set; } = false;


        public Order(int orderId, Customer customer, string orderedItems)
        {
            OrderId = orderId;
            Customer = customer;
            OrderedItems = orderedItems;
        }
    }
}
