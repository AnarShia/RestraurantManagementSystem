using RestaurantManagementSystem.Core.Restaurant;
using RestaurantManagementSystem.Interfaces;

namespace RestaurantManagementSystem.Core.Personels
{
    public class Waiter : IPersonel
    {
        private readonly RestaurantController Restaurant;
        public int WaiterId { get; set; }
        public string Name { get; set; }
        public Waiter(RestaurantController restaurant, int waiterId, string name)
        {
            WaiterId = waiterId;
            Name = name;
            Restaurant = restaurant;
        }
        public string WaiterStatus { get; set; } = "Free";
        public Table? Table { get; set; }

        private static readonly object lockServeObject = new();
        private static readonly object lockTakeOrderObject = new();
        public void TakeOrder()
        {
            lock (lockTakeOrderObject)
            {
                Table = null;
                try
                {
                    Table = Restaurant.GetTables().First(t => t.IsOccupied && !t.OrderTaken && !t.IsWaiterWaitingOnTable);
                    Table.IsWaiterWaitingOnTable = true;
                    WaiterStatus = $"Taking Order {Table.TableNumber}";
                    Table.OrderTaken = true;

                }
                catch (Exception)
                {
                }
                if (Table != null)
                {
                    Table.IsWaiterWaitingOnTable = false;
                }
            }

        }
        public Task Initialize()
        {
            while (true)
            {
                WaiterStatus = "Free";
                TakeOrder();
                Thread.Sleep(3000);
                ServeMealsToCustomers();
                Thread.Sleep(4000);
            }
        }
        public void ServeMealsToCustomers()
        {
            Order? readyToServe = null;
            lock (lockServeObject)
            {
                readyToServe = Restaurant.GetCollectedOrders().FirstOrDefault(x => x.IsCooked && x.IsServed == false);
            }

            if (readyToServe != null)
            {

                WaiterStatus = $"Delivering {readyToServe.Customer.CustomerId}";
                readyToServe.Customer.Table.MealServed = true;
                readyToServe.IsServed = true;
                Thread.Sleep(1000);
            }

        }

    }
}
