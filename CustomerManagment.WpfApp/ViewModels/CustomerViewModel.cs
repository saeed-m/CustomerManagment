using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerManagment.WpfApp.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        private string _customerName = string.Empty;
        public string CustomerName
        {
            get => _customerName;
            set => SetField(ref _customerName, value);
        }

        private string? _customerJobTitle;
        public string? CustomerJobTitle
        {
            get => _customerJobTitle;
            set => SetField(ref _customerJobTitle, value);
        }

        private string? _customerAddress;
        public string? CustomerAddress
        {
            get => _customerAddress;
            set => SetField(ref _customerAddress, value);
        }

        private string? _customerMobileNumber;
        public string? CustomerMobileNumber
        {
            get => _customerMobileNumber;
            set => SetField(ref _customerMobileNumber, value);
        }

        private string? _customerPhoneNumber;
        public string? CustomerPhoneNumber
        {
            get => _customerPhoneNumber;
            set => SetField(ref _customerPhoneNumber, value);
        }

        private string _customerEmail = string.Empty;
        public string CustomerEmail
        {
            get => _customerEmail;
            set => SetField(ref _customerEmail, value);
        }

        private string? _customerRequestDescription;
        public string? CustomerRequestDescription
        {
            get => _customerRequestDescription;
            set => SetField(ref _customerRequestDescription, value);
        }

        private int _requestsCount;
        public int RequestsCount
        {
            get => _requestsCount;
            set => SetField(ref _requestsCount, value);
        }

        private int _solvedRequestsCount;
        public int SolvedRequestsCount
        {
            get => _solvedRequestsCount;
            set => SetField(ref _solvedRequestsCount, value);
        }

        private int _pendingRequestsCount;
        public int PendingRequestsCount
        {
            get => _pendingRequestsCount;
            set => SetField(ref _pendingRequestsCount, value);
        }

        // Read-only property for display
        public string RequestsSummary => $"{RequestsCount} (✓{SolvedRequestsCount} ✗{PendingRequestsCount})";

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
    }
}