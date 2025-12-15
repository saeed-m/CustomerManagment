using CustomerManagment.WpfApp.Data.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomerManagment.WpfApp.ViewModels
{
    public class CustomerRequestViewModel : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        private RequestStatus _status;
        public RequestStatus Status
        {
            get => _status;
            set
            {
                SetField(ref _status, value);
                OnPropertyChanged(nameof(StatusDisplayName));
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(StatusIcon));
                OnPropertyChanged(nameof(IsSolved)); // Keep this for backward compatibility if needed
            }
        }

        // For backward compatibility - returns true if status is Solved
        public bool IsSolved => Status == RequestStatus.Solved;

        public string StatusDisplayName => Status.GetDisplayName();
        public string StatusColor => Status.GetColor();
        public string StatusIcon => Status.GetIcon();
        public string StatusDescription => Status.GetDescription();

        private DateTime _createDate;
        public DateTime CreateDate
        {
            get => _createDate;
            set
            {
                SetField(ref _createDate, value);
                OnPropertyChanged(nameof(CreateDateFormatted));
            }
        }

        public string CreateDateFormatted => CreateDate.ToString("dd/MM/yyyy HH:mm");

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