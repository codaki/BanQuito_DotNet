using CLICON_EUREKABANK.Controlador;
using CLICON_EUREKABANK.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    public class MainMenu
    {
        public async Task mainMenuView()
        {
            LoginController wSLoginUnitConverter = new LoginController();
            UserModel userLogin = new UserModel();
            BanquitoMenuView menuPrincipal = new BanquitoMenuView();

            bool loginSuccessful = false;

            do
            {
                Console.WriteLine("------------ Login Page ------------------");

                Console.Write("Ingrese su usuario: ");
                userLogin.username = Console.ReadLine();

                Console.Write("Ingrese su contraseña: ");
                userLogin.password = ReadPassword();


                loginSuccessful = await wSLoginUnitConverter.validarCredenciales(userLogin);

                if (!loginSuccessful)
                {
                    Console.WriteLine("Usuario o Contrasenia Incorrectos.\n");
                }
                else
                {
                    Console.WriteLine("Bienvenido!");
                    await menuPrincipal.BanquitoMenu();
                }

            } while (!loginSuccessful);
        }


        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b"); // Erase last * character
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*"); // Display * for each character
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
