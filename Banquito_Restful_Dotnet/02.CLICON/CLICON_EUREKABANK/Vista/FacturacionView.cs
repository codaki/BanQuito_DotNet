using CLICON_EUREKABANK.Modelo;
using CLICON_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CLICON_EUREKABANK.Vista
{
    internal class FacturacionView
    {
        private readonly BanquitoServices _controller;
        private List<(Telefono telefono, int cantidad)> _carrito;
        private List<Telefono> _telefonos;
        int _cantidad;

        public FacturacionView()
        {
            _controller = new BanquitoServices();
            _carrito = new List<(Telefono telefono, int cantidad)>();
            _telefonos = new List<Telefono>();
        }

        public async Task Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Facturación ===");

                await LoadTelefonos();

                if (_telefonos.Count == 0)
                {
                    Console.WriteLine("No hay teléfonos disponibles para facturación.");
                    return;
                }

                Console.WriteLine("\nSeleccione una opción:");
                Console.WriteLine("1. Agregar teléfono al carrito");
                Console.WriteLine("2. Ver carrito");
                Console.WriteLine("3. Regresar al menú principal");
                Console.Write("\nIngrese su opción: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AgregarTelefonoAlCarrito();
                        break;

                    case "2":
                        await VerCarrito();
                        break;

                    case "3":
                        exit = true;
                        Console.WriteLine("Regresando al menú principal...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private async Task LoadTelefonos()
        {
            try
            {
                _telefonos = await _controller.ObtenerTelefonosService();

                if (_telefonos.Count == 0)
                {
                    Console.WriteLine("No se encontraron teléfonos.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los teléfonos: {ex.Message}");
            }
        }

        private async Task AgregarTelefonoAlCarrito()
        {
            Console.Clear();
            Console.WriteLine("=== Agregar Teléfono al Carrito ===");

            Console.WriteLine("\nSeleccione un teléfono:");
            for (int i = 0; i < _telefonos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_telefonos[i].NOMBRE} - Precio: {_telefonos[i].PRECIO:C}");
            }

            Console.Write("\nIngrese el número del teléfono: ");
            if (!int.TryParse(Console.ReadLine(), out int telefonoIndex) || telefonoIndex < 1 || telefonoIndex > _telefonos.Count)
            {
                Console.WriteLine("Selección no válida.");
                return;
            }

            Telefono selectedPhone = _telefonos[telefonoIndex - 1];

            Console.Write("Ingrese la cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
            {
                Console.WriteLine("Cantidad no válida.");
                return;
            }

            _carrito.Add((selectedPhone, cantidad));
            Console.WriteLine($"{cantidad} unidad(es) de {selectedPhone.NOMBRE} se ha(n) agregado al carrito.");
        }

        private async Task VerCarrito()
        {
            Console.Clear();
            Console.WriteLine("=== Carrito de Compras ===");

            if (_carrito.Count == 0)
            {
                Console.WriteLine("El carrito está vacío.");
                return;
            }

            decimal total = 0;
            _cantidad = 0;
            Console.WriteLine("\nTeléfonos en el carrito:");
            Console.WriteLine("Nombre\t\tCantidad\tPrecio Total");
            Console.WriteLine("---------------------------------------------");

            foreach (var item in _carrito)
            {
                decimal subtotal = item.telefono.PRECIO * item.cantidad;
                total += subtotal;
                _cantidad++;

                Console.WriteLine($"{item.telefono.NOMBRE}\t{item.cantidad}\t\t{subtotal:C}");
            }

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"Total: {total:C}");

            Console.WriteLine("\nSeleccione una opción:");
            Console.WriteLine("1. Proceder al pago");
            Console.WriteLine("2. Regresar al menú principal");

            Console.Write("\nIngrese su opción: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ProcesarPago(total);
                    break;

                case "2":
                    Console.WriteLine("Regresando al menú principal...");
                    break;

                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }

        private async Task ProcesarPago(decimal total)
        {
            Console.Clear();
            Console.WriteLine("=== Procesar Pago ===");
            Console.WriteLine($"Total a pagar: {total:C}");

            Console.WriteLine("\nSeleccione el método de pago:");
            Console.WriteLine("1. Pagar con efectivo");
            Console.WriteLine("2. Pagar con crédito");

            Console.Write("\nIngrese su elección: ");
            string paymentChoice = Console.ReadLine();

            switch (paymentChoice)
            {
                case "1":
                    await PagarEfectivo(total);
                    break;

                case "2":
                    await PagarCredito(total);
                    break;

                default:
                    Console.WriteLine("Método de pago no válido.");
                    break;
            }
        }

        private async Task PagarEfectivo(decimal total)
        {
            var pagarEfectivoView = new PagarEfectivoView(total, _cantidad);
            await pagarEfectivoView.Run();
            FinalizarCompra();
        }

        private async Task PagarCredito(decimal total)
        {
            var pagarCreditoView = new PagarCreditoView(total, _cantidad);
            await pagarCreditoView.Run();
            FinalizarCompra();
        }

        private void FinalizarCompra()
        {
            Console.WriteLine("\nCompra realizada con éxito. Gracias por su compra.");
            _carrito.Clear();
        }
    }
}
