using System.Net.Http;
using System.Text.Json;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class GenerarTabla : ContentPage
{
    private readonly HttpClient _httpClient;
    private int _codCliente, _cantidad;
    private decimal _totalPrestamo;

    public GenerarTabla(int codCliente, string total, int cantidad)
    {
        InitializeComponent();
        BindingContext = this;

        _httpClient = new HttpClient();
        _codCliente = codCliente;
        _cantidad = cantidad;

        // Parse total as decimal, handling potential formatting issues
        if (decimal.TryParse(total, out decimal parsedTotal))
        {
            _totalPrestamo = parsedTotal;
        }
        else
        {
            // Handle parsing error, perhaps set a default or show an error
            _totalPrestamo = 0;
        }

        // Wire up the Generate button
        BtnGenerar.Clicked += OnGenerarClicked;
    }

    // Existing orientation code
    DeviceOrientationService service;
    protected override void OnAppearing()
    {
        service = new DeviceOrientationService();
        service.SeOrientation(DisplayOrientation.Landscape);
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        service.SeOrientation(DisplayOrientation.Portrait);
    }

    void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        Console.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
    }

    private async void OnGenerarClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate number of quotas
            if (!int.TryParse(EntryCuotas.Text, out int numeroCuotas))
            {
                await DisplayAlert("Error", "Por favor ingrese un número válido de cuotas", "OK");
                return;
            }

            // Check quota range
            if (numeroCuotas < 3 || numeroCuotas > 18)
            {
                await DisplayAlert("Error", "El número de cuotas debe estar entre 3 y 18", "OK");
                return;
            }

            // First, create amortization table
            string createTableUrl = $"{ApiConfig.BaseUrl}credito/crear-tabla-amortizacion?codCliente={_codCliente}&valorPrestamo={_totalPrestamo}&numCuotas={numeroCuotas}";
            string generarFactura = $"{ApiConfig.BaseUrl}factura/crear?formaPago=CR&codCliente={_codCliente}&total={_totalPrestamo}&cantidad={_cantidad}";

            var createResponse = await _httpClient.PostAsync(createTableUrl, null);

            var createResponseFactura = await _httpClient.PostAsync(generarFactura, null);

            /* if (!createResponse.IsSuccessStatusCode)
             {
                 await DisplayAlert("Error", "No se pudo crear la tabla de amortización", "OK");
                 return;
             }*/

            // Get the created amortization table
            string getTableUrl = $"{ApiConfig.BaseUrl}credito/obtener-tabla-amortizacion/{_codCliente}";

            var getResponse = await _httpClient.GetAsync(getTableUrl);

            if (getResponse.IsSuccessStatusCode)
            {
                var json = await getResponse.Content.ReadAsStringAsync();
                var tablaAmortizacion = JsonSerializer.Deserialize<List<AmortizacionDetalle>>(json);

                // Bind to CollectionView
                AmortizacionCollection.ItemsSource = tablaAmortizacion;

                // Optional: Show success message
                await DisplayAlert("Éxito", "Tabla de amortización generada", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la tabla de amortización", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
        }
    }
}

// Model for amortization table (matches the API response)
public class AmortizacionDetalle
{
    public int NumeroCuota { get; set; }
    public decimal ValorCuota { get; set; }
    public decimal InteresPagado { get; set; }
    public decimal CapitalPagado { get; set; }
    public decimal Saldo { get; set; }
}