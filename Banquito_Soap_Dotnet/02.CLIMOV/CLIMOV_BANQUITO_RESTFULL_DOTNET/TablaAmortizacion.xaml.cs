using System.Net.Http;
using System.Text.Json;
using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class TablaAmortizacion : ContentPage
{
    private WSBANQUITO.WSBanquitoSoap _soapClient;

    public TablaAmortizacion()
    {
        InitializeComponent();
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        BindingContext = this;
    }

    // Existing orientation code remains the same
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

            // First, retrieve the client code using SOAP service
            var requestCodigoClienteBody = new WSBANQUITO.ObtenerCodigoClienteRequestBody
            {
                cedula = EntryCedula.Text
            };

            var requestCodigoCliente = new WSBANQUITO.ObtenerCodigoClienteRequest
            {
                Body = requestCodigoClienteBody
            };

            var responseCodigoCliente = await _soapClient.ObtenerCodigoClienteAsync(requestCodigoCliente);

            if (responseCodigoCliente == null ||
                responseCodigoCliente.Body == null ||
                responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el código de cliente", "OK");
                return;
            }

            int codigoCliente = (int)responseCodigoCliente.Body.ObtenerCodigoClienteResult.CodigoCliente;

            // Create request body for getting the amortization table
            var requestObtenerTablaBody = new WSBANQUITO.ObtenerTablaAmortizacionRequestBody
            {
                codCliente = codigoCliente
            };

            // Create request for getting the amortization table
            var requestObtenerTabla = new WSBANQUITO.ObtenerTablaAmortizacionRequest
            {
                Body = requestObtenerTablaBody
            };

            // Call SOAP service to get amortization table
            var responseObtenerTabla = await _soapClient.ObtenerTablaAmortizacionAsync(requestObtenerTabla);

            if (responseObtenerTabla == null ||
                responseObtenerTabla.Body == null ||
                responseObtenerTabla.Body.ObtenerTablaAmortizacionResult == null)
            {
                await DisplayAlert("Error", "No se pudo obtener la tabla de amortización", "OK");
                AmortizacionCollection.ItemsSource = null;
                return;
            }

            var tablaAmortizacion = responseObtenerTabla.Body.ObtenerTablaAmortizacionResult;

            // Bind to CollectionView
            if (tablaAmortizacion != null && tablaAmortizacion.Length > 0)
            {
                AmortizacionCollection.ItemsSource = tablaAmortizacion;
            }
            else
            {
                await DisplayAlert("Información", "No se encontró tabla de amortización para esta cédula", "OK");
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