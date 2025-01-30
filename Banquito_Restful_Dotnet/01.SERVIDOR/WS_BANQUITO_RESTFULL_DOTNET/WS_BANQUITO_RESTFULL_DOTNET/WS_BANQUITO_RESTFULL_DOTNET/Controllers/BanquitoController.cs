using System;
using System.Web.Http;
using System.Data.SqlClient;
using WS_BANQUITO_RESTFULL_DOTNET.BD;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace WS_BANQUITO_RESTFULL_DOTNET.Controllers
{
    public class BanquitoController : ApiController
    {
        private readonly ConexionBD conexion = new ConexionBD();

        [HttpGet]
        [Route("api/credito/verificar/{cedula}")]
        public IHttpActionResult VerificarSujetoCredito(string cedula)
        {
            try
            {
                string motivoRechazo = "";
                SqlConnection cn = null;

                string sqlCliente = @"SELECT 
            c.COD_CLIENTE, 
            c.GENERO, 
            DATEDIFF(YEAR, c.FECHA_NACIMIENTO, GETDATE()) AS EDAD, 
            COUNT(m.COD_MOVIMIENTO) AS DEPOSITOS_RECIENTES 
        FROM 
            CLIENTE c 
        LEFT JOIN 
            CUENTA cu ON c.COD_CLIENTE = cu.COD_CLIENTE 
        LEFT JOIN 
            MOVIMIENTO m ON cu.NUM_CUENTA = m.NUM_CUENTA 
                          AND m.TIPO = 'DEP' 
                          AND m.FECHA >= DATEADD(MONTH, -1, GETDATE()) 
        WHERE 
            c.CEDULA = @Cedula 
        GROUP BY 
            c.COD_CLIENTE, c.GENERO, c.FECHA_NACIMIENTO";

                string sqlCreditoActivo = @"SELECT COUNT(*) AS CREDITOS_ACTIVOS 
                                    FROM CREDITO 
                                    WHERE COD_CLIENTE = @CodCliente AND ESTADO = 'A'";

                cn = conexion.GetConnection();
                cn.Open();

                using (SqlCommand cmdCliente = new SqlCommand(sqlCliente, cn))
                {
                    cmdCliente.Parameters.AddWithValue("@Cedula", cedula);

                    using (SqlDataReader rs = cmdCliente.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            int edad = Convert.ToInt32(rs["EDAD"]);
                            string genero = rs["GENERO"].ToString();
                            int depositosRecientes = Convert.ToInt32(rs["DEPOSITOS_RECIENTES"]);
                            int codCliente = Convert.ToInt32(rs["COD_CLIENTE"]);

                            if (genero == "M" && edad < 25)
                            {
                                motivoRechazo = "El cliente masculino tiene menos de 25 años.";
                            }
                            else if (depositosRecientes == 0)
                            {
                                motivoRechazo = "El cliente no tiene transacciones de depósito en el último mes.";
                            }
                            else
                            {
                                using (SqlCommand cmdCredito = new SqlCommand(sqlCreditoActivo, cn))
                                {
                                    cmdCredito.Parameters.AddWithValue("@CodCliente", codCliente);
                                    int creditosActivos = Convert.ToInt32(cmdCredito.ExecuteScalar());

                                    if (creditosActivos > 0)
                                    {
                                        motivoRechazo = "El cliente ya tiene un crédito activo.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            motivoRechazo = "El cliente no está registrado en el banco.";
                        }
                    }
                }

                return Ok(new
                {
                    EsSujetoDCredito = string.IsNullOrEmpty(motivoRechazo),
                    Mensaje = string.IsNullOrEmpty(motivoRechazo)
                        ? "El cliente es sujeto de crédito."
                        : motivoRechazo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("api/credito/monto-maximo/{codCliente}")]
        public IHttpActionResult CalcularMontoMaximoCredito(int codCliente)
        {
            SqlConnection cn = null;

            try
            {
                double montoMaximo = 0.0;
                string sqlPromedios = @"SELECT 
            (SELECT COALESCE(SUM(m.VALOR), 0) / NULLIF(COUNT(DISTINCT CAST(m.FECHA AS DATE)), 0)
             FROM MOVIMIENTO m 
             JOIN CUENTA cu ON m.NUM_CUENTA = cu.NUM_CUENTA 
             WHERE cu.COD_CLIENTE = @CodCliente AND m.TIPO = 'DEP' 
                   AND m.FECHA >= DATEADD(MONTH, -3, GETDATE())) AS PROMEDIO_DEPOSITOS, 
            (SELECT COALESCE(SUM(m.VALOR), 0) / NULLIF(COUNT(DISTINCT CAST(m.FECHA AS DATE)), 0)
             FROM MOVIMIENTO m 
             JOIN CUENTA cu ON m.NUM_CUENTA = cu.NUM_CUENTA 
             WHERE cu.COD_CLIENTE = @CodCliente AND m.TIPO = 'RET' 
                   AND m.FECHA >= DATEADD(MONTH, -3, GETDATE())) AS PROMEDIO_RETIROS";

                cn = conexion.GetConnection();
                cn.Open();

                double promedioDepositos = 0.0;
                double promedioRetiros = 0.0;

                using (SqlCommand cmd = new SqlCommand(sqlPromedios, cn))
                {
                    cmd.Parameters.AddWithValue("@CodCliente", codCliente);

                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            promedioDepositos = rs["PROMEDIO_DEPOSITOS"] == DBNull.Value
                                ? 0.0
                                : Convert.ToDouble(rs["PROMEDIO_DEPOSITOS"]);

                            promedioRetiros = rs["PROMEDIO_RETIROS"] == DBNull.Value
                                ? 0.0
                                : Convert.ToDouble(rs["PROMEDIO_RETIROS"]);
                        }
                    }
                }

                // Calcular el monto máximo de crédito
                double diferencia = promedioDepositos - promedioRetiros;
                montoMaximo = Math.Round((diferencia * 0.30) * 6, 2); // Redondear a 2 decimales

                // Definir el mensaje según el monto máximo
                string mensaje;
                if (montoMaximo > 0)
                {
                    mensaje = "Monto máximo de crédito calculado exitosamente.";
                }
                else
                {
                    mensaje = "El monto máximo de crédito es cero porque la diferencia entre depósitos y retiros es insuficiente.";
                }

                return Ok(new
                {
                    MontoMaximoCredito = montoMaximo > 0 ? montoMaximo : 0,
                    Mensaje = mensaje
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                // Asegurar que la conexión esté cerrada
                if (cn != null)
                {
                    cn.Close();
                }
            }
        }


        [HttpGet]
        [Route("api/cliente/codigo/{cedula}")]
        public IHttpActionResult ObtenerCodigoCliente(string cedula)
        {
            SqlConnection cn = null;

            try
            {
                string sql = @"SELECT COD_CLIENTE 
                       FROM CLIENTE 
                       WHERE CEDULA = @Cedula";

                cn = conexion.GetConnection();
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int codigoCliente = Convert.ToInt32(result);
                        return Ok(new
                        {
                            CodigoCliente = codigoCliente,
                            Mensaje = "Código del cliente obtenido exitosamente."
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            CodigoCliente = (int?)null,
                            Mensaje = "No se encontró un cliente con la cédula proporcionada."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                }
            }
        }

        [HttpPost]
        [Route("api/credito/crear-tabla-amortizacion")]
        public IHttpActionResult CrearTablaAmortizacion(int codCliente, decimal valorPrestamo, int numCuotas)
        {
            SqlConnection cn = null;
            try
            {
                const decimal tasaAnual = 16.5m; // Tasa fija anual 16.5%
                decimal tasaMensual = tasaAnual / 12 / 100; // Conversión a tasa mensual
                decimal saldo = valorPrestamo;

                decimal valorCuota = valorPrestamo * (tasaMensual * (decimal)Math.Pow((double)(1 + tasaMensual), numCuotas))
                                        / ((decimal)Math.Pow((double)(1 + tasaMensual), numCuotas) - 1);

                int idCredito; // Para almacenar el ID del préstamo insertado en la base de datos
                int inicioCuota; // Para almacenar el número de inicio de la cuota

                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    // Insertar en la tabla CREDITO
                    // Generar un nuevo ID_CREDITO basado en el ID más alto en la base de datos
                    using (SqlCommand cmdMaxId = new SqlCommand("SELECT ISNULL(MAX(ID_CREDITO), 0) + 1 FROM CREDITO", connection))
                    {
                        idCredito = (int)cmdMaxId.ExecuteScalar();
                    }

                    // Obtener el número máximo de cuota existente para este cliente
                    using (SqlCommand cmdMaxCuota = new SqlCommand(
                        "SELECT ISNULL(MAX(N_CUOTA), 0) + 1 FROM CUOTA",
                        connection))
                    {
                        cmdMaxCuota.Parameters.AddWithValue("@CodCliente", codCliente);
                        inicioCuota = (int)cmdMaxCuota.ExecuteScalar();
                    }

                    // Insertar el nuevo préstamo con el ID generado
                    using (SqlCommand cmdInsertCredito = new SqlCommand(
                        "INSERT INTO CREDITO (ID_CREDITO, COD_CLIENTE, ESTADO, CUOTAS, VALOR_PRESTAMO, TASA_ANUAL) VALUES (@IdCredito, @CodCliente, @Estado, @NumCuotas, @ValorPrestamo, @TasaAnual)",
                        connection))
                    {
                        cmdInsertCredito.Parameters.AddWithValue("@IdCredito", idCredito);
                        cmdInsertCredito.Parameters.AddWithValue("@CodCliente", codCliente);
                        cmdInsertCredito.Parameters.AddWithValue("@Estado", "A");
                        cmdInsertCredito.Parameters.AddWithValue("@NumCuotas", numCuotas);
                        cmdInsertCredito.Parameters.AddWithValue("@ValorPrestamo", valorPrestamo);
                        cmdInsertCredito.Parameters.AddWithValue("@TasaAnual", tasaAnual);

                        var rowsAffected = cmdInsertCredito.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return BadRequest("No se pudo insertar el préstamo en la base de datos. Verifique los datos.");
                        }
                    }

                    // Insertar las cuotas en la tabla CUOTA
                    for (int cuota = 1; cuota <= numCuotas; cuota++)
                    {
                        decimal interesPagado = saldo * tasaMensual;
                        decimal capitalPagado = valorCuota - interesPagado;
                        saldo -= capitalPagado;

                        using (SqlCommand cmdInsertCuota = new SqlCommand(
                            "INSERT INTO CUOTA (N_CUOTA, ID_CREDITO, VALOR_CUOTA, INTERES_PAGADO, CAPITAL_PAGADO, SALDO) VALUES (@Ncuota, @IdCredito, @ValorCuota, @InteresPagado, @CapitalPagado, @Saldo)",
                            connection))
                        {
                            cmdInsertCuota.Parameters.AddWithValue("@Ncuota", inicioCuota + cuota - 1);
                            cmdInsertCuota.Parameters.AddWithValue("@IdCredito", idCredito);
                            cmdInsertCuota.Parameters.AddWithValue("@ValorCuota", Math.Round(valorCuota, 2));
                            cmdInsertCuota.Parameters.AddWithValue("@InteresPagado", Math.Round(interesPagado, 2));
                            cmdInsertCuota.Parameters.AddWithValue("@CapitalPagado", Math.Round(capitalPagado, 2));
                            cmdInsertCuota.Parameters.AddWithValue("@Saldo", Math.Round(saldo, 2));

                            cmdInsertCuota.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Tabla de amortización y préstamo almacenados exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error en el cálculo o base de datos: " + ex.Message);
            }
        }


        [HttpGet]
        [Route("api/credito/obtener-tabla-amortizacion/{codCliente}")]
        public IHttpActionResult ObtenerTablaAmortizacion(int codCliente)
        {
            SqlConnection cn = null;
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();
                    // Buscar el último ID_CREDITO activo para el cliente
                    int idCredito;
                    using (SqlCommand cmdMaxCredito = new SqlCommand(
                        "SELECT TOP 1 ID_CREDITO FROM CREDITO WHERE COD_CLIENTE = @CodCliente AND ESTADO = 'A' ORDER BY ID_CREDITO DESC",
                        connection))
                    {
                        cmdMaxCredito.Parameters.AddWithValue("@CodCliente", codCliente);
                        idCredito = (int)cmdMaxCredito.ExecuteScalar();
                        if (idCredito == 0)
                        {
                            return NotFound(); // No se encontró ningún crédito activo para este cliente
                        }
                    }

                    // Obtener las cuotas de la tabla CUOTA para el ID_CREDITO encontrado
                    List<object> tablaAmortizacion = new List<object>();
                    int minimoNumeroCuota;

                    // Primero, obtener el número mínimo de cuota
                    using (SqlCommand cmdMinCuota = new SqlCommand(
                        "SELECT MIN(N_CUOTA) FROM CUOTA WHERE ID_CREDITO = @IdCredito",
                        connection))
                    {
                        cmdMinCuota.Parameters.AddWithValue("@IdCredito", idCredito);
                        minimoNumeroCuota = Convert.ToInt32(cmdMinCuota.ExecuteScalar());
                    }

                    // Luego, obtener y renumerar las cuotas
                    using (SqlCommand cmdCuotas = new SqlCommand(
                        "SELECT N_CUOTA, VALOR_CUOTA, INTERES_PAGADO, CAPITAL_PAGADO, SALDO FROM CUOTA WHERE ID_CREDITO = @IdCredito ORDER BY N_CUOTA",
                        connection))
                    {
                        cmdCuotas.Parameters.AddWithValue("@IdCredito", idCredito);
                        using (SqlDataReader reader = cmdCuotas.ExecuteReader())
                        {
                            int contadorCuota = 1;
                            while (reader.Read())
                            {
                                tablaAmortizacion.Add(new
                                {
                                    NumeroCuota = contadorCuota++,
                                    ValorCuota = reader.GetDecimal(1),
                                    InteresPagado = reader.GetDecimal(2),
                                    CapitalPagado = reader.GetDecimal(3),
                                    Saldo = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                    return Ok(tablaAmortizacion);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener la tabla de amortización: " + ex.Message);
            }
        }

        // Método para crear una factura con la lógica de descuento
        [HttpPost]
        [Route("api/factura/crear")]
        public IHttpActionResult CrearFactura(int codCliente, decimal total, string formaPago, int cantidad)
        {
            SqlConnection cn = null;
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    // Validar forma de pago y aplicar descuento si es en efectivo
                    decimal totalFinal = total;
                    int descuento = 0;

                    if (formaPago == "EF") // Efectivo
                    {
                        descuento = 42; // Descuento del 42%
                        totalFinal = total - (total * (descuento / 100m));
                    }

                    // Obtener el nuevo ID_FACTURA
                    int idFactura;
                    using (SqlCommand cmdMaxFactura = new SqlCommand(
                        "SELECT ISNULL(MAX(ID_FACTURA), 0) + 1 FROM FACTURA", connection))
                    {
                        idFactura = (int)cmdMaxFactura.ExecuteScalar();
                    }

                    // Insertar la factura en la base de datos
                    using (SqlCommand cmdInsertFactura = new SqlCommand(
                        "INSERT INTO FACTURA (ID_FACTURA, COD_CLIENTE, FECHA, TOTAL, FORMA_PAGO, DESCUENTO, CANTIDAD) VALUES (@IdFactura, @CodCliente, @Fecha, @Total, @FormaPago, @Descuento, @Cantidad)",
                        connection))
                    {
                        cmdInsertFactura.Parameters.AddWithValue("@IdFactura", idFactura);
                        cmdInsertFactura.Parameters.AddWithValue("@CodCliente", codCliente);
                        cmdInsertFactura.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        cmdInsertFactura.Parameters.AddWithValue("@Total", Math.Round(totalFinal, 2));
                        cmdInsertFactura.Parameters.AddWithValue("@FormaPago", formaPago);
                        cmdInsertFactura.Parameters.AddWithValue("@Descuento", descuento);
                        cmdInsertFactura.Parameters.AddWithValue("@Cantidad", cantidad);

                        var rowsAffected = cmdInsertFactura.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return BadRequest("No se pudo insertar la factura en la base de datos.");
                        }
                    }

                    return Ok(new { Mensaje = "Factura creada exitosamente", IdFactura = idFactura, Total = Math.Round(totalFinal, 2), Descuento = descuento, Cantidad= cantidad });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear la factura: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("api/factura/obtener-facturas")]
        public IHttpActionResult VerFacturas()
        {
            SqlConnection cn = null;
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    string sqlFacturas = @"
                SELECT 
                    f.ID_FACTURA, 
                    c.NOMBRE, 
                    c.CEDULA, 
                    f.FECHA, 
                    f.TOTAL,
                    CASE 
                        WHEN f.FORMA_PAGO = 'EF' THEN 'Efectivo'
                        WHEN f.FORMA_PAGO = 'CR' THEN 'Crédito'
                        ELSE f.FORMA_PAGO 
                    END AS FORMA_PAGO,
                    f.DESCUENTO,
                    f.CANTIDAD
                FROM 
                    FACTURA f
                JOIN 
                    CLIENTE c ON f.COD_CLIENTE = c.COD_CLIENTE
                ORDER BY 
                    f.ID_FACTURA DESC";

                    List<object> facturas = new List<object>();

                    using (SqlCommand cmd = new SqlCommand(sqlFacturas, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                facturas.Add(new
                                {
                                    IdFactura = reader.GetInt32(0),
                                    NombreCliente = reader.GetString(1),
                                    Cedula = reader.GetString(2),
                                    Fecha = reader.GetDateTime(3),
                                    Total = reader.GetDecimal(4),
                                    FormaPago = reader.GetString(5),
                                    Descuento = reader.GetInt32(6),
                                    Cantidad= reader.GetInt32(7),
                                });
                            }
                        }
                    }

                    return Ok(facturas);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener las facturas: " + ex.Message);
            }
        }






    }
}