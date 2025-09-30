using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GempaBMKG
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private DispatcherTimer refreshTimer;
        private string currentMode = "terkini";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromMinutes(5);
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (currentMode == "realtime")
            {
                _ = FetchDataAsync(currentMode);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await FetchDataAsync("terkini");
        }

        private async Task FetchDataAsync(string mode)
        {
            LoadingIndicator.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Collapsed;

            string url = "";
            if (mode == "terkini")
            {
                url = "https://data.bmkg.go.id/DataMKG/TEWS/gempaterkini.json";
            }
            else
            {
                url = "https://data.bmkg.go.id/DataMKG/TEWS/autogempa.json";
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                if (mode == "terkini")
                {
                    var data = JsonConvert.DeserializeObject<GempaTerkiniRoot>(responseBody);
                    UpdateContent(data?.Infogempa?.gempa);
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<GempaRealtimeRoot>(responseBody);
                    var gempaInfo = data?.Infogempa?.gempa;
                    if (gempaInfo != null)
                    {
                        UpdateContent(new List<GempaInfo> { gempaInfo });
                    }
                    else
                    {
                        UpdateContent(new List<GempaInfo>());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Gagal mengambil data: {ex.Message}";
                ErrorText.Visibility = Visibility.Visible;
            }
            finally
            {
                LoadingIndicator.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;
            }
        }

        private void UpdateContent(List<GempaInfo>? gempaList)
        {
            ContentStackPanel.Children.Clear(); 

            if (gempaList == null || !gempaList.Any())
            {
                ContentStackPanel.Children.Add(new TextBlock { Text = "Tidak ada data gempa untuk ditampilkan.", Foreground = Brushes.White, FontSize = 16 });
                return;
            }

            foreach (var gempa in gempaList)
            {
                if (gempa == null) continue;

                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x2D)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var stackPanel = new StackPanel { Margin = new Thickness(15, 0, 0, 0) };
                Grid.SetColumn(stackPanel, 1);

                stackPanel.Children.Add(new TextBlock { Text = gempa.Wilayah ?? "N/A", Foreground = Brushes.White, FontSize = 18, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap });
                stackPanel.Children.Add(new TextBlock { Text = $"{gempa.Tanggal ?? "N/A"} - {gempa.Jam ?? "N/A"}", Foreground = Brushes.LightGray, Margin = new Thickness(0, 5, 0, 10) });
                stackPanel.Children.Add(new TextBlock { Text = $"Magnitude: {gempa.Magnitude ?? "N/A"} | Kedalaman: {gempa.Kedalaman ?? "N/A"}", Foreground = Brushes.White, Margin = new Thickness(0, 2, 0, 2) });
                stackPanel.Children.Add(new TextBlock { Text = $"Lokasi: {gempa.Lintang ?? "N/A"} | {gempa.Bujur ?? "N/A"}", Foreground = Brushes.White, Margin = new Thickness(0, 2, 0, 2) });
                stackPanel.Children.Add(new TextBlock { Text = gempa.Potensi ?? "N/A", Foreground = Brushes.Orange, Margin = new Thickness(0, 8, 0, 0), TextWrapping = TextWrapping.Wrap });
                
                grid.Children.Add(stackPanel);
                border.Child = grid;

                ContentStackPanel.Children.Add(border);
            }
        }

        private void BtnTerkini_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "terkini";
            refreshTimer.Stop();
            _ = FetchDataAsync(currentMode);
        }


        private void BtnRealtime_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "realtime";
            refreshTimer.Start();
            _ = FetchDataAsync(currentMode);
        }

        public class GempaInfo
        {
            public string? Tanggal { get; set; }
            public string? Jam { get; set; }
            public string? Magnitude { get; set; }
            public string? Kedalaman { get; set; }
            public string? Lintang { get; set; }
            public string? Bujur { get; set; }
            public string? Wilayah { get; set; }
            public string? Potensi { get; set; }
        }

        public class InfoGempaTerkini
        {
            public List<GempaInfo>? gempa { get; set; }
        }

        public class GempaTerkiniRoot
        {
            public InfoGempaTerkini? Infogempa { get; set; }
        }

        public class InfoGempaRealtime
        {
            public GempaInfo? gempa { get; set; }
        }

        public class GempaRealtimeRoot
        {
            public InfoGempaRealtime? Infogempa { get; set; }
        }
    }
}

