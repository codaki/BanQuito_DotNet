using System.Net.Http;
using System.Text.Json;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class TablaAmortizacion : ContentPage
{
    private readonly HttpClient _httpClient;

    public TablaAmortizacion()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        BindingContext = this;
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

    private async void OnObtenerTablaClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate cedula input
            if (string.IsNullOrWhiteSpace(EntryCedula.Text))
            {
                await DisplayAlert("Error", "Por favor ingrese un número de cédula", "OK");
                return;
            }

            // First, retrieve the client code
            string clienteUrl = $"{ApiConfig.BaseUrl}cliente/codigo/{EntryCedula.Text}";
            var clienteResponse = await _httpClient.GetAsync(clienteUrl);

            if (clienteResponse.IsSuccessStatusCode)
            {
                var clienteJson = await clienteResponse.Content.ReadAsStringAsync();
                using var clienteDocument = JsonDocument.Parse(clienteJson);
                var root = clienteDocument.RootElement;

                // Check if CodigoCliente is null
                if (root.TryGetProperty("CodigoCliente", out var codigoClienteElement) &&
                    !codigoClienteElement.ValueKind.Equals(JsonValueKind.Null))
                {
                    int codigoCliente = codigoClienteElement.GetInt32();

                    // Now use the client code to fetch the amortization table
                    string tablaUrl = $"{ApiConfig.BaseUrl}credito/obtener-tabla-amortizacion/{codigoCliente}";
                    var tablaResponse = await _httpClient.GetAsync(tablaUrl);

                    if (tablaResponse.IsSuccessStatusCode)
                    {
                        var tablaJson = await tablaResponse.Content.ReadAsStringAsync();
                        var tablaAmortizacion = JsonSerializer.Deserialize<List<AmortizacionDetalle>>(tablaJson);

                        if (tablaAmortizacion != null && tablaAmortizacion.Any())
                        {
                            // Bind to CollectionView
                            AmortizacionCollection.ItemsSource = tablaAmortizacion;
                        }
                        else
                        {
                            await DisplayAlert("Información", "No se encontró tabla de amortización para esta cédula", "OK");
                            AmortizacionCollection.ItemsSource = null;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontró tabla de amortización para esta cédula", "OK");
                        AmortizacionCollection.ItemsSource = null;
                    }
                }
                else
                {
                    // If CodigoCliente is null, show the message from the response
                    string mensaje = root.GetProperty("Mensaje").GetString();
                    await DisplayAlert("Información", mensaje, "OK");
                    AmortizacionCollection.ItemsSource = null;
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener el código de cliente", "OK");
                AmortizacionCollection.ItemsSource = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
            AmortizacionCollection.ItemsSource = null;
        }
    }
}

