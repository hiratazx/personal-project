using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace GempaBMKG
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string URL_GEMPA_REALTIME = "https://data.bmkg.go.id/DataMKG/TEWS/autogempa.json";
        private const string URL_GEMPA_TERKINI = "https://data.bmkg.go.id/DataMKG/TEWS/gempaterkini.json";
        
        private DispatcherTimer refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRealtimeQuakeData();

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(60);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }
        
        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (ContentTitle.Text == "Gempa Realtime")
            {
                await LoadRealtimeQuakeData();
            }
        }

        private async Task<string> FetchDataAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                return $"Error: Gagal menyambung ke server BMKG. Pesan: {e.Message}";
            }
        }

        private void UpdateContent(string title, string content)
        {
            ContentTitle.Text = title;
            ContentTextBox.Text = content;
        }

        private void UpdateActiveButton(Button activeButton)
        {
            BtnRealtime.ClearValue(Button.BackgroundProperty);
            BtnTerkini.ClearValue(Button.BackgroundProperty);

            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005A9E"));
        }

        private async void BtnRealtime_Click(object sender, RoutedEventArgs e)
        {
            await LoadRealtimeQuakeData();
        }

        private async void BtnTerkini_Click(object sender, RoutedEventArgs e)
        {
           await LoadLatestQuakesData();
        }
        
        private async Task LoadRealtimeQuakeData()
        {
            UpdateActiveButton(BtnRealtime);
            UpdateContent("Gempa Realtime", "Sedang memuat data terbaru...");
            string jsonString = await FetchDataAsync(URL_GEMPA_REALTIME);

            if (jsonString.StartsWith("Error:"))
            {
                UpdateContent("Gempa Realtime", jsonString);
                return;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<InfoGempaContainer>(jsonString);
                var gempa = data.Infogempa.gempa;

                var sb = new StringBuilder();
                sb.AppendLine("--- GEMPA BUMI TERBARU ---");
                sb.AppendLine();
                sb.AppendLine($"Waktu         : {gempa.Jam}, {gempa.Tanggal}");
                sb.AppendLine($"Magnitudo     : {gempa.Magnitude} SR");
                sb.AppendLine($"Kedalaman     : {gempa.Kedalaman}");
                sb.AppendLine($"Koordinat     : {gempa.Lintang}, {gempa.Bujur}");
                sb.AppendLine();
                sb.AppendLine($"Lokasi        : {gempa.Wilayah}");
                sb.AppendLine($"Potensi       : {gempa.Potensi}");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Sumber data: BMKG INATEWS");
                UpdateContent("Gempa Realtime", sb.ToString());
            }
            catch (Exception ex)
            {
                UpdateContent("Gempa Realtime", $"Gagal mem-parsing data. {ex.Message}");
            }
        }

        private async Task LoadLatestQuakesData()
        {
            UpdateActiveButton(BtnTerkini);
            UpdateContent("Daftar Gempa Terkini (M > 5.0)", "Sedang memuat data...");
            string jsonString = await FetchDataAsync(URL_GEMPA_TERKINI);

            if (jsonString.StartsWith("Error:"))
            {
                UpdateContent("Daftar Gempa Terkini (M > 5.0)", jsonString);
                return;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<InfoGempaTerkiniContainer>(jsonString);
                var sb = new StringBuilder();
                sb.AppendLine("--- 15 GEMPA BUMI DIRASAKAN TERKINI ---");
                sb.AppendLine();

                int count = 1;
                foreach (var gempa in data.Infogempa.gempa)
                {
                    sb.AppendLine($"{count++}. Tanggal: {gempa.Tanggal}, Jam: {gempa.Jam}");
                    sb.AppendLine($"   Magnitudo: {gempa.Magnitude} SR, Kedalaman: {gempa.Kedalaman}");
                    sb.AppendLine($"   Lokasi: {gempa.Wilayah}");
                    sb.AppendLine($"   Potensi: {gempa.Potensi}");
                    sb.AppendLine("--------------------------------------------------");
                }
                sb.AppendLine("Sumber data: BMKG INATEWS");
                UpdateContent("Daftar Gempa Terkini (M > 5.0)", sb.ToString());
            }
            catch (Exception ex)
            {
                UpdateContent("Daftar Gempa Terkini (M > 5.0)", $"Gagal mem-parsing data. {ex.Message}");
            }
        }
    }

    public class GempaData
    {
        public string Tanggal { get; set; }
        public string Jam { get; set; }
        public string Magnitude { get; set; }
        public string Kedalaman { get; set; }
        public string Lintang { get; set; }
        public string Bujur { get; set; }
        public string Wilayah { get; set; }
        public string Potensi { get; set; }
    }

    public class InfoGempa
    {
        public GempaData gempa { get; set; }
    }

    public class InfoGempaContainer
    {
        public InfoGempa Infogempa { get; set; }
    }
    
    public class InfoGempaTerkini
    {
        public GempaData[] gempa { get; set; }
    }

    public class InfoGempaTerkiniContainer
    {
        public InfoGempaTerkini Infogempa { get; set; }
    }
}

