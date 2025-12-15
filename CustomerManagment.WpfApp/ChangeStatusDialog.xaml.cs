using CustomerManagment.WpfApp.Data.Models;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerManagment.WpfApp
{
    public partial class ChangeStatusDialog : Window, INotifyPropertyChanged
    {
        private readonly int _requestId;
        private readonly string _requestDescription;
        private readonly RequestStatus _currentStatus;

        private RequestStatus _selectedStatus;
        public RequestStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        public ChangeStatusDialog(int requestId, string requestDescription, RequestStatus currentStatus)
        {
            InitializeComponent();
            _requestId = requestId;
            _requestDescription = requestDescription;
            _currentStatus = currentStatus;
            SelectedStatus = currentStatus;

            DataContext = this;
            InitializeUI();
            LoadStatuses();
        }

        private void InitializeUI()
        {
            txtRequestDescription.Text = _requestDescription;
            txtCurrentStatus.Text = _currentStatus.GetDisplayName();
            txtCurrentStatusIcon.Text = _currentStatus.GetIcon();
            txtStatusDescription.Text = _currentStatus.GetDescription();

            // Set current status border color
            var currentStatusColor = _currentStatus.GetColor();
            currentStatusBorder.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter()
                .ConvertFromString(currentStatusColor);
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
            cbStatus.SelectedValue = _currentStatus;

            // Update description when selection changes
            cbStatus.SelectionChanged += (s, e) =>
            {
                if (cbStatus.SelectedItem != null)
                {
                    var selectedItem = (dynamic)cbStatus.SelectedItem;
                    SelectedStatus = selectedItem.Status;
                    txtStatusDescription.Text = selectedItem.Description;
                }
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
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