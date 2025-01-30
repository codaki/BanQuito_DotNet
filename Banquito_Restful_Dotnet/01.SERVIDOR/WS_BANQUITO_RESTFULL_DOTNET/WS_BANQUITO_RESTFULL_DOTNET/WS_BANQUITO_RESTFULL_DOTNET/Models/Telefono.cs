using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_RESTFULL_DOTNET.Models
{
    public class Telefono
    {
        // Propiedades principales
        public int Id { get; set; } // Identificador único del teléfono
        public string Nombre { get; set; } // Número telefónico
        public decimal Precio { get; set; } // Tipo de teléfono (ej. Móvil, Casa, Trabajo)

        // Métodos opcionales
        public override string ToString()
        {
            return $"Id: {Id}, Nombre: {Nombre}, Precio: {Precio}";
        }
    }
}