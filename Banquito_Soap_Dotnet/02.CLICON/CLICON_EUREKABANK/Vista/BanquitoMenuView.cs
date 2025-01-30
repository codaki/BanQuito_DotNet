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
            Console.WriteLine("--- Facturación Registrada ---");

            var facturas = await _services.ObtenerFacturasService();

            if (facturas.Count > 0)
            {
                // Encabezados de la tabla
                Console.WriteLine($"{"ID",5} {"Cliente",-20} {"Cantidad",10} {"Descuento",10} {"Total",12} {"Fecha",-20} {"Pago",-10}");
                Console.WriteLine(new string('-', 80));

                // Datos de las facturas
                foreach (var factura in facturas)
                {
                    Console.WriteLine($"{factura.IdFactura,5} {factura.NombreCliente,-20} {factura.Cantidad,10:F2} {factura.Descuento,10:F2}% {factura.Total,12:C} {factura.Fecha,-20} {factura.FormaPago,-10}");
                }
            }
            else
            {
                Console.WriteLine("No se encontraron facturas.");
            }
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
            if (cedula == null)
            {
                return;
            }

            var responseCliente = await _services.ObtenerCodigoCliente(cedula);
            int? codCliente = responseCliente.CodigoCliente;


            if (codCliente == null)
            {
                Console.WriteLine("Cliente no encontrado.");
                return;
            }

            var montoMaximo = await _services.CalcularMontoMaximoCreditoService(codCliente.Value);
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
