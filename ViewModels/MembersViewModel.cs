using FitZoneGymScheduler.Commands;
using FitZoneGymScheduler.DAL;
using FitZoneGymScheduler.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FitZoneGymScheduler.Views;
using FitZoneGymScheduler.Services;

namespace FitZoneGymScheduler.ViewModels
{
    public class MembersViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Member> Members { get; set; }

        // =========================
        // SELECTED MEMBER
        // =========================

        private Member _selectedMember;

        public Member SelectedMember
        {
            get => _selectedMember;

            set
            {
                _selectedMember = value;

                OnPropertyChanged();

                if (_selectedMember == null)
                    return;

                FullName = _selectedMember.FullName;
                Age = _selectedMember.Age;
                Gender = _selectedMember.Gender;
                Country = _selectedMember.Country;

                Height = _selectedMember.Height;
                HeightUnit = _selectedMember.HeightUnit;

                Weight = _selectedMember.Weight;
                WeightUnit = _selectedMember.WeightUnit;

                BMI = _selectedMember.BMI;
                BMIWord = _selectedMember.BMIWord;

                FitnessLevel = _selectedMember.FitnessLevel;
                FitnessGoal = _selectedMember.FitnessGoal;

                PhoneNumber = _selectedMember.PhoneNumber;

                Notes = _selectedMember.Notes;
            }
        }

        private void RefreshStatistics()
        {
            OnPropertyChanged(nameof(TotalMembers));
            OnPropertyChanged(nameof(MaleMembers));
            OnPropertyChanged(nameof(FemaleMembers));
            OnPropertyChanged(nameof(AverageBMI));
        }


        // =========================
        // FORM FIELDS
        // =========================

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();

                CalculateBMI();
            }
        }

        private string _heightUnit;
        public string HeightUnit
        {
            get => _heightUnit;
            set
            {
                _heightUnit = value;
                OnPropertyChanged();

                CalculateBMI();
            }
        }

        private double _weight;
        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged();

                CalculateBMI();
            }
        }

        private string _weightUnit;
        public string WeightUnit
        {
            get => _weightUnit;
            set
            {
                _weightUnit = value;
                OnPropertyChanged();

                CalculateBMI();
            }
        }

        private double _bmi;
        public double BMI
        {
            get => _bmi;
            set
            {
                _bmi = value;
                OnPropertyChanged();
            }
        }

        private string _bmiWord;
        public string BMIWord
        {
            get => _bmiWord;
            set
            {
                _bmiWord = value;
                OnPropertyChanged();
            }
        }

        private string _fitnessLevel;
        public string FitnessLevel
        {
            get => _fitnessLevel;
            set
            {
                _fitnessLevel = value;
                OnPropertyChanged();
            }
        }

        private string _fitnessGoal;
        public string FitnessGoal
        {
            get => _fitnessGoal;
            set
            {
                _fitnessGoal = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        // =========================
        // SEARCH
        // =========================

        private string _searchText;

        public string SearchText
        {
            get => _searchText;

            set
            {
                _searchText = value;

                OnPropertyChanged();

                SearchMembers();
            }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand SaveMemberCommand { get; }
        public ICommand UpdateMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public MembersViewModel()
        {
            _context = new AppDbContext();

            Members = new ObservableCollection<Member>();

            SaveMemberCommand =
                new RelayCommand(SaveMember);

            UpdateMemberCommand =
                new RelayCommand(UpdateMember);

            DeleteMemberCommand =
                new RelayCommand(DeleteMember);

            LoadMembers();
        }

        // =========================
        // BMI CALCULATION
        // =========================

        private void CalculateBMI()
        {
            if (Height <= 0 || Weight <= 0)
                return;

            double heightInMeters = 0;
            double weightInKg = 0;

            // HEIGHT

            if (HeightUnit == "cm")
            {
                heightInMeters = Height / 100;
            }
            else if (HeightUnit == "ft")
            {
                heightInMeters = Height * 0.3048;
            }
            else if (HeightUnit == "inch")
            {
                heightInMeters = Height * 0.0254;
            }

            // WEIGHT

            if (WeightUnit == "kg")
            {
                weightInKg = Weight;
            }
            else if (WeightUnit == "lb")
            {
                weightInKg = Weight * 0.453592;
            }

            if (heightInMeters <= 0)
                return;

            BMI = Math.Round(
                weightInKg / (heightInMeters * heightInMeters), 1);

            // STATUS

            if (BMI < 18.5)
            {
                BMIWord = "Underweight";
            }
            else if (BMI < 25)
            {
                BMIWord = "Normal";
            }
            else if (BMI < 30)
            {
                BMIWord = "Overweight";
            }
            else
            {
                BMIWord = "Obese";
            }
        }

        // =========================
        // VALIDATION
        // =========================

        private bool ValidateMember()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please enter the member's full name.");

                return false;
            }

            if (Age <= 0)
            {
                DialogService.Warning(
                    "Invalid Age",
                    "Please enter a valid age greater than zero.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a gender.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(Country))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a country.");

                return false;
            }

            if (Height <= 0)
            {
                DialogService.Warning(
                    "Invalid Height",
                    "Please enter a valid height.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(HeightUnit))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a height unit.");

                return false;
            }

            if (Weight <= 0)
            {
                DialogService.Warning(
                    "Invalid Weight",
                    "Please enter a valid weight.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(WeightUnit))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a weight unit.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(FitnessLevel))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a fitness level.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(FitnessGoal))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please select a fitness goal.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                DialogService.Warning(
                    "Missing Information",
                    "Please enter a phone number.");

                return false;
            }

            if (PhoneNumber.Length < 10)
            {
                DialogService.Warning(
                    "Invalid Phone Number",
                    "Please enter a valid phone number.");

                return false;
            }

            return true;
        }







        // =========================
        // SAVE
        // =========================

        private void SaveMember(object obj)
        {
            if (!ValidateMember())
                return;

            if (!DialogService.Confirm(
                "Add Member",
                $"Are you sure you want to register {FullName} as a new member?"))
            {
                return;
            }

            try
            {
                bool exists = _context.Members.Any(x =>
                    x.FullName == FullName &&
                    x.PhoneNumber == PhoneNumber);

                if (exists)
                {
                    DialogService.Warning(
                        "Duplicate Member",
                        "A member with the same name and phone number already exists.");

                    return;
                }

                Member member = new Member
                {
                    FullName = FullName,
                    Age = Age,
                    Gender = Gender,
                    Country = Country,

                    Height = Height,
                    HeightUnit = HeightUnit,

                    Weight = Weight,
                    WeightUnit = WeightUnit,

                    BMI = BMI,
                    BMIWord = BMIWord,

                    FitnessLevel = FitnessLevel,
                    FitnessGoal = FitnessGoal,

                    PhoneNumber = PhoneNumber,

                    JoinDate = DateTime.Now,

                    Notes = string.IsNullOrWhiteSpace(Notes)
                        ? "No additional notes provided."
                        : Notes
                };

                _context.Members.Add(member);

                _context.SaveChanges();

                Members.Add(member);

                RefreshStatistics();

                DialogService.Success(
                    "Member Added",
                    $"{FullName} has been successfully registered.");

                ClearFields();
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Unexpected Error",
                    ex.Message);
            }
        }

        // =========================
        // UPDATE
        // =========================

        private void UpdateMember(object obj)
        {
            try
            {
                if (SelectedMember == null)
                {
                    DialogService.Warning(
                        "No Member Selected",
                        "Please select a member to update.");

                    return;
                }

                if (!ValidateMember())
                    return;

                if (!DialogService.Confirm(
                    "Update Member",
                    $"Are you sure you want to update {SelectedMember.FullName}'s information?"))
                {
                    return;
                }

                SelectedMember.FullName = FullName;
                SelectedMember.Age = Age;
                SelectedMember.Gender = Gender;
                SelectedMember.Country = Country;

                SelectedMember.Height = Height;
                SelectedMember.HeightUnit = HeightUnit;

                SelectedMember.Weight = Weight;
                SelectedMember.WeightUnit = WeightUnit;

                SelectedMember.BMI = BMI;
                SelectedMember.BMIWord = BMIWord;

                SelectedMember.FitnessLevel = FitnessLevel;
                SelectedMember.FitnessGoal = FitnessGoal;

                SelectedMember.PhoneNumber = PhoneNumber;

                SelectedMember.Notes =
                    string.IsNullOrWhiteSpace(Notes)
                        ? "No additional notes provided."
                        : Notes;

                _context.SaveChanges();

                LoadMembers();

                RefreshStatistics();

                DialogService.Success(
                    "Update Complete",
                    "Member information has been updated successfully.");
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Unexpected Error",
                    ex.Message);
            }
        }


        //Delete

        private void DeleteMember(object obj)
        {
            try
            {
                if (SelectedMember == null)
                {
                    DialogService.Warning(
                        "No Member Selected",
                        "Please select a member to delete.");

                    return;
                }

                if (!DialogService.Confirm(
                    "Delete Member",
                    $"Are you sure you want to delete {SelectedMember.FullName}?\n\nThis action cannot be undone?"))
                {
                    return;
                }

                _context.Members.Remove(SelectedMember);

                _context.SaveChanges();

                Members.Remove(SelectedMember);

                RefreshStatistics();

                ClearFields();

                DialogService.Success(
                    "Delete Complete",
                    "The member record has been removed successfully.");
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Unexpected Error",
                    ex.Message);
            }
        }

        // =========================
        // LOAD MEMBERS
        // =========================

        private void LoadMembers()
        {
            var membersFromDb =
                _context.Members.ToList();

            Members.Clear();

            foreach (var member in membersFromDb)
            {
                Members.Add(member);
            }
        }

        // =========================
        // SEARCH
        // =========================

        private void SearchMembers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadMembers();
                return;
            }

            var filteredMembers =
                _context.Members
                .Where(m =>
                    m.FullName.Contains(SearchText) ||
                    m.PhoneNumber.Contains(SearchText) ||
                    m.Country.Contains(SearchText))
                .ToList();

            Members.Clear();

            foreach (var member in filteredMembers)
            {
                Members.Add(member);
            }
        }

        // =========================
        // CLEAR FORM
        // =========================

        private void ClearFields()
        {
            FullName = "";
            Age = 0;
            Gender = "";

            Country = "";

            Height = 0;
            HeightUnit = "";

            Weight = 0;
            WeightUnit = "";

            BMI = 0;
            BMIWord = "";

            FitnessLevel = "";
            FitnessGoal = "";

            PhoneNumber = "";

            Notes = "";

            SelectedMember = null;
        }

        public int TotalMembers => Members.Count;

        public int MaleMembers =>
            Members.Count(x => x.Gender == "Male");

        public int FemaleMembers =>
            Members.Count(x => x.Gender == "Female");

        public double AverageBMI =>
            Members.Any()
                ? Math.Round(Members.Average(x => x.BMI), 1)
                : 0;
    }


}