using RestaurantManagementSystem.Core.Restaurant;
using System.Data;

namespace RestaurantManagementSystem
{
    public partial class MainForm : Form
    {
        private RestaurantController Restaurant;
        public MainForm()
        {
            InitializeComponent();
            Restaurant = new RestaurantController();
            Restaurant.InitializeComponents();
        }

        private void UpdateCustomerEnterDataGridView()
        {
            List<Customer> list = Restaurant.GetCustomers().Where(x => x.Table == null).ToList();
            dgv_beklemeListesi.DataSource = list;
        }

        private void Restaurant_LoadAsync(object sender, EventArgs e)
        {
            Restaurant.RunRestaurantOperationsAsync();
            UpdateCustomerEnterDataGridView();
        }
        private void Btn_EnterCustomer_Click(object sender, EventArgs e)
        {
            RestaurantOperations(numberOfCustomer: 15);

            UpdateTableUIs();
            UpdateCustomerEnterDataGridView();
            UpdateCustomerSitDataGridView();
            UpdateChefsUIs();
            UpdateWaiterButtons();
        }
        private void Btn_Next_Click(object sender, EventArgs e)
        {
            UpdateTableUIs();
            UpdateCustomerEnterDataGridView();
            UpdateCustomerSitDataGridView();
            UpdateChefsUIs();
            UpdateWaiterButtons();
        }

        private void RestaurantOperations(int numberOfCustomer)
        {
            Task.Run(() => Restaurant.EnterCustomer(numberOfCustomer: numberOfCustomer));

        }
        private void UpdateWaiterButtons()
        {
            foreach (var waiter in Restaurant.GetWaiters())
            {
                if (Controls.Find($"btn_Garson{waiter.WaiterId}", true).FirstOrDefault() is Button waiterButton)
                {
                    waiterButton.Text = waiter.WaiterStatus;
                }
            }
        }
        private void UpdateTableUIs()
        {
            foreach (var table in Restaurant.GetTables())
            {
                if (Controls.Find($"btn_Masa{table.TableNumber}", true).FirstOrDefault() is Button tableButton)
                {
                    tableButton.Text = table.OccupyingCustomer != null && table.IsOccupied ? $"Customer: {table.OccupyingCustomer!.CustomerId}" : "Free";
                }
            }
        }
        private void UpdateChefsUIs()
        {
            foreach (var chef in Restaurant.GetChefs())
            {
                if (Controls.Find($"btn_Asci{chef.ChefId}", true).FirstOrDefault() is Button chefButton)
                {
                    chefButton.Text = chef.Status;
                }
            }
        }
        private void UpdateCustomerSitDataGridView()
        {
            List<Customer> list = Restaurant.GetCustomers().Where(x => x.Table != null).ToList();
            dgv_oturanlarListesi.DataSource = list;
        }

     
    }
}