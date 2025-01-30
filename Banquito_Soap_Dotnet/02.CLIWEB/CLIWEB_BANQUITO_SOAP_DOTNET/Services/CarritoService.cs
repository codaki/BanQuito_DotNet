using CLIWEB_BANQUITO_SOAP_DOTNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLIWEB_BANQUITO_SOAP_DOTNET.Services
{
    public class CarritoService
    {
        private static List<TelefonoConImagen> _carritoItems = new List<TelefonoConImagen>();

        public static IEnumerable<TelefonoConImagen> CarritoItems => _carritoItems;

        public static decimal Total => _carritoItems.Sum(t => t.PRECIO);

        public static void AgregarAlCarrito(TelefonoConImagen item)
        {
            _carritoItems.Add(item);
        }

        public static void EliminarDelCarrito(TelefonoConImagen item)
        {
            _carritoItems.Remove(item);
        }

        public static void VaciarCarrito()
        {
            _carritoItems.Clear();
        }
    }
}