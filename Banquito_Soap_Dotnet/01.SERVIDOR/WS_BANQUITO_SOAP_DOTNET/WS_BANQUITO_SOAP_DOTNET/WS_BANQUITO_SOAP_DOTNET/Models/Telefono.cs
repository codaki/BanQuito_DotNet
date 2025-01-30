using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_SOAP_DOTNET.Models
{
    public class Telefono
    {
        // Propiedades principales
        public int COD_TEL { get; set; } // Identificador único del teléfono
        public string NOMBRE { get; set; } // Número telefónico
        public decimal PRECIO { get; set; } // Tipo de teléfono (ej. Móvil, Casa, Trabajo)

        public String FOTO { get; set; }

        // Métodos opcionales
        public override string ToString()
        {
            return $"Id: {COD_TEL}, Nombre: {NOMBRE}, Precio: {PRECIO}";
        }
    }
}