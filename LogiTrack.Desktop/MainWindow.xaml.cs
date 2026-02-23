using LogiTrack.Application.DTOs;
using LogiTrack.Domain.Constants;
using LogiTrack.Domain.Enums;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LogiTrack.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient();
        private const string ApiUrl = "https://localhost:7219/api/shipment";
        private List<ShipmentResponseDto> _allShipments = new List<ShipmentResponseDto>();

        public MainWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e) => LoadData();

        private async void LoadData()
        {
            try
            {
                var response = await _client.GetStringAsync(ApiUrl);
                _allShipments = JsonConvert.DeserializeObject<List<ShipmentResponseDto>>(response) ?? new List<ShipmentResponseDto>();
                ApplyFilterAndSort();
            }
            catch { }
        }

        private void ApplyFilterAndSort()
        {
            if (ShipmentsList == null) return;

            var search = TxtSearch?.Text?.Trim().ToLower() ?? "";

            var result = _allShipments
                .Where(s => string.IsNullOrEmpty(search) ||
                            s.CityFrom.ToLower().Contains(search) ||
                            s.CityTo.ToLower().Contains(search))
                .OrderByDescending(s => SortCombo?.SelectedIndex == 1 ? -s.Id : s.Id)
                .ToList();

            ShipmentsList.ItemsSource = result;
            UpdateStats();
        }

        private void UpdateStats()
        {
            int p = 0; int t = 0; int d = 0; decimal rev = 0; double w = 0;
            foreach (var s in _allShipments)
            {
                if (s.Status == ShipmentStatus.Pending) p++;
                else if (s.Status == ShipmentStatus.InTransit) t++;
                else if (s.Status == ShipmentStatus.Delivered)
                {
                    d++;
                    rev += s.Price;
                }
                w += s.Weight;
            }
            LblPendingCount.Text = p.ToString();
            LblInTransitCount.Text = t.ToString();
            LblDeliveredCount.Text = d.ToString();
            LblTotalRevenue.Text = rev.ToString("N2") + " €";
            LblTotalWeight.Text = w.ToString("N1") + " kg";
        }

        private void StatusCombo_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is ShipmentResponseDto shipment)
            {
                combo.SelectionChanged -= StatusCombo_SelectionChanged;
                string currentStatus = shipment.Status.ToString();

                foreach (var item in combo.Items.OfType<ComboBoxItem>())
                {
                    if ((item.Content?.ToString() ?? "").Equals(currentStatus, StringComparison.OrdinalIgnoreCase))
                    {
                        combo.SelectedItem = item;
                        break;
                    }
                }

                combo.SelectionChanged += StatusCombo_SelectionChanged;
            }
        }

        private async void StatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.IsLoaded && combo.IsDropDownOpen && combo.DataContext is ShipmentResponseDto shipment)
            {
                var selectedItem = combo.SelectedItem as ComboBoxItem;
                if (selectedItem == null) return;

                string selectedStatusString = selectedItem.Content?.ToString() ?? string.Empty;

                if (Enum.TryParse<ShipmentStatus>(selectedStatusString, out var newStatusEnum))
                {
                    var original = _allShipments.FirstOrDefault(x => x.Id == shipment.Id);
                    if (original != null) original.Status = newStatusEnum;
                    shipment.Status = newStatusEnum;

                    UpdateStats();

                    try
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(new { status = selectedStatusString }), Encoding.UTF8, "application/json");
                        await _client.PutAsync($"{ApiUrl}/{shipment.Id}/status", content);
                    }
                    catch { }
                }
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string from = TxtFrom.Text, to = TxtTo.Text, weightRaw = TxtWeight.Text.Replace(',', '.');

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to)) return;

            string? error = (!from.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)) ||
                            !to.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))) ? ErrorMessages.InvalidCityFormat :
                           (!double.TryParse(weightRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double w)) ? ErrorMessages.InvalidWeightFormat :
                           (w <= 0) ? ErrorMessages.InvalidWeight : null;

            if (error != null)
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double.TryParse(weightRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double weight);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new ShipmentCreateDto { CityFrom = from, CityTo = to, Weight = weight }), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TxtFrom.Clear(); TxtTo.Clear(); TxtWeight.Clear();
                    TxtFrom.Focus();
                    LoadData();
                }
            }
            catch { }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var dialog = new ConfirmWindow();

                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    await _client.DeleteAsync($"{ApiUrl}/{id}");
                    LoadData();
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilterAndSort();
        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilterAndSort();
    }
}