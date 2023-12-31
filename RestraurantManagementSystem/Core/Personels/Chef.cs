using RestaurantManagementSystem.Core.Restaurant;
using RestaurantManagementSystem.Interfaces;

namespace RestaurantManagementSystem.Core.Personels
{
    public class Chef : IPersonel
    {
        private readonly RestaurantController Restaurant;
        private readonly object lockOrder = new object();

        public int ChefId { get; set; }
        public string Name { get; set; }
        public bool CanTakeOrder { get; set; } = true;
        public int CookingLimit { get; set; } = 2;
        public string Status { get; internal set; } = "Free";

        public Chef(RestaurantController restaurant, int chefId, string name)
        {
            ChefId = chefId;
            Name = name;
            Restaurant = restaurant;
        }

        public Task Initialize()
        {
            while (true)
            {
                if (Restaurant.GetCollectedOrders().Count > 0 && CookingLimit > 0)
                {
                    for (int i = 0; i < CookingLimit; i++)
                    {
                        Task.Run(() => Cook());
                        Task.Run(() => Cook2());
                    }
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }

        }
        public Task Cook()
        {
            Order? order = null;
            lock (lockOrder)
            {
                order = Restaurant.GetCollectedOrders().FirstOrDefault(x => !x.IsCooked && !x.HasChefStartedCooking);

            }

            if (order != null)
            {
                Status = $"Cooking {order.Customer.CustomerId}";
                order.HasChefStartedCooking = true;
                CookingLimit--;
                Thread.Sleep(2000);
                order.IsCooked = true;
                CookingLimit++;
                Status = $"Cooked {order.Customer.CustomerId}";
            }
            return Task.CompletedTask;

        }

        public Task Cook2()
        {

            Order? order = null;
            lock (lockOrder)
            {
                order = Restaurant.GetCollectedOrders().FirstOrDefault(x => !x.IsCooked && !x.HasChefStartedCooking);

            }

            if (order != null)
            {
                Status = $"Cooking {order.Customer.CustomerId}";
                order.HasChefStartedCooking = true;
                CookingLimit--;
                Thread.Sleep(2000);
                order.IsCooked = true;
                CookingLimit++;
                Status = $"Cooked {order.Customer.CustomerId}";
            }
            return Task.CompletedTask;

        }
    }
}
