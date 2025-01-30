using System;
using System.Collections.Generic;
using System.Web.Services;
using WS_BANQUITO_SOAP_DOTNET.BD;
using WS_BANQUITO_SOAP_DOTNET.Controllers;
using WS_BANQUITO_SOAP_DOTNET.Models;

namespace WS_BANQUITO_SOAP_DOTNET.Servicio
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WSBanquito : System.Web.Services.WebService
    {
        private readonly TelefonoController telefonoLogic = new TelefonoController();
        private readonly BanquitoController banquitoLogic = new BanquitoController();

        [WebMethod]
        public string InsertarTelefono(string nombre, decimal precio, string foto)
        {
            var telefono = new Telefono
            {
                NOMBRE = nombre,
                PRECIO = precio,
                FOTO = foto
            };

            return telefonoLogic.InsertarTelefono(telefono);
        }

        [WebMethod]
        public List<Telefono> ObtenerTodosTelefonos()
        {
            return telefonoLogic.ObtenerTodosTelefonos();
        }

        [WebMethod]
        public string ActualizarTelefono(int codTel, string nombre, decimal precio, string foto)
        {
            var request = new TelefonoController.TelefonoUpdateRequest
            {
                CodTel = codTel,
                Nombre = nombre,
                Precio = precio,
                Foto = foto
            };

            return telefonoLogic.ActualizarTelefono(request);
        }

        [WebMethod]
        public string EliminarTelefono(int codTel)
        {
            return telefonoLogic.EliminarTelefono(codTel);
        }

        [WebMethod]
        public bool ValidarCredenciales(string usuario, string contrasena)
        {
            return telefonoLogic.ValidarCredenciales(usuario, contrasena);
        }

        [WebMethod]
        public VerificarSujetoCreditoResult VerificarSujetoCredito(string cedula)
        {
            var result = banquitoLogic.VerificarSujetoCredito(cedula);
            return new VerificarSujetoCreditoResult
            {
                EsSujetoCredito = result.EsSujetoCredito,
                Mensaje = result.Mensaje
            };
        }

        [WebMethod]
        public CalcularMontoMaximoCreditoResult CalcularMontoMaximoCredito(int codCliente)
        {
            var result = banquitoLogic.CalcularMontoMaximoCredito(codCliente);
            return new CalcularMontoMaximoCreditoResult
            {
                MontoMaximoCredito = result.MontoMaximoCredito,
                Mensaje = result.Mensaje
            };
        }

        [WebMethod]
        public ObtenerCodigoClienteResult ObtenerCodigoCliente(string cedula)
        {
            var result = banquitoLogic.ObtenerCodigoCliente(cedula);
            return new ObtenerCodigoClienteResult
            {
                CodigoCliente = result.CodigoCliente,
                Mensaje = result.Mensaje
            };
        }

        [WebMethod]
        public string CrearTablaAmortizacion(int codCliente, decimal valorPrestamo, int numCuotas)
        {
            return banquitoLogic.CrearTablaAmortizacion(codCliente, valorPrestamo, numCuotas);
        }

        [WebMethod]
        public List<Amortizacion> ObtenerTablaAmortizacion(int codCliente)
        {
            return banquitoLogic.ObtenerTablaAmortizacion(codCliente);
        }

        [WebMethod]
        public CrearFacturaResult CrearFactura(int codCliente, decimal total, string formaPago, int cantidad)
        {
            var result = banquitoLogic.CrearFactura(codCliente, total, formaPago, cantidad);
            return new CrearFacturaResult
            {
                Mensaje = result.Mensaje,
                IdFactura = result.IdFactura,
                TotalFinal = result.TotalFinal,
                Descuento = result.Descuento
            };
        }


        [WebMethod]
        public List<Factura> ObtenerFacturas()
        {
            return banquitoLogic.ObtenerFacturas();
        }
    }


}
