using FitZoneGymScheduler.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System;
using System.Windows.Forms;
using FitZoneGymScheduler.Services;
using WpfColor = System.Windows.Media.Color;
using WpfColors = System.Windows.Media.Colors;

using PdfColor = QuestPDF.Infrastructure.Color;
using PdfColors = QuestPDF.Helpers.Colors;
using QRCoder;
using SkiaSharp;






namespace FitZoneGymScheduler.Views
{
    public partial class PdfPreviewWindow : Window
    {

        private void ChangePdfFolder_Click(
    object sender,
    RoutedEventArgs e)
        {
            var settings =
                SettingsService.Load();

            using FolderBrowserDialog dialog =
                new FolderBrowserDialog();

            if (dialog.ShowDialog() ==
                System.Windows.Forms.DialogResult.OK)
            {
                settings.PdfFolderPath =
                    dialog.SelectedPath;

                SettingsService.Save(settings);

                System.Windows.MessageBox.Show(
                    "PDF folder updated successfully.");
            }
        }

        private readonly WorkoutPlansViewModel _plan;

        public PdfPreviewWindow(WorkoutPlansViewModel plan)
        {
            InitializeComponent();

            _plan = plan;

            LoadPreview();
        }

        private void LoadPreview()
        {
            // =====================================
            // PLAN TITLE
            // =====================================

            PreviewContainer.Children.Add(
                new TextBlock
                {
                    Text = _plan.PlanName,
                    FontSize = 26,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                });

            // =====================================
            // MEMBER CARD
            // =====================================

            var memberCard =
                new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 25),
                    Background = Brushes.WhiteSmoke
                };

            var memberStack =
                new StackPanel();

            memberStack.Children.Add(
                new TextBlock
                {
                    Text = $"Member : {_plan.SelectedMember?.FullName}",
                    FontWeight = FontWeights.Bold,
                    FontSize = 16
                });

            memberStack.Children.Add(
                new TextBlock
                {
                    Text = $"Goal : {_plan.Goal}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

            memberStack.Children.Add(
                new TextBlock
                {
                    Text = $"Difficulty : {_plan.Difficulty}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

            memberStack.Children.Add(
                new TextBlock
                {
                    Text = $"Created : {System.DateTime.Now:dd MMM yyyy}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

            memberCard.Child = memberStack;

            PreviewContainer.Children.Add(memberCard);

            // =====================================
            // DAYS
            // =====================================

            foreach (var day in _plan.WorkoutDays)
            {
                // DAY HEADER

                var dayHeader =
                    new Border
                    {
                        Background =
                            new SolidColorBrush(WpfColor.FromRgb(17, 24, 39)),

                        CornerRadius =
                            new CornerRadius(6),

                        Padding =
                            new Thickness(12),

                        Margin =
                            new Thickness(0, 15, 0, 10)
                    };

                dayHeader.Child =
                    new TextBlock
                    {
                        Text = $"DAY {day.DayNumber}",
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        FontSize = 18
                    };

                PreviewContainer.Children.Add(dayHeader);

                // REST DAY

                if (day.IsRestDay)
                {
                    PreviewContainer.Children.Add(
                        new Border
                        {
                            Background =
                                new SolidColorBrush(WpfColor.FromRgb(255, 235, 235)),

                            BorderBrush =
                                Brushes.Red,

                            BorderThickness =
                                new Thickness(1),

                            CornerRadius =
                                new CornerRadius(6),

                            Padding =
                                new Thickness(12),

                            Child =
                                new TextBlock
                                {
                                    Text = "REST DAY",
                                    FontWeight = FontWeights.Bold,
                                    Foreground = Brushes.DarkRed
                                }
                        });

                    continue;
                }

                // =====================================
                // SECTIONS
                // =====================================

                foreach (var section in day.Sections)
                {
                    var sectionHeader =
                        new Border
                        {
                            Background =
                               new SolidColorBrush(WpfColor.FromRgb(55, 65, 81)),

                            CornerRadius =
                                new CornerRadius(5),

                            Padding =
                                new Thickness(10),

                            Margin =
                                new Thickness(15, 10, 0, 5)
                        };

                    sectionHeader.Child =
                        new TextBlock
                        {
                            Text = section.SectionName,
                            Foreground = Brushes.White,
                            FontWeight = FontWeights.Bold,
                            FontSize = 15
                        };

                    PreviewContainer.Children.Add(sectionHeader);


                    var tableHeader =
    new Border
    {
        Background =
            new SolidColorBrush(WpfColor.FromRgb(230, 230, 230)),

        BorderBrush =
            Brushes.LightGray,

        BorderThickness =
            new Thickness(1),

        Margin =
            new Thickness(25, 5, 0, 0),

        Padding =
            new Thickness(8)
    };

                    var headerGrid =
                        new Grid();

                    headerGrid.ColumnDefinitions.Add(
                        new ColumnDefinition
                        {
                            Width =
                                new GridLength(3, GridUnitType.Star)
                        });

                    headerGrid.ColumnDefinitions.Add(
                        new ColumnDefinition
                        {
                            Width =
                                new GridLength(1, GridUnitType.Star)
                        });

                    headerGrid.ColumnDefinitions.Add(
                        new ColumnDefinition
                        {
                            Width =
                                new GridLength(1.5, GridUnitType.Star)
                        });

                    headerGrid.ColumnDefinitions.Add(
                        new ColumnDefinition
                        {
                            Width =
                                new GridLength(2, GridUnitType.Star)
                        });

                    var h1 =
                        new TextBlock
                        {
                            Text = "Exercise",
                            FontWeight = FontWeights.Bold
                        };

                    var h2 =
                        new TextBlock
                        {
                            Text = "Sets",
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment =
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                        };

                    Grid.SetColumn(h2, 1);

                    var h3 =
                        new TextBlock
                        {
                            Text = "Reps",
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment =
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                        };

                    Grid.SetColumn(h3, 2);

                    var h4 =
                        new TextBlock
                        {
                            Text = "Notes",
                            FontWeight = FontWeights.Bold
                        };

                    Grid.SetColumn(h4, 3);

                    headerGrid.Children.Add(h1);
                    headerGrid.Children.Add(h2);
                    headerGrid.Children.Add(h3);
                    headerGrid.Children.Add(h4);

                    tableHeader.Child = headerGrid;

                    PreviewContainer.Children.Add(tableHeader);

                    // =====================================
                    // EXERCISES
                    // =====================================

                    foreach (var exercise in section.Exercises)
                    {
                        var exerciseCard =
                            new Border
                            {
                                BorderBrush = Brushes.LightGray,
                                BorderThickness = new Thickness(1),
                                CornerRadius = new CornerRadius(5),
                                Padding = new Thickness(10),
                                Margin = new Thickness(25, 5, 0, 5)
                            };

                        var grid =
                            new Grid();

                        grid.ColumnDefinitions.Add(
          new ColumnDefinition
          {
              Width =
                  new GridLength(3, GridUnitType.Star)
          });

                        grid.ColumnDefinitions.Add(
                            new ColumnDefinition
                            {
                                Width =
                                    new GridLength(1, GridUnitType.Star)
                            });

                        grid.ColumnDefinitions.Add(
                            new ColumnDefinition
                            {
                                Width =
                                    new GridLength(1.5, GridUnitType.Star)
                            });

                        grid.ColumnDefinitions.Add(
                            new ColumnDefinition
                            {
                                Width =
                                    new GridLength(2, GridUnitType.Star)
                            });

                        // Exercise Name

                        var exerciseName =
                            new TextBlock
                            {
                                Text = exercise.ExerciseName,
                                FontWeight = FontWeights.SemiBold
                            };

                        Grid.SetColumn(exerciseName, 0);

                        // Sets

                        var sets =
                            new TextBlock
                            {
                                Text = exercise.Sets.ToString(),
                                HorizontalAlignment =
                                   HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                            };

                        Grid.SetColumn(sets, 1);

                        // Reps

                        var reps =
                            new TextBlock
                            {
                                Text = exercise.RepsOrDuration,
                                HorizontalAlignment =
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                            };



                        Grid.SetColumn(reps, 2);

                        grid.Children.Add(exerciseName);
                        grid.Children.Add(sets);
                        grid.Children.Add(reps);

                        var notes =
    new TextBlock
    {
        Text = exercise.Notes
    };

                        Grid.SetColumn(notes, 3);

                        grid.Children.Add(notes);

                        exerciseCard.Child = grid;

                        PreviewContainer.Children.Add(exerciseCard);
                    }


                }
            }
        }


        private void DownloadPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dialog = new()
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = "WorkoutPlan.pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    SavePdf(dialog.FileName);

                    System.Windows.MessageBox.Show(
                        "PDF downloaded successfully.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        private void SendWhatsapp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var member = _plan.SelectedMember;

                if (member == null)
                {
                    System.Windows.MessageBox.Show("No member selected.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(member.PhoneNumber))
                {
                    System.Windows.MessageBox.Show("Member has no phone number.");
                    return;
                }

                // CLEAN PHONE NUMBER (Sri Lanka format safe)
                var phone = member.PhoneNumber
                    .Replace("+", "")
                    .Replace(" ", "")
                    .Trim();

                // Optional: force Sri Lanka country code if user enters local number
                if (phone.StartsWith("0"))
                {
                    phone = "94" + phone.Substring(1);
                }

                // MESSAGE
                var message =
                    $"Hello {member.FullName},\n\n" +
                    $"Your FitZone workout plan *{_plan.PlanName}* is ready.\n\n" +
                    $"You can now follow your training schedule.\n\n" +
                    $"Stay consistent 💪🔥";

                // WHATSAPP URL
                var url = $"https://wa.me/{phone}?text={Uri.EscapeDataString(message)}";

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("WhatsApp error: " + ex.Message);
            }
        }


       // save logic

        private void SavePdf(string filePath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

           

            Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);

                    // =========================
                    // FOOTER
                    // =========================
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("FitZone Gym • ");
                            x.Span(DateTime.Now.ToString("dd MMM yyyy"));
                        });

                    // =========================
                    // MAIN CONTENT
                    // =========================
                    page.Content().Column(col =>
                    {
                        // =========================
                        // COVER PAGE
                        // =========================
                        col.Item().Element(container =>
                        {
                            container.Column(c =>
                            {
                                var logo = LoadLogo("Assets/logo.jpg");

                                if (logo != null)
                                {
                                    c.Item()
                                        .AlignCenter()
                                        .Height(120)
                                        .Image(logo);
                                }

                                // TITLE
                                c.Item()
                                    .PaddingTop(10)
                                    .AlignCenter()
                                    .Text("FITZONE WORKOUT PLAN")
                                    .FontSize(22)
                                    .Bold();

                                c.Item()
                                    .AlignCenter()
                                    .Text(_plan.PlanName)
                                    .FontSize(14)
                                    .FontColor(PdfColors.Grey.Darken1);

                                // INFO BOX
                                c.Item()
                                    .PaddingTop(20)
                                    .Background(PdfColors.Grey.Lighten4)
                                    .Padding(15)
                                    .Column(info =>
                                    {
                                        info.Item().Text($"Member: {_plan.SelectedMember?.FullName ?? "N/A"}");
                                        info.Item().Text($"Goal: {_plan.Goal ?? "N/A"}");
                                        info.Item().Text($"Difficulty: {_plan.Difficulty ?? "N/A"}");
                                        info.Item().Text($"Date: {DateTime.Now:dd MMM yyyy}");
                                    });

                                // SUMMARY
                                c.Item()
                                    .PaddingTop(10)
                                    .Text($"Days: {_plan.WorkoutDays.Count} | " +
                                          $"Sections: {_plan.WorkoutDays.Sum(d => d.Sections.Count)} | " +
                                          $"Exercises: {_plan.WorkoutDays.Sum(d => d.Sections.Sum(s => s.Exercises.Count))}")
                                    .FontSize(11);

                                
                            });
                        });

                        col.Item().PageBreak();

                        // =========================
                        // DAY PAGES
                        // =========================
                        foreach (var day in _plan.WorkoutDays)
                        {
                            col.Item().Element(container =>
                            {
                                container.Column(c =>
                                {
                                    // DAY HEADER
                                    c.Item()
                                        .Background(PdfColors.Black)
                                        .Padding(10)
                                        .AlignCenter()
                                        .Text($"DAY {day.DayNumber}")
                                        .FontColor(PdfColors.White)
                                        .Bold();

                                    if (day.IsRestDay)
                                    {
                                        c.Item()
                                            .Background(PdfColors.Red.Lighten4)
                                            .Border(1)
                                            .Padding(10)
                                            .AlignCenter()
                                            .Text("REST DAY");

                                        return;
                                    }

                                    foreach (var section in day.Sections)
                                    {
                                        // SECTION HEADER
                                        c.Item()
                                            .PaddingTop(10)
                                            .Background(PdfColors.Grey.Darken1)
                                            .Padding(6)
                                            .AlignCenter()
                                            .Text(section.SectionName)
                                            .FontColor(PdfColors.White)
                                            .Bold();

                                        // TABLE HEADER
                                        c.Item()
                                            .Border(1)
                                            .BorderColor(PdfColors.Grey.Lighten2)
                                            .Background(PdfColors.Grey.Lighten3)
                                            .Row(row =>
                                            {
                                                row.RelativeItem().Padding(5).AlignCenter().Text("Exercise").Bold();
                                                row.ConstantItem(60).Padding(5).AlignCenter().Text("Sets").Bold();
                                                row.ConstantItem(80).Padding(5).AlignCenter().Text("Reps").Bold();
                                                row.RelativeItem().Padding(5).AlignCenter().Text("Notes").Bold();
                                            });

                                        // ROWS
                                        foreach (var ex in section.Exercises)
                                        {
                                            c.Item()
                                                .BorderBottom(1)
                                                .BorderColor(PdfColors.Grey.Lighten3)
                                                .Row(row =>
                                                {
                                                    row.RelativeItem()
                                                        .Padding(5)
                                                        .AlignLeft()
                                                        .Text(ex.ExerciseName);

                                                    row.ConstantItem(60)
                                                        .Padding(5)
                                                        .AlignCenter()
                                                        .Text(ex.Sets.ToString());

                                                    row.ConstantItem(80)
                                                        .Padding(5)
                                                        .AlignCenter()
                                                        .Text(ex.RepsOrDuration);

                                                    row.RelativeItem()
                                                        .Padding(5)
                                                        .AlignLeft()
                                                        .Text(ex.Notes ?? "-");
                                                });
                                        }
                                    }
                                });
                            });

                            col.Item().PageBreak();
                        }
                    });
                });
            })
            .GeneratePdf(filePath);
        }



        private static class PdfTheme
        {
            public static readonly string Primary = "#111827";   // dark
            public static readonly string Accent = "#22C55E";    // green
            public static readonly string Light = "#F3F4F6";
            public static readonly string Border = "#E5E7EB";
        }


        private byte[]? LoadLogo(string relativePath)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            return File.Exists(path)
                ? File.ReadAllBytes(path)
                : null;
        }
    }

}
    
    

