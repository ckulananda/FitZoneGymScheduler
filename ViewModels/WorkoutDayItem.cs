using FitZoneGymScheduler.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FitZoneGymScheduler.ViewModels
{
    public class WorkoutDayItem : BaseViewModel
    {
        private int _dayNumber;

        public int DayNumber
        {
            get => _dayNumber;

            set
            {
                _dayNumber = value;

                OnPropertyChanged();
            }
        }

        private bool _isRestDay;

        public bool IsRestDay
        {
            get => _isRestDay;

            set
            {
                _isRestDay = value;

                OnPropertyChanged();
            }
        }

        // SECTIONS

        public ObservableCollection<WorkoutSectionItem> Sections
        {
            get;
            set;
        }

        // COMMAND

        public ICommand AddSectionCommand { get; }
        public ICommand RemoveDayCommand { get; }

        public ObservableCollection<WorkoutDayItem> ParentCollection
        {
            get;
            set;
        }

        public WorkoutDayItem()
        {
            Sections =
                new ObservableCollection<WorkoutSectionItem>();

            AddSectionCommand =
                new RelayCommand(AddSection);

            RemoveDayCommand =
                new RelayCommand(RemoveDay);
        }

        // ADD SECTION

        private void AddSection(object obj)
        {
            Sections.Add(new WorkoutSectionItem());
        }

        private void RemoveDay(object obj)
        {
            ParentCollection?.Remove(this);

            // REORDER DAYS

            if (ParentCollection != null)
            {
                for (int i = 0; i < ParentCollection.Count; i++)
                {
                    ParentCollection[i].DayNumber = i + 1;
                }
            }
        }
    }
}