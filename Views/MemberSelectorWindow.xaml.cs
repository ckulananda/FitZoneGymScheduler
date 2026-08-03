using FitZoneGymScheduler.Models;
using FitZoneGymScheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FitZoneGymScheduler.Views
{
    public partial class MemberSelectorWindow : Window
    {
        private List<Member> _allMembers;

        public Member SelectedMember { get; private set; }

        public MemberSelectorWindow(List<Member> members)
        {
            InitializeComponent();

            try
            {
                _allMembers = members ?? new List<Member>();

                MembersGrid.ItemsSource = _allMembers;

                if (!_allMembers.Any())
                {
                    DialogService.Warning(
                        "No Members",
                        "There are currently no members available to select.");
                }
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Loading Failed",
                    $"Unable to load members.\n\n{ex.Message}");
            }
        }

        private void SearchBox_TextChanged(
     object sender,
     TextChangedEventArgs e)
        {
           
            string search =
                SearchBox.Text?.Trim().ToLower() ?? "";

            var filteredMembers =
                _allMembers.Where(m =>
                    (m.FullName ?? "")
                        .ToLower()
                        .Contains(search)

                    ||

                    (m.PhoneNumber ?? "")
                        .ToLower()
                        .Contains(search)

                    ||

                    (m.Country ?? "")
                        .ToLower()
                        .Contains(search))
                .ToList();

            MembersGrid.ItemsSource = filteredMembers;

            EmptyStateText.Visibility =
                filteredMembers.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }


        private void MembersGrid_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            try
            {
                if (MembersGrid.SelectedItem is not Member member)
                {
                    DialogService.Warning(
                        "Selection Required",
                        "Please select a member first.");
                    return;
                }

                bool confirmed =
                    DialogService.Confirm(
                        "Confirm Selection",
                        $"Select {member.FullName} for this workout plan?");

                if (!confirmed)
                    return;

                SelectedMember = member;

                DialogService.Success(
                    "Member Selected",
                    $"{member.FullName} has been selected successfully.");

                DialogResult = true;
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Selection Error",
                    ex.Message);
            }
        }

        private void SelectButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (sender is not Button button)
                {
                    DialogService.Warning(
                        "Invalid Selection",
                        "Unable to determine selected member.");
                    return;
                }

                if (button.Tag is not Member member)
                {
                    DialogService.Warning(
                        "Invalid Selection",
                        "Unable to determine selected member.");
                    return;
                }

                bool confirmed =
                    DialogService.Confirm(
                        "Confirm Selection",
                        $"Select {member.FullName} for this workout plan?");

                if (!confirmed)
                    return;

                SelectedMember = member;

                DialogService.Success(
                    "Member Selected",
                    $"{member.FullName} has been selected successfully.");

                DialogResult = true;
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Selection Error",
                    ex.Message);
            }
        }

        private void CloseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                bool confirmed =
                    DialogService.Confirm(
                        "Close Window",
                        "Are you sure you want to close the member selector?");

                if (!confirmed)
                    return;

                Close();
            }
            catch (Exception ex)
            {
                DialogService.Error(
                    "Close Error",
                    ex.Message);
            }
        }

        protected override void OnClosing(
            System.ComponentModel.CancelEventArgs e)
        {
            if (SelectedMember == null)
            {
                bool confirmed =
                    DialogService.Confirm(
                        "No Member Selected",
                        "No member has been selected. Close anyway?");

                if (!confirmed)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }
    }
}