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
    private WSBANQUITO.WSBanquitoSoap _soapClient;


    public GenerarTabla(int codCliente, string total,int cantidad)
    {
        InitializeComponent();
        _cantidad = cantidad;
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);

        BindingContext = this;

        _httpClient = new HttpClient();
        _codCliente = codCliente;

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

            // Create request body for amortization table
            var requestCrearTablaBody = new WSBANQUITO.CrearTablaAmortizacionRequestBody
            {
                codCliente = _codCliente,
                valorPrestamo = Convert.ToDecimal(_totalPrestamo),
                numCuotas = numeroCuotas
            };

            // Create request for creating amortization table
            var requestCrearTabla = new WSBANQUITO.CrearTablaAmortizacionRequest
            {
                Body = requestCrearTablaBody
            };

            // Call SOAP service to create amortization table
            var responseCrearTabla = await _soapClient.CrearTablaAmortizacionAsync(requestCrearTabla);

            if (responseCrearTabla == null || responseCrearTabla.Body == null)
            {
                await DisplayAlert("Error", "No se pudo generar la tabla de amortización", "OK");
                return;
            }

            // Create request body for generating the invoice
            var requestCrearFacturaBody = new WSBANQUITO.CrearFacturaRequestBody
            {
                formaPago = "CR",
                codCliente = _codCliente,
                total = Convert.ToDecimal(_totalPrestamo),
                cantidad = _cantidad

            };

            // Create request for creating invoice
            var requestCrearFactura = new WSBANQUITO.CrearFacturaRequest
            {
                Body = requestCrearFacturaBody
            };

            // Call SOAP service to create invoice
            var responseCrearFactura = await _soapClient.CrearFacturaAsync(requestCrearFactura);

            if (responseCrearFactura == null || responseCrearFactura.Body == null || responseCrearFactura.Body.CrearFacturaResult.Mensaje != "Factura creada exitosamente")
            {
                await DisplayAlert("Error", "No se pudo generar la factura", "OK");
                return;
            }

            // Create request body for getting the amortization table
            var requestObtenerTablaBody = new WSBANQUITO.ObtenerTablaAmortizacionRequestBody
            {
                codCliente = _codCliente
            };

            // Create request for getting the amortization table
            var requestObtenerTabla = new WSBANQUITO.ObtenerTablaAmortizacionRequest
            {
                Body = requestObtenerTablaBody
            };

            // Call SOAP service to get amortization table
            var responseObtenerTabla = await _soapClient.ObtenerTablaAmortizacionAsync(requestObtenerTabla);

            if (responseObtenerTabla == null || responseObtenerTabla.Body == null || responseObtenerTabla.Body.ObtenerTablaAmortizacionResult == null)
            {
                await DisplayAlert("Error", "No se pudo obtener la tabla de amortización", "OK");
                return;
            }

            var tablaAmortizacion = responseObtenerTabla.Body.ObtenerTablaAmortizacionResult;

            // Bind to CollectionView
            AmortizacionCollection.ItemsSource = tablaAmortizacion;

            // Optional: Show success message
            await DisplayAlert("Éxito", "Tabla de amortización generada", "OK");
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