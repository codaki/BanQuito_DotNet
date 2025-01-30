using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using WS_BANQUITO_RESTFULL_DOTNET.BD;

namespace WS_BANQUITO_RESTFULL_DOTNET.Controllers
{
    public class TelefonoController : ApiController
    {
        private readonly ConexionBD conexion = new ConexionBD();

        // Modelo para los datos de teléfono
        public class Telefono
        {
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string Foto { get; set; }
        }

        public class TelefonoUpdateRequest
        {
            public int CodTel { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string Foto { get; set; }  // Foto en base64
        }


        // Insertar teléfono
        [HttpPost]
        [Route("api/telefono/insertar")]
        public IHttpActionResult InsertarTelefono([FromBody] Telefono telefono)
        {
            try
            {
                int nuevoCodigoTel;

                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    // Verificar si ya existe un teléfono con el mismo nombre
                    using (SqlCommand cmdVerificar = new SqlCommand(
                        "SELECT COUNT(1) FROM TELEFONO WHERE NOMBRE = @Nombre",
                        connection))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Nombre", telefono.Nombre);

                        int existe = (int)cmdVerificar.ExecuteScalar();

                        if (existe > 0)
                        {
                            return BadRequest("Ya existe un teléfono con el mismo nombre.");
                        }
                    }

                    // Obtener el último código de teléfono
                    using (SqlCommand cmdMaxId = new SqlCommand("SELECT ISNULL(MAX(COD_TEL), 0) + 1 FROM TELEFONO", connection))
                    {
                        nuevoCodigoTel = (int)cmdMaxId.ExecuteScalar();
                    }

                    // Insertar la información en la base de datos
                    using (SqlCommand cmdInsertar = new SqlCommand(
                        "INSERT INTO TELEFONO (COD_TEL, NOMBRE, PRECIO, FOTO) VALUES (@CodTel, @Nombre, @Precio, @Foto)",
                        connection))
                    {
                        cmdInsertar.Parameters.AddWithValue("@CodTel", nuevoCodigoTel);
                        cmdInsertar.Parameters.AddWithValue("@Nombre", telefono.Nombre);
                        cmdInsertar.Parameters.AddWithValue("@Precio", telefono.Precio);
                        cmdInsertar.Parameters.AddWithValue("@Foto", telefono.Foto);

                        var filasAfectadas = cmdInsertar.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return Ok("Teléfono insertado exitosamente.");
                        }
                        else
                        {
                            return BadRequest("No se pudo insertar el teléfono en la base de datos.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al insertar el teléfono: " + ex.Message);
            }
        }


        // Obtener todos los teléfonos
        [HttpGet]
        [Route("api/telefono/obtener")]
        public IHttpActionResult ObtenerTodosTelefonos()
        {
            try
            {
                List<object> telefonos = new List<object>();

                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TELEFONO ORDER BY COD_TEL ASC", connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                telefonos.Add(new
                                {
                                    COD_TEL = reader["COD_TEL"],
                                    NOMBRE = reader["NOMBRE"],
                                    PRECIO = reader["PRECIO"],
                                    FOTO = reader["FOTO"]
                                });
                            }
                        }
                    }
                }

                return Ok(telefonos);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener los teléfonos: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("api/telefono/actualizar")]
        public IHttpActionResult ActualizarTelefono([FromBody] TelefonoUpdateRequest request)
        {
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE TELEFONO SET NOMBRE = @Nombre, PRECIO = @Precio, FOTO = @Foto WHERE COD_TEL = @CodTel", connection))
                    {
                        cmd.Parameters.AddWithValue("@CodTel", request.CodTel);
                        cmd.Parameters.AddWithValue("@Nombre", request.Nombre);
                        cmd.Parameters.AddWithValue("@Precio", request.Precio);
                        cmd.Parameters.AddWithValue("@Foto", request.Foto);

                        var filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return Ok("Teléfono actualizado exitosamente.");
                        }
                        else
                        {
                            return BadRequest("No existe un teléfono con ese id.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar el teléfono: " + ex.Message);
            }
        }


        // Eliminar un teléfono
        [HttpDelete]
        [Route("api/telefono/eliminar/{codTel}")]
        public IHttpActionResult EliminarTelefono(int codTel)
        {
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM TELEFONO WHERE COD_TEL = @CodTel", connection))
                    {
                        cmd.Parameters.AddWithValue("@CodTel", codTel);

                        var filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return Ok("Teléfono eliminado exitosamente.");
                        }
                        else
                        {
                            return BadRequest("No existe un telefono con ese id.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar el teléfono: " + ex.Message);
            }
        }

        // Método para validar credenciales
        [HttpPost]
        public bool ValidarCredenciales(string usuario, string contrasena)
        {
            SqlConnection cn = null;
            try
            {
                cn = conexion.GetConnection();
                cn.Open();

                // Consulta para obtener la contraseña almacenada
                string sql = "SELECT contrasena FROM CREDENCIALES WHERE usuario = @usuario";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@usuario", usuario);

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return false;  // Usuario no encontrado
                }

                // Recuperar la contraseña almacenada
                string storedPassword = reader.GetString(0);
                reader.Close();

                // Generar el hash MD5 de la contraseña ingresada
                string hashedPassword = GenerateMD5(contrasena);

                // Comparar el hash generado con el almacenado
                return storedPassword == hashedPassword;  // Retorna true si la contraseña es válida
            }
            catch (Exception)
            {
                return false;  // Error al validar credenciales
            }
            finally
            {
                cn?.Close();
            }
        }


        // Método para generar el hash MD5 de la contraseña
        private string GenerateMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
            }
        }

        
    }
}
