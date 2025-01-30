using CLIMOV_BANQUITO_RESTFULL_DOTNET.Model;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class Facturacion : ContentPage
{
    private List<Telefono> telefonos = new List<Telefono>();
    private List<Telefono> telefonosFiltrados = new List<Telefono>();

    public Facturacion()
    {
        InitializeComponent();
        LimpiarCarrito(); // Limpia el carrito al inicializar la p�gina
        LoadTelefonos();
        // Establecer "Todos los tel�fonos" como opci�n predeterminada
        MarcasPicker.SelectedIndex = 0;
    }

    private void LimpiarCarrito()
    {
        CarritoService.CarritoItems.Clear();
    }

    private async void LoadTelefonos()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{ApiConfig.BaseUrl}telefono/obtener");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // Deserializar la lista de tel�fonos
                telefonos = JsonSerializer.Deserialize<List<Telefono>>(json);
                // Asignar la lista inicial a telefonosFiltrados
                telefonosFiltrados = new List<Telefono>(telefonos);
                // Asignar la lista a la colecci�n para la vista
                CollectionViewTelefonos.ItemsSource = telefonosFiltrados;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar los tel�fonos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
        }
    }

    private void OnMarcaSelected(object sender, EventArgs e)
    {
        var marcaSeleccionada = MarcasPicker.SelectedItem as string;

        if (string.IsNullOrEmpty(marcaSeleccionada) || marcaSeleccionada == "Todos los tel�fonos")
        {
            telefonosFiltrados = new List<Telefono>(telefonos);
        }
        else
        {
            telefonosFiltrados = telefonos
                .Where(t => t.NOMBRE.ToUpper().Contains(marcaSeleccionada))
                .ToList();
        }

        CollectionViewTelefonos.ItemsSource = telefonosFiltrados;
    }

    private async void OnAgregarAlCarritoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var telefono = button?.CommandParameter as Telefono;

        if (telefono != null)
        {
            CarritoService.CarritoItems.Add(telefono);
            await DisplayAlert("�xito", "Producto agregado al carrito", "OK");
        }
    }

    private async void OnVerCarritoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Carrito());
    }
}