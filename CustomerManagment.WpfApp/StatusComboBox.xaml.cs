using CustomerManagment.WpfApp.Data.Models;
using System.Windows;
using System.Windows.Controls;

namespace CustomerManagment.WpfApp.Controls
{
    public partial class StatusComboBox : UserControl
    {
        public static readonly DependencyProperty SelectedStatusProperty =
            DependencyProperty.Register("SelectedStatus", typeof(RequestStatus),
                typeof(StatusComboBox), new PropertyMetadata(RequestStatus.Pending, OnSelectedStatusChanged));

        public RequestStatus SelectedStatus
        {
            get => (RequestStatus)GetValue(SelectedStatusProperty);
            set => SetValue(SelectedStatusProperty, value);
        }

        public StatusComboBox()
        {
            InitializeComponent();
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
            cbStatus.SelectedValuePath = "Status";
        }

        private static void OnSelectedStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StatusComboBox)d;
            if (control.cbStatus.ItemsSource != null)
            {
                control.cbStatus.SelectedValue = e.NewValue;
            }
        }
    }
}