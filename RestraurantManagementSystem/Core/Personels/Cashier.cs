using RestaurantManagementSystem.Core.Restaurant;
using RestaurantManagementSystem.Interfaces;

namespace RestaurantManagementSystem.Core.Personels
{
    public class Cashier : IPersonel
    {
        public string Name { get; set; }
        public RestaurantController Restaurant { get; set; }

        public Cashier(RestaurantController restaurant, string name)
        {
            Name = name;
            Restaurant = restaurant;
        }
        public Task Initialize()
        {
            while (true)
            {
                Customer? customer = Restaurant.GetCustomers().FirstOrDefault(x => x.Order != null && x.Order.HasEaten);
                if (customer != null)
                {
                    Thread.Sleep(2000);
                    customer.Order = null;
                    Restaurant.LeaveCustomer(customer);
                }
            }
        }
    }
}
