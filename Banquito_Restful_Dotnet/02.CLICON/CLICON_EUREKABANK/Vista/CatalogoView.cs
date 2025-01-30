using CLICON_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLICON_EUREKABANK.Vista
{
    public class CatalogoView
    {
        private readonly BanquitoServices _controller;

        public CatalogoView()
        {
            _controller = new BanquitoServices();
        }

        public async Task Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Catálogo de Teléfonos ===");
                Console.WriteLine("1. Agregar Teléfono");
                Console.WriteLine("2. Eliminar Teléfono");
                Console.WriteLine("3. Actualizar Teléfono");
                Console.WriteLine("4. Ver Catálogo de Teléfonos");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione una opción: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AgregarTelefono();
                        break;

                    case "2":
                        await EliminarTelefono();
                        break;

                    case "3":
                        await ActualizarTelefono();
                        break;

                    case "4":
                        await VerCatalogoTelefonos();
                        break;

                    case "5":
                        exit = true;
                        Console.WriteLine("Saliendo del catálogo...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private async Task AgregarTelefono()
        {
            Console.Clear();
            Console.WriteLine("=== Agregar Teléfono ===");
            Console.Write("Ingrese el nombre del teléfono: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese el precio del teléfono: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal precio))
            {
                Console.WriteLine("Seleccione una imagen para el teléfono:");
                string foto = SelectImageFile();

                if (!string.IsNullOrEmpty(foto))
                {
                    var result = await _controller.AgregarTelefonoService(nombre, precio, foto);

                    if (result)
                    {
                        Console.WriteLine("Teléfono agregado exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("Error al agregar el teléfono.");
                    }
                }
                else
                {
                    Console.WriteLine("No se seleccionó ninguna imagen. No se puede agregar el teléfono.");
                }
            }
            else
            {
                Console.WriteLine("Precio no válido.");
            }
        }

        private async Task EliminarTelefono()
        {
            Console.Clear();
            Console.WriteLine("=== Eliminar Teléfono ===");
            Console.Write("Ingrese el ID del teléfono a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var result = await _controller.EliminarTelefonoService(id);

                if (result)
                {
                    Console.WriteLine("Teléfono eliminado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Error al eliminar el teléfono.");
                }
            }
            else
            {
                Console.WriteLine("ID no válido.");
            }
        }

        private async Task ActualizarTelefono()
        {
            Console.Clear();
            Console.WriteLine("=== Actualizar Teléfono ===");
            Console.Write("Ingrese el ID del teléfono: ");
            if (int.TryParse(Console.ReadLine(), out int codTel))
            {
                Console.Write("Ingrese el nuevo nombre del teléfono: ");
                string nombre = Console.ReadLine();

                Console.Write("Ingrese el nuevo precio del teléfono: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal precio))
                {
                    Console.WriteLine("Seleccione una imagen para el teléfono:");
                    string foto = SelectImageFile();

                    if (!string.IsNullOrEmpty(foto))
                    {
                        var result = await _controller.ActualizarTelefonoService(codTel, nombre, precio, foto);

                        if (result)
                        {
                            Console.WriteLine("Teléfono actualizado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error al actualizar el teléfono.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se seleccionó ninguna imagen. No se puede actualizar el teléfono.");
                    }
                }
                else
                {
                    Console.WriteLine("Precio no válido.");
                }
            }
            else
            {
                Console.WriteLine("ID no válido.");
            }
        }

        private async Task VerCatalogoTelefonos()
        {
            Console.Clear();
            Console.WriteLine("=== Catálogo de Teléfonos ===");

            var telefonos = await _controller.ObtenerTelefonosService();

            if (telefonos != null && telefonos.Count > 0)
            {
                Console.WriteLine("\nID\tNombre\t\tPrecio");
                Console.WriteLine("---------------------------------");

                foreach (var telefono in telefonos)
                {
                    Console.WriteLine($"{telefono.COD_TEL}\t{telefono.NOMBRE}\t\t${telefono.PRECIO:F2}");
                }
            }
            else
            {
                Console.WriteLine("No se encontraron teléfonos.");
            }
        }


        private string SelectImageFile()
        {
            string result = null;

            Thread dialogThread = new Thread(() =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "Seleccionar imagen del teléfono";
                    openFileDialog.Filter = "Archivos de imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                    openFileDialog.Multiselect = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                            result = Convert.ToBase64String(imageBytes);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al leer el archivo de imagen: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se seleccionó ninguna imagen.");
                    }
                }
            });

            dialogThread.SetApartmentState(ApartmentState.STA);
            dialogThread.Start();
            dialogThread.Join();

            return result;
        }
    }
}
