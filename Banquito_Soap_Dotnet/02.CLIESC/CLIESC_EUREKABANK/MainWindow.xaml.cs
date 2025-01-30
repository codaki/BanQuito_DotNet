using CLIESC_EUREKABANK.Controlador;
using CLIESC_EUREKABANK.Modelo;
using CLIESC_EUREKABANK.Vista;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLIESC_EUREKABANK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginController loginController;
        UserModel model;
        EurekabankMenu menu;
        public MainWindow()
        {
            loginController = new LoginController();
            model = new UserModel();
            menu = new EurekabankMenu();
            InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            model.username = userNamerTextBox.Text;
            model.password = passTextBox.Password.ToString();

            var loginSuccesfull = await loginController.validarCredenciales(model);

            if (loginSuccesfull)
            {
                LogLabel.Content = "Ingreso exitoso";
                this.Close();
                menu.Show();
            }

            LogLabel.Content = "usuario o contraseñ invalidos!";

        
        }
    }

    
    
}