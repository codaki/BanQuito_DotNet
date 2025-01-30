using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

// Factura.cs
public class Factura
{
    public int IdFactura { get; set; }
    public string NombreCliente { get; set; }
    public string Cedula { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string FormaPago { get; set; }
    public int Descuento { get; set; }
    public int Cantidad { get; set; }
    public List<string> Productos { get; set; }

    public string ProductosString
    {
        get
        {
            return string.Join(", ", Productos);
        }
    }
}

// VerFacturas.xaml.cs
public partial class VerFacturas : ContentPage
{
    public ObservableCollection<Factura> Facturas { get; set; }
    private List<Factura> _allFacturas;

    public VerFacturas()
    {
        InitializeComponent();
        Facturas = new ObservableCollection<Factura>();
        LoadFacturas();
        BuscarCedulaEntry.TextChanged += OnBuscarCedulaTextChanged;
    }

    private async void LoadFacturas()
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"{ApiConfig.BaseUrl}factura/obtener-facturas");
                _allFacturas = JsonSerializer.Deserialize<List<Factura>>(response);

                // Asignar productos según la cantidad
                foreach (var factura in _allFacturas)
                {
                    factura.Productos = GetProductosPorCantidad(factura.Cantidad);
                }

                Facturas.Clear();
                foreach (var factura in _allFacturas)
                {
                    Facturas.Add(factura);
                }
                BindingContext = this;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las facturas: {ex.Message}", "OK");
        }
    }

    private List<string> GetProductosPorCantidad(int cantidad)
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
                productos.Add("Cantidad no especificada");
                break;
        }

        return productos;
    }

    private void OnBuscarCedulaTextChanged(object sender, TextChangedEventArgs e)
    {
        string textoBusqueda = e.NewTextValue?.Trim() ?? string.Empty;

        Facturas.Clear();
        foreach (var factura in _allFacturas)
        {
            if (factura.Cedula.Contains(textoBusqueda))
            {
                Facturas.Add(factura);
            }
        }
    }
}