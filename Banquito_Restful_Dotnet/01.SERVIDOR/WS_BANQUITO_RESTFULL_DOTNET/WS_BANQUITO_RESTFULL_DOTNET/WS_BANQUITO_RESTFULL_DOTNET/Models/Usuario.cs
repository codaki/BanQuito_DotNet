using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_RESTFULL_DOTNET.Models
{
    public class Usuario

    {
        // Propiedades principales
        public int Id { get; set; } 
        public string NUsuario { get; set; } 
        public decimal Contraseña { get; set; }

        // Métodos opcionales
        public override string ToString()
        {
            return $"Id: {Id}, Usuario: {NUsuario}, Contrasena: {Contraseña}";
        }
    }
}