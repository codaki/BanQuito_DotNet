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
    /// Lógica de interacción para TablaView.xaml
    /// </summary>
    public partial class TablaView : Window
    {
        private BanquitoServices _controller;

        public TablaView()
        {
            InitializeComponent();
            _controller = new BanquitoServices();
        }

        private async void BuscarCuenta_Click(object sender, RoutedEventArgs e)
        {
            string cedula = Cedula.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor, ingrese la cédula del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Get the client ID using the cedula
                int? idCliente = await _controller.ObtenerCodigoCliente(cedula);

                if (idCliente == null)
                {
                    MessageBox.Show("No se pudo obtener el código del cliente. Verifique la cédula.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the amortization table for the client
                var tablaAmortizacion = await _controller.ObtenerTablaCliente(idCliente.Value);

                if (tablaAmortizacion != null && tablaAmortizacion.Count > 0)
                {
                    // Bind the data to the DataGrid
                    MovimientosDataGrid.ItemsSource = tablaAmortizacion;
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para la tabla de amortización.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
