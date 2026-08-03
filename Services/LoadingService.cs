using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FitZoneGymScheduler.Services
{
    public class LoadingService : INotifyPropertyChanged
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _message = "Loading...";

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public void Show(string message)
        {
            Message = message;
            IsLoading = true;
        }

        public void Hide()
        {
            IsLoading = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}