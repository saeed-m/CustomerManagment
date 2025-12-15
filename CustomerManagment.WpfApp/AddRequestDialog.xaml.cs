using CustomerManagment.WpfApp.Data;
using CustomerManagment.WpfApp.Data.Models;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerManagment.WpfApp
{
    public partial class AddRequestDialog : Window, INotifyPropertyChanged
    {
        private readonly int _customerId;
        private readonly AppDbContext _context;

        private RequestStatus _selectedStatus = RequestStatus.Pending;
        public RequestStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public AddRequestDialog(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            _context = new AppDbContext();
            DataContext = this;
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Please enter request description.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var request = new CustomerRequest
                {
                    CustomerRequestDescription = txtDescription.Text.Trim(),
                    Status = SelectedStatus,
                    CustomerId = _customerId,
                    CreateDate = DateTime.UtcNow
                };

                _context.CustomerRequests.Add(request);
                _context.SaveChanges();

                MessageBox.Show("Request added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving request: {ex.Message}", "Error",
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