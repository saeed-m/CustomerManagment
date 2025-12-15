using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.Data.Models;
using CustomerManagment.WpfApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CustomerManagment.WpfApp
{
    public partial class CustomerRequestsPage : Window, INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly int _customerId;
        private Customer? _customer;
        private ObservableCollection<CustomerRequestViewModel> _allRequests = new();
        private string _customerName = string.Empty;
        private string _customerEmail = string.Empty;
        private string _customerMobileNumber = string.Empty;
        private int _totalRequests;
        private int _solvedRequests;
        private int _pendingRequests;

        public string CustomerName
        {
            get => _customerName;
            set => SetField(ref _customerName, value);
        }

        public string CustomerEmail
        {
            get => _customerEmail;
            set => SetField(ref _customerEmail, value);
        }

        public string CustomerMobileNumber
        {
            get => _customerMobileNumber;
            set => SetField(ref _customerMobileNumber, value);
        }

        public int TotalRequests
        {
            get => _totalRequests;
            set => SetField(ref _totalRequests, value);
        }

        public int SolvedRequests
        {
            get => _solvedRequests;
            set => SetField(ref _solvedRequests, value);
        }

        public int PendingRequests
        {
            get => _pendingRequests;
            set => SetField(ref _pendingRequests, value);
        }

        public ObservableCollection<CustomerRequestViewModel> Requests { get; } = new();

        public CustomerRequestsPage(int customerId)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _customerId = customerId;
            DataContext = this;
            LoadCustomerData();
            LoadRequests();
        }

        private void LoadCustomerData()
        {
            try
            {
                _customer = _context.Customers
                    .FirstOrDefault(c => c.Id == _customerId);

                if (_customer != null)
                {
                    CustomerName = _customer.CustomerName;
                    CustomerEmail = _customer.CustomerEmail;
                    CustomerMobileNumber = _customer.CustomerMobileNumber ?? "N/A";

                    // Update UI elements
                    txtCustomerName.Text = _customer.CustomerName;
                    txtCustomerEmail.Text = _customer.CustomerEmail;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRequests()
        {
            try
            {
                var requests = _context.CustomerRequests
                    .Where(r => r.CustomerId == _customerId)
                    .OrderByDescending(r => r.CreateDate)
                    .ToList();

                _allRequests.Clear();
                foreach (var request in requests)
                {
                    _allRequests.Add(new CustomerRequestViewModel
                    {
                        Id = request.Id,
                        Description = request.CustomerRequestDescription,
                        Status = request.Status, // Use Status instead of IsSolved
                        CreateDate = request.CreateDate
                    });
                }

                UpdateRequestsList();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading requests: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRequestsList()
        {
            Requests.Clear();

            var filteredRequests = _allRequests;
            if (chkShowOnlyPending?.IsChecked == true)
            {
                // Filter to show only non-solved statuses
                filteredRequests = new ObservableCollection<CustomerRequestViewModel>(
                    _allRequests.Where(r => r.Status != RequestStatus.Solved));
            }

            foreach (var request in filteredRequests)
            {
                Requests.Add(request);
            }

            // Show/hide no data message
            if (txtNoData != null)
            {
                txtNoData.Visibility = Requests.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateStatistics()
        {
            TotalRequests = _allRequests.Count;
            SolvedRequests = _allRequests.Count(r => r.Status == RequestStatus.Solved); // Use Status
            PendingRequests = _allRequests.Count(r => r.Status != RequestStatus.Solved); // Use Status

            // Update UI elements
            if (txtTotalRequestsCount != null)
                txtTotalRequestsCount.Text = TotalRequests.ToString();

            if (txtSummaryTotal != null)
                txtSummaryTotal.Text = TotalRequests.ToString();

            if (txtSummarySolved != null)
                txtSummarySolved.Text = SolvedRequests.ToString();

            if (txtSummaryPending != null)
                txtSummaryPending.Text = PendingRequests.ToString();
        }

        private void FilterRequests(object sender, RoutedEventArgs e)
        {
            UpdateRequestsList();
        }

        private void BtnAddRequest_Click(object sender, RoutedEventArgs e)
        {
            var addRequestDialog = new AddRequestDialog(_customerId);
            if (addRequestDialog.ShowDialog() == true)
            {
                LoadRequests();
            }
        }

        // Updated event handlers for status changes
        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int requestId)
            {
                var request = _allRequests.FirstOrDefault(r => r.Id == requestId);
                if (request != null)
                {
                    var dialog = new ChangeStatusDialog(
                        requestId,
                        request.Description,
                        request.Status);

                    if (dialog.ShowDialog() == true && dialog.SelectedStatus != request.Status)
                    {
                        UpdateRequestStatus(requestId, dialog.SelectedStatus);
                    }
                }
            }
        }

        private void BtnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int requestId)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this request?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteRequest(requestId);
                }
            }
        }

        // Remove the old MarkAsSolved and MarkAsPending methods since we now use ChangeStatusDialog
        // Keep ToggleRequestStatus for backward compatibility if needed

        private void UpdateRequestStatus(int requestId, RequestStatus newStatus)
        {
            try
            {
                var request = _context.CustomerRequests.Find(requestId);
                if (request != null)
                {
                    request.Status = newStatus;
                   // request. = DateTime.UtcNow;
                    _context.SaveChanges();

                    LoadRequests(); // Refresh the list
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating request status: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRequest(int requestId)
        {
            try
            {
                var request = _context.CustomerRequests.Find(requestId);
                if (request != null)
                {
                    _context.CustomerRequests.Remove(request);
                    _context.SaveChanges();
                    LoadRequests(); // Refresh the list
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting request: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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