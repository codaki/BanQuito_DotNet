using CLICON_EUREKABANK.Servicios;
using System;
using System.Threading.Tasks;

namespace CLICON_EUREKABANK.Vista
{
    public class BanquitoMenuView
    {
        private readonly BanquitoServices _services;

        public BanquitoMenuView()
        {
            _services = new BanquitoServices();
        }

        public async Task BanquitoMenu()
        {
            bool exit = false;

            // Main menu loop
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("\n--- Menú Venta de telefonos ---");
                Console.WriteLine("1. Catálogo de Teléfonos");
                Console.WriteLine("2. Facturación");
                Console.WriteLine("3. Ver Facturación");
                Console.WriteLine("4. Tabla de Amortización");
                Console.WriteLine("5. Monto Máximo");
                Console.WriteLine("6. Salir");
                Console.Write("\nElige una opción: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await MostrarCatalogoTelefonos();
                        break;

                    case "2":
                        await IniciarFacturacion();
                        break;

                    case "3":
                        await VerFacturacion();
                        break;

                    case "4":
                        await GenerarTablaAmortizacion();
                        break;

                    case "5":
                        await ConsultarMontoMaximo();
                        break;

                    case "6":
                        Console.WriteLine("\nSaliendo del sistema...");
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("\nOpción no válida. Intenta de nuevo.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPresiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private async Task MostrarCatalogoTelefonos()
        {
            CatalogoView catalogoView = new CatalogoView();
            await catalogoView.Run();
        }

        private async Task IniciarFacturacion()
        {
            FacturacionView facturacionView = new FacturacionView();
            await facturacionView.Run();
        }

        private async Task VerFacturacion()
        {
            Console.Clear();
            Console.WriteLine("--- Facturación Registrada ---\n");

            var facturas = await _services.ObtenerFacturas();

            if (facturas.Count > 0)
            {
                foreach (var factura in facturas)
                {
                    Console.WriteLine(new string('-', 50)); // Separator
                    Console.WriteLine($"{"FACTURA N° " + factura.IdFactura}".PadLeft(30));
                    Console.WriteLine(new string('-', 50));

                    Console.WriteLine($"{"Cliente:".PadRight(12)} {factura.NombreCliente}");
                    Console.WriteLine($"{"Cédula:".PadRight(12)} {factura.Cedula}");
                    Console.WriteLine($"{"Dirección:".PadRight(12)} {"Sangolqui"}");
                    Console.WriteLine($"{"Cantidad:".PadRight(12)} {factura.Cantidad}");

                    // Displaying Phones as a List
                    Console.WriteLine("Productos:");
                    List<string> phones = new List<string> { "Samsung", "Infinix", "Oppo", "Samsung" };
                    var selectedPhones = phones.Take(factura.Cantidad).ToList();
                    foreach (var phone in selectedPhones)
                    {
                        Console.WriteLine($"  - {phone}");
                    }

                    Console.WriteLine($"{"Fecha:".PadRight(12)} {factura.Fecha:dd/MM/yyyy}");

                    // Align Forma de Pago to the right
                    string formaPagoText = $"{"Forma de Pago:".PadRight(20)} {factura.FormaPago}";
                    Console.WriteLine(formaPagoText);

                    // Total a Pagar (Green Color)
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n{"Total a Pagar:".PadRight(20)} ${factura.Total:0.00}");
                    Console.ResetColor();

                    Console.WriteLine(new string('-', 50)); // Separator
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No se encontraron facturas.");
            }

            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }


        private async Task GenerarTablaAmortizacion()
        {
            TablaView tablaView = new TablaView();
            await tablaView.Run();
        }

        private async Task ConsultarMontoMaximo()
        {
            Console.Clear();
            Console.WriteLine("--- Consultar Monto Máximo ---");

            Console.Write("Introduce la cédula del cliente: ");
            string cedula = Console.ReadLine();

            var codCliente = await _services.ObtenerCodigoCliente(cedula);
            if (codCliente == null)
            {
                Console.WriteLine("Cliente no encontrado.");
                return;
            }

            var montoMaximo = await _services.ObtenerMontoMaximo(codCliente.Value);
            if (montoMaximo != null)
            {
                Console.WriteLine($"Monto máximo de crédito: ${montoMaximo.MontoMaximoCredito:F2}");
            }
            else
            {
                Console.WriteLine("Error al consultar el monto máximo.");
            }
        }
    }
}
