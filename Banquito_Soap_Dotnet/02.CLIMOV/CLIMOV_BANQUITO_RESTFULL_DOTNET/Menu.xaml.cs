namespace CLIMOV_BANQUITO_RESTFULL_DOTNET;

public partial class Menu : ContentPage
{
    public Menu()
    {
        InitializeComponent();
    }

    private async void OnCatalogoTelefonosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatalogoTelefonos());
    }

    private async void OnFacturacionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Facturacion());
    }

    private async void OnMontoMaximoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MontoMaximo());
    }

    private async void OnTablaAmortizacionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TablaAmortizacion());
    }

    private async void OnVerFacturasClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VerFacturas());
    }
}