using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows.Controls;
using CustomerManagment.WpfApp.Data.Models;

namespace CustomerManagment.WpfApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<CustomerViewModel> _customers = new();
        private readonly AppDbContext _context;

        public ObservableCollection<CustomerViewModel> Customers
        {
            get => _customers;
            set => SetField(ref _customers, value);
        }

        // Add these properties to your MainWindow class
        private string _totalRequestsText;
        private string _pendingRequestsText;
        private string _solvedRequestsText;

        public string TotalRequestsText
        {
            get => _totalRequestsText;
            set => SetField(ref _totalRequestsText, value);
        }

        public string PendingRequestsText
        {
            get => _pendingRequestsText;
            set => SetField(ref _pendingRequestsText, value);
        }

        public string SolvedRequestsText
        {
            get => _solvedRequestsText;
            set => SetField(ref _solvedRequestsText, value);
        }

        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            DataContext = this;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                _context.Database.EnsureCreated();

                var customers = _context.Customers
                    .Include(c => c.CustomerRequests)
                    .OrderByDescending(c => c.CreateDate)
                    .ToList();

                var customerViewModels = new ObservableCollection<CustomerViewModel>();
                int totalRequests = 0;
                int solvedRequests = 0;
                int pendingRequests = 0;

                foreach (var customer in customers)
                {
                    var requestsCount = customer.CustomerRequests?.Count ?? 0;
                    // Use Status enum instead of IsSolved
                    var solvedCount = customer.CustomerRequests?.Count(r => r.Status == RequestStatus.Solved) ?? 0;
                    var pendingCount = customer.CustomerRequests?.Count(r => r.Status == RequestStatus.Pending) ?? 0;

                    totalRequests += requestsCount;
                    solvedRequests += solvedCount;
                    pendingRequests += pendingCount;

                    customerViewModels.Add(new CustomerViewModel
                    {
                        Id = customer.Id,
                        CustomerName = customer.CustomerName,
                        CustomerEmail = customer.CustomerEmail,
                        CustomerMobileNumber = customer.CustomerMobileNumber,
                        CustomerJobTitle = customer.CustomerJobTitle,
                        CustomerAddress = customer.CustomerAddress,
                        CustomerPhoneNumber = customer.CustomerPhoneNumber,
                        RequestsCount = requestsCount,
                        SolvedRequestsCount = solvedCount,
                        PendingRequestsCount = pendingCount
                    });
                }

                Customers = customerViewModels;
                TotalRequestsText = totalRequests.ToString();
                SolvedRequestsText = solvedRequests.ToString();
                PendingRequestsText = pendingRequests.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event Handlers
        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerPage();
            if (addCustomerWindow.ShowDialog() == true)
            {
                LoadCustomers(); // Refresh the list
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void BtnSubmitRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int customerId)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    var submitRequestWindow = new SubmitRequestPage(customerId);
                    if (submitRequestWindow.ShowDialog() == true)
                    {
                        LoadCustomers(); // Refresh the list with updated counts
                    }
                }
            }
        }

        private void BtnViewRequests_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int customerId)
            {
                var customer = Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    var requestsWindow = new CustomerRequestsPage(customerId);
                    requestsWindow.Owner = this;
                    requestsWindow.ShowDialog();

                    // Refresh the counts after closing the requests window
                    LoadCustomers();
                }
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}