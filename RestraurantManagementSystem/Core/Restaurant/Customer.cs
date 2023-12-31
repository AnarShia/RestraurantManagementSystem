using RestaurantManagementSystem.Core.Restaurant;

namespace RestaurantManagementSystem
{
    public class Customer
    {
        private readonly RestaurantController restaurant;
        private readonly object sitLock = new object();
        private readonly object orderLock = new object();
        private readonly object tableLock = new object();
        public int CustomerId { get; set; }
        public bool IsPriority { get; set; }
        private bool IsSeated { get; set; } = false;
        public bool IsOnTheLine { get; set; } = false;


        public Order? Order { get; set; }
        public Table? Table { get; set; }

        public Customer(RestaurantController restaurant, int customerId, bool isPriority)
        {
            CustomerId = customerId;
            IsPriority = isPriority;
            Order = null;
            Table = null;
            this.restaurant = restaurant;
        }
        public Task Initialize()
        {
            bool isInRestaurant = true;
            while (isInRestaurant)
            {
                while ((!IsSeated) && IsOnTheLine && Table == null)
                {
                    try
                    {
                        SitAtTable();
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(2000);
                    }
                    IsOnTheLine = false;
                }

                while (IsSeated && Order == null)
                {
                    try
                    {
                        GiveOrder();

                    }
                    catch (Exception)
                    {
                        Thread.Sleep(2000);
                    }
                }

                if (Order != null && Order.IsCooked && Order.IsServed)
                {

                    Thread.Sleep(2000);
                    Order.HasEaten = true;
                    LeaveTable();

                }
                Thread.Sleep(2000);
            }
            return Task.CompletedTask;

        }

        public void LeaveTable()
        {
            lock (tableLock)
            {
                Table!.Vacate();
            }
            IsSeated = false;
        }
        public void SitAtTable()
        {
            Table? placeToSit = null;
            lock (sitLock)
            {
                placeToSit = restaurant.GetTables().First(t => t.OccupyingCustomer == this);
            }
            if (placeToSit == null)
            {
                return;
            }

            placeToSit.Occupy(this);
            Table = placeToSit;
            IsSeated = true;
            Order = null;
        }
        public void GiveOrder()
        {
            Order order = new Order(restaurant.GetCollectedOrders().Count + 1, this, "Food");
            Order = order;

            lock(orderLock)
            {
                restaurant.GetCollectedOrders().Add(order);
            }
        }
    }
}
