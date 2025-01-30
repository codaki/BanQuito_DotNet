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
    /// Lógica de interacción para MontoMaximoView.xaml
    /// </summary>
    public partial class MontoMaximoView : Window
    {
        private BanquitoServices _controller;
        public MontoMaximoView()
        {
            InitializeComponent();
            _controller = new BanquitoServices();
        }

        private async void verMontoMaximo_Click(object sender, RoutedEventArgs e)
        {
            string cedula = txtCedula.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                MessageBox.Show("Por favor, ingrese el número de cédula del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // Get the maximum credit amount using the client ID
                var montoMaximoResponse = await _controller.ObtenerMontoMaximo(idCliente.Value);

                if (montoMaximoResponse != null)
                {
                    MessageBox.Show($"{montoMaximoResponse.Mensaje}\nMonto Máximo Crédito: {montoMaximoResponse.MontoMaximoCredito:C}", "Monto Máximo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No se pudo obtener el monto máximo del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }


}
