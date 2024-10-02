using Microsoft.Maui.Controls;

namespace BKWitten_App_Frontend.Pages
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value) // Dark Mode aktiviert
            {
                BackgroundColor = Color.FromArgb("#222222"); // Dunkler Hintergrund
                modeLabel.Text = "Mode: Dark"; // Label für Dark Mode

                // Optional: Schriftfarbe anpassen
                foreach (var child in (this.Content as VerticalStackLayout).Children)
                {
                    if (child is Label label)
                    {
                        label.TextColor = Color.FromRgb(255, 255, 255); // Schriftfarbe auf Weiß
                    }
                    else if (child is Picker picker)
                    {
                        picker.TextColor = Color.FromRgb(255, 255, 255); // Schriftfarbe des Pickers auf Weiß
                    }
                }
            }
            else // Normal Mode aktiviert
            {
                BackgroundColor = Color.FromRgb(255, 255, 255); // Heller Hintergrund
                modeLabel.Text = "Mode: Normal"; // Label für Normal Mode

                // Optional: Schriftfarbe zurücksetzen
                foreach (var child in (this.Content as VerticalStackLayout).Children)
                {
                    if (child is Label label)
                    {
                        label.TextColor = Color.FromRgb(0, 0, 0); // Schriftfarbe auf Schwarz
                    }
                    else if (child is Picker picker)
                    {
                        picker.TextColor = Color.FromRgb(0, 0, 0); // Schriftfarbe des Pickers auf Schwarz
                    }
                }
            }
        }
    }
}
