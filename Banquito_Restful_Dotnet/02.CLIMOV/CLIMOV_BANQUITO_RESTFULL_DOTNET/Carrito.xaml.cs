namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;
using CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class Carrito : ContentPage
{
    public Carrito()
    {
        InitializeComponent();
        CarritoCollectionView.ItemsSource = CarritoService.CarritoItems;
        CalcularTotal();

        // Suscribirse a cambios en la colección
        CarritoService.CarritoItems.CollectionChanged += (s, e) => CalcularTotal();
    }

    void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        Console.WriteLine($"ScrollX: {e.ScrollX}, ScrollY: {e.ScrollY}");
    }

    private void CalcularTotal()
    {
        decimal total = CarritoService.Total;
        LabelTotal.Text = $"Total: ${total:F2}";
    }

    private void OnEliminarDelCarritoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var telefono = button?.CommandParameter as Telefono;

        if (telefono != null)
        {
            CarritoService.CarritoItems.Remove(telefono);
        }
    }

    private async void OnPagarConEfectivoClicked(object sender, EventArgs e)
    {
        var total = LabelTotal.Text.Replace("Total: $", "");
        int cantidadProductos = CarritoService.CarritoItems.Count;
        await Navigation.PushAsync(new PagarEfectivo(total,cantidadProductos));
    }

    private async void OnPagarConCreditoClicked(object sender, EventArgs e)
    {
        var total = LabelTotal.Text.Replace("Total: $", "");
        int cantidadProductos = CarritoService.CarritoItems.Count;
        await Navigation.PushAsync(new PagarCredito(total, cantidadProductos));
    }

}