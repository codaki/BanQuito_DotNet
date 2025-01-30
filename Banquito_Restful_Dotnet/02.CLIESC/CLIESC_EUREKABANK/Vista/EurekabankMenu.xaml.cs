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
    /// Lógica de interacción para EurekabankMenu.xaml
    /// </summary>
    public partial class EurekabankMenu : Window
    {
        CatalogoView catalogo;
        FacturacionView facturacion;
        VerFacturasView verFacturas;
        MontoMaximoView monto;
        TablaView tabla;
        public EurekabankMenu()
        {
            
            InitializeComponent();
        }

       
        private void CatalogoTelefonos_Click(object sender, RoutedEventArgs e)
        {
            catalogo = new CatalogoView();
            catalogo.Show();
        }

        private void Facturacion_Click(object sender, RoutedEventArgs e)
        {
            facturacion = new FacturacionView();
            facturacion.Show();
        }

        private void MontoMaximo_Click(object sender, RoutedEventArgs e)
        {
            monto = new MontoMaximoView();
            monto.Show();
        }

        private void TablaAmortizacion_Click(object sender, RoutedEventArgs e)
        {
            tabla = new TablaView();
            tabla.Show();
        }

        private void VerFacturacion_Click(object sender, RoutedEventArgs e)
        {
            verFacturas = new VerFacturasView();
            verFacturas.Show();
        }
    }
}
