using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementSystem
{
    public class Table
    {
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; } = false;
        public Customer? OccupyingCustomer { get; set; }
        public bool OrderTaken { get; set; } = false;
        public bool MealServed { get; set; } = false;
        public bool IsWaiterWaitingOnTable { get; set; } = false;

        public Table(int tableNumber)
        {
            TableNumber = tableNumber;
        }

        public void Occupy(Customer customer)
        {
            OccupyingCustomer = customer;
            IsOccupied = true;
        }

        public void Vacate()
        {
            OccupyingCustomer = null;
            IsOccupied = false;
            MealServed = false;
            OrderTaken = false;
        }
    }
}
