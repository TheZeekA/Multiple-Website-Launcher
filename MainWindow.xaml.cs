using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;

namespace MorningWebsiteLauncher
{
    public partial class MainWindow : Window
    {
        // Default websites - these will be added if no saved file exists
        private readonly List<string> defaultWebsites = new List<string>
    {
        "https://www.gmail.com",
        "https://www.google.com",
        "https://www.reddit.com",
        "https://www.youtube.com",
        "https://www.weather.com",
        "https://www.bbc.com/news",
        "https://www.github.com"
    };

        private List<string> websites = new List<string>();
        private readonly string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MorningLauncher", "websites.txt");

        public MainWindow()
        {
            InitializeComponent();
            LoadWebsites();
            PopulateWebsiteList();
        }
        private void LoadWebsites()
        {
            try
            {
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));

                if (File.Exists(settingsFile))
                {
                    // Load websites from file
                    websites = File.ReadAllLines(settingsFile).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

                    // If file is empty or corrupted, use defaults
                    if (websites.Count == 0)
                    {
                        websites = new List<string>(defaultWebsites);
                        SaveWebsites();
                    }
                }
                else
                {
                    // First time running - use default websites
                    websites = new List<string>(defaultWebsites);
                    SaveWebsites();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading websites: {ex.Message}\nUsing default websites.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                websites = new List<string>(defaultWebsites);
            }
        }

        private void SaveWebsites()
        {
            try
            {
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));

                // Save websites to file
                File.WriteAllLines(settingsFile, websites);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving websites: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PopulateWebsiteList()
        {
            websiteListBox.Items.Clear();
            foreach (string website in websites)
            {
                websiteListBox.Items.Add(website);
            }
        }

        private void LaunchAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                statusLabel.Content = "Opening websites...";

                foreach (string website in websites)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = website,
                        UseShellExecute = true
                    });

                    System.Threading.Thread.Sleep(500);
                }

                statusLabel.Content = "Successfully opened " + websites.Count + " websites!";
            }
            catch (Exception ex)
            {
                statusLabel.Content = "Error: " + ex.Message;
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            string newWebsite = newWebsiteTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newWebsite))
            {
                MessageBox.Show("Please enter a website URL.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!newWebsite.StartsWith("http://") && !newWebsite.StartsWith("https://"))
            {
                newWebsite = "https://" + newWebsite;
            }

            websites.Add(newWebsite);
            SaveWebsites(); // Save to file immediately
            PopulateWebsiteList();
            newWebsiteTextBox.Clear();
            statusLabel.Content = "Added: " + newWebsite;
        }

        private void RemoveWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            if (websiteListBox.SelectedItem != null)
            {
                string selectedWebsite = websiteListBox.SelectedItem.ToString();
                websites.Remove(selectedWebsite);
                SaveWebsites(); // Save to file immediately
                PopulateWebsiteList();
                statusLabel.Content = "Removed: " + selectedWebsite;
            }
            else
            {
                MessageBox.Show("Please select a website to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
