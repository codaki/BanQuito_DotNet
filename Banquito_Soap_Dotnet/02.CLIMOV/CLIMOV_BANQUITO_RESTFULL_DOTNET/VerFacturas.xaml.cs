using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class VerFacturas : ContentPage
{
    private WSBANQUITO.WSBanquitoSoapClient _soapClient;
    public ObservableCollection<Factura> Facturas { get; set; }

    public VerFacturas()
    {
        InitializeComponent();
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        LoadFacturas();
    }

    private List<string> ObtenerProductosPorCantidad(int cantidad)
    {
        var productos = new List<string>();

        switch (cantidad)
        {
            case 1:
                productos.Add("Samsung");
                break;
            case 2:
                productos.Add("Samsung");
                productos.Add("Infinix");
                break;
            case 3:
                productos.Add("Samsung");
                productos.Add("Infinix");
                productos.Add("Oppo");
                break;
            case 4:
                productos.Add("Samsung");
                productos.Add("Infinix");
                productos.Add("Oppo");
                productos.Add("Samsung");
                break;
            case 5:
                productos.Add("Samsung");
                productos.Add("Infinix");
                productos.Add("Oppo");
                productos.Add("Samsung");
                productos.Add("Infinix");
                break;
            default:
                productos.Add("Samsung");
                break;
        }

        return productos;
    }

    private async void LoadFacturas()
    {
        try
        {
            // Create request for obtaining invoices
            var requestObtenerFacturas = new WSBANQUITO.ObtenerFacturasRequest();

            // Call SOAP service to get invoices
            var responseObtenerFacturas = await _soapClient.ObtenerFacturasAsync();

            if (responseObtenerFacturas?.Body?.ObtenerFacturasResult == null)
            {
                await DisplayAlert("Error", "No se pudieron cargar las facturas", "OK");
                return;
            }

            // Convert SOAP service Factura to local Factura
            Facturas = new ObservableCollection<Factura>(
                responseObtenerFacturas.Body.ObtenerFacturasResult.Select(soapFactura => new Factura
                {
                    IdFactura = soapFactura.IdFactura,
                    NombreCliente = soapFactura.NombreCliente,
                    Cedula = soapFactura.Cedula,
                    Fecha = soapFactura.Fecha,
                    Total = soapFactura.Total,
                    FormaPago = soapFactura.FormaPago,
                    Descuento = soapFactura.Descuento,
                    Cantidad = soapFactura.Cantidad,
                    Productos = ObtenerProductosPorCantidad(soapFactura.Cantidad) // Agregamos los productos según la cantidad
                })
            );

            BindingContext = this;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las facturas: {ex.Message}", "OK");
        }
    }
}

public class Factura
{
    public int IdFactura { get; set; }
    public string NombreCliente { get; set; }
    public string Cedula { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string FormaPago { get; set; }
    public int Descuento { get; set; }
    public int Cantidad { get; set; }
    public List<string> Productos { get; set; } // Nueva propiedad para la lista de productos
}