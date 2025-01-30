using CLIESC_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CLIESC_EUREKABANK.Vista
{
    /// <summary>
    /// Lógica de interacción para PagarEfectivoView.xaml
    /// </summary>
    public partial class PagarEfectivoView : Window
    {
        private BanquitoServices _controller;
        private decimal _total;
        decimal totaldescuento;
        int _cantidad;

        public PagarEfectivoView(decimal total)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            _total = total;
            // Set the total amount on the label
            TotalLabel.Content = $"Total: {_total:C}";
            totaldescuento = _total - (_total*0.42m);
            totalDsc.Content = $"Total con descuento:{totaldescuento:C}";
        }

        public PagarEfectivoView(decimal total,int cantidad)
        {
            InitializeComponent();
            _controller = new BanquitoServices();
            _total = total;
            _cantidad = cantidad;
            // Set the total amount on the label
            TotalLabel.Content = $"Total: {_total:C}";
            totaldescuento = _total - (_total * 0.42m);
            totalDsc.Content = $"Total con descuento:{totaldescuento:C}";
        }

        private async void pagarEfectivo_Click(object sender, RoutedEventArgs e)
        {
            string cedula = CedulaTextBox.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor, ingrese la cédula del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Fetch the codCliente using the cedula
                var responseCodigo = await _controller.ObtenerCodigoCliente(cedula);
                int? codCliente = responseCodigo.CodigoCliente;

                if (codCliente == null)
                {
                    MessageBox.Show("No se pudo obtener el código del cliente. Verifique la cédula.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create the factura
                var responseFactura = await _controller.CrearFacturaService(codCliente.Value, _total, "EF",_cantidad);
                string success = responseFactura.Mensaje;

                if (success == "Factura creada exitosamente")
                {
                    MessageBox.Show("Factura creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Error al crear la factura. Intente de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
