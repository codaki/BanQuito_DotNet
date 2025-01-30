using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class Facturacion : ContentPage
{
    private List<WSBANQUITO.Telefono> telefonos = new List<WSBANQUITO.Telefono>();
    private List<TelefonoConImagen> telefonosConImagen = new List<TelefonoConImagen>();
    private WSBANQUITO.WSBanquitoSoapClient _soapClient;

    public Facturacion()
    {
        InitializeComponent();
        _soapClient = new WSBANQUITO.WSBanquitoSoapClient(WSBANQUITO.WSBanquitoSoapClient.EndpointConfiguration.WSBanquitoSoap);
        LimpiarCarrito(); // Limpia el carrito al inicializar la página
        MarcaPicker.SelectedIndex = 0; // Seleccionar "TODOS LOS TELÉFONOS" por defecto
        LoadTelefonos();
    }

    private void LimpiarCarrito()
    {
        CarritoService.CarritoItems.Clear();
    }

    private async void LoadTelefonos()
    {
        try
        {
            // Crear la solicitud SOAP
            var request = new WSBANQUITO.ObtenerTodosTelefonosRequest();
            // Llamar al servicio SOAP
            var response = await _soapClient.ObtenerTodosTelefonosAsync();
            // Verifica si la lista tiene elementos antes de procesarla
            if (response.Body.ObtenerTodosTelefonosResult != null && response.Body.ObtenerTodosTelefonosResult.Length > 0)
            {
                // Procesar la respuesta como una lista
                telefonos = response.Body.ObtenerTodosTelefonosResult.ToList();
                // Crear una lista de TelefonoConImagen
                telefonosConImagen = telefonos.Select(t => new TelefonoConImagen(t)).ToList();
                // Aplicar el filtro inicial
                FilterTelefonos(MarcaPicker.SelectedItem?.ToString());
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron obtener los teléfonos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private void OnMarcaSelected(object sender, EventArgs e)
    {
        var selectedMarca = MarcaPicker.SelectedItem?.ToString();
        FilterTelefonos(selectedMarca);
    }

    private void FilterTelefonos(string marca)
    {
        if (string.IsNullOrEmpty(marca) || marca == "TODOS LOS TELÉFONOS")
        {
            CollectionViewTelefonos.ItemsSource = telefonosConImagen;
            return;
        }

        var filteredTelefonos = telefonosConImagen
            .Where(t => t.NOMBRE.ToUpper().Contains(marca.ToUpper()))
            .ToList();

        CollectionViewTelefonos.ItemsSource = filteredTelefonos;
    }

    private async void OnAgregarAlCarritoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var telefono = button?.CommandParameter as TelefonoConImagen;

        if (telefono != null)
        {
            CarritoService.CarritoItems.Add(telefono);
            await DisplayAlert("Éxito", "Producto agregado al carrito", "OK");
        }
    }

    private async void OnVerCarritoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Carrito());
    }
}

public class TelefonoConImagen
{
    public int COD_TEL { get; set; }
    public string NOMBRE { get; set; }
    public decimal PRECIO { get; set; }
    public string FOTO { get; set; }
    public ImageSource FotoImage { get; set; }

    // Constructor que toma un objeto Telefono y asigna la propiedad FotoImage
    public TelefonoConImagen(WSBANQUITO.Telefono telefono)
    {
        COD_TEL = telefono.COD_TEL;
        NOMBRE = telefono.NOMBRE;
        PRECIO = telefono.PRECIO;
        FOTO = telefono.FOTO;
        FotoImage = GetImageSourceFromBase64(telefono.FOTO);
    }

    // Método para convertir Base64 a ImageSource
    private ImageSource GetImageSourceFromBase64(string fotoBase64)
    {
        if (!string.IsNullOrEmpty(fotoBase64))
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(fotoBase64);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch (FormatException)
            {
                return null; // Si la cadena Base64 es inválida, devolver null
            }
        }
        return null; // Si la cadena está vacía o nula, devolver null
    }
}