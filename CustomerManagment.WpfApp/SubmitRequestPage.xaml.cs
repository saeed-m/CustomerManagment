using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerManagment.WpfApp
{
    public partial class SubmitRequestPage : Window, INotifyPropertyChanged
    {
        private readonly int _customerId;
        private readonly AppDbContext _context;
        private Customer? _customer;

        private RequestStatus _selectedStatus = RequestStatus.Pending;
        public RequestStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        public SubmitRequestPage(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            _context = new AppDbContext();
            DataContext = this;
            LoadCustomerData();
            LoadStatuses();
        }

        private void LoadStatuses()
        {
            var statuses = RequestStatusExtensions.GetAllStatuses();
            var items = statuses.Select(s => new
            {
                Status = s,
                DisplayName = s.GetDisplayName(),
                Icon = s.GetIcon(),
                Color = s.GetColor(),
                Description = s.GetDescription()
            }).ToList();

            cbStatus.ItemsSource = items;
            cbStatus.SelectedValue = RequestStatus.Pending;

            // Handle selection change
            cbStatus.SelectionChanged += (s, e) =>
            {
                if (cbStatus.SelectedItem != null)
                {
                    var selectedItem = (dynamic)cbStatus.SelectedItem;
                    SelectedStatus = selectedItem.Status;
                }
            };
        }

        private void LoadCustomerData()
        {
            try
            {
                _customer = _context.Customers
                    .Include(c => c.CustomerRequests)
                    .FirstOrDefault(c => c.Id == _customerId);

                if (_customer != null)
                {
                    txtCustomerInfo.Text = $"{_customer.CustomerName}";

                    // Show request statistics - updated for enum
                    int requestCount = _customer.CustomerRequests?.Count ?? 0;
                    int solvedCount = _customer.CustomerRequests?.Count(r => r.Status == RequestStatus.Solved) ?? 0;
                    int pendingCount = _customer.CustomerRequests?.Count(r => r.Status == RequestStatus.Pending) ?? 0;

                    txtTotalRequests.Text = requestCount.ToString();
                    txtSolvedRequests.Text = solvedCount.ToString();
                    txtPendingRequests.Text = pendingCount.ToString();

                    // Show the latest request if available
                    if (requestCount > 0)
                    {
                        var latestRequest = _customer.CustomerRequests?
                            .OrderByDescending(r => r.CreateDate)
                            .FirstOrDefault();

                        if (latestRequest != null)
                        {
                            txtLatestRequest.Text =
                                $"Latest request ({latestRequest.CreateDate:dd/MM/yyyy}): " +
                                $"{latestRequest.CustomerRequestDescription} " +
                                $"({latestRequest.Status.GetDisplayName()})";
                        }
                    }
                    else
                    {
                        txtLatestRequest.Text = "No previous requests";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRequestDescription.Text))
                {
                    MessageBox.Show("Please enter request description.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new request with selected status
                var request = new CustomerRequest
                {
                    CustomerRequestDescription = txtRequestDescription.Text.Trim(),
                    Status = SelectedStatus,
                    CustomerId = _customerId,
                    CreateDate = DateTime.UtcNow
                };

                _context.CustomerRequests.Add(request);
                _context.SaveChanges();

                MessageBox.Show("Request submitted successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting request: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}