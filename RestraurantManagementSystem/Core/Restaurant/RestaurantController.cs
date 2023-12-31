using RestaurantManagementSystem.Core.Personels;


namespace RestaurantManagementSystem.Core.Restaurant
{
    public class RestaurantController
    {
        private readonly object customerLock = new ();
        private readonly object orderLock = new ();

        private List<Customer> Customers { get; set; }
        private List<Table> Tables { get; set; } = new List<Table>();
        private List<Waiter> Waiters { get; set; }
        private List<Chef> Chefs { get; set; }
        private Cashier Cashier { get; set; }

        private List<Order> collectedOrders = new List<Order>();

        public RestaurantController()
        {
            Customers = new List<Customer>();
            Tables = new List<Table>();
            Waiters = new List<Waiter>();
            Chefs = new List<Chef>();
            Cashier = new Cashier(this, "Kasa Görevlisi");
        }
        public void InitializeComponents()
        {
            for (int i = 0; i < 6; i++)
            {
                Tables.Add(new Table(i + 1));
            }

            for (int i = 0; i < 3; i++)
            {
                Waiters.Add(new Waiter(this, i + 1, $"Garson {i + 1}"));
            }

            for (int i = 0; i < 2; i++)
            {
                Chefs.Add(new Chef(this, i + 1, $"Aşçı {i + 1}"));
            }
        }

        private bool DeterminePriority()
        {
            return new Random().Next(0, 2) == 1;
        }


        public void RunRestaurantOperationsAsync()
        {
            Task.Run(() => StartToWaitersWork());
            Task.Run(() => StartToChefsWork());
            Task.Run(() => AssignCustomersToTables());
            Task.Run(() => EnterCustomer(5));
        }
        public Task AssignCustomersToTables()
        {
            while (true)
            {
                List<Table> emptyTables = Tables.Where(x => x.IsOccupied == false).ToList();


                if (Customers.Count > 0 && emptyTables.Count > 0)
                {
                    foreach (var table in emptyTables)
                    {

                        if (Customers.Count == 0)
                        {
                            break;
                        }

                        Customer? customer = GetCustomer();

                        if (customer != null)
                        {
                            table.OccupyingCustomer = customer;
                            customer.IsOnTheLine = true;
                        }
                    }

                }
                else
                {
                    Thread.Sleep(3000);
                }

            }
        }
        public Task StartToWaitersWork()
        {
            foreach (var waiter in Waiters)
            {
                Task.Run(() => waiter.Initialize());
            }
            return Task.CompletedTask;
        }
        public Task StartToChefsWork()
        {
            foreach (var chef in Chefs)
            {
                Task.Run(() => chef.Initialize());
            }
            return Task.CompletedTask;
        }
        public Task EnterCustomer(int numberOfCustomer)
        {
            lock (customerLock)
            {
                for (int i = 0; i < numberOfCustomer; i++)
                {
                    bool priority = DeterminePriority();
                    Customer customer = new(this, Customers.Count + 1, priority);
                    new Thread(() => customer.Initialize()).Start();
                    Customers.Add(customer);
                }
            }
        
            return Task.CompletedTask;
        }

        public List<Customer> GetCustomers()
        {
            lock (customerLock)
            {
                return Customers;
            }
        }

        public List<Table> GetTables()
        {
            return Tables;
        }

        public List<Waiter> GetWaiters()
        {
            return Waiters;
        }

        public List<Chef> GetChefs()
        {
            return Chefs;
        }
        public List<Order> GetCollectedOrders()
        {
            lock (orderLock)
            {
                return collectedOrders;

            }
        }

        public void AddOrder(Order order)
        {
            collectedOrders.Add(order);
        }

        public void LeaveCustomer(Customer customer)
        {
            lock (customerLock)
            {
                Customers.Remove(customer);
                Order? order = collectedOrders.FirstOrDefault(x => x.Customer == customer);
                if (order != null)
                {
                    collectedOrders.Remove(order);
                }
            }

        }

        public Customer? GetCustomer()
        {
            lock(customerLock)
            {
                Customer? customer = Customers.FirstOrDefault(customer => customer.Table == null);

                return customer;
            }
          
        }

    }
}
