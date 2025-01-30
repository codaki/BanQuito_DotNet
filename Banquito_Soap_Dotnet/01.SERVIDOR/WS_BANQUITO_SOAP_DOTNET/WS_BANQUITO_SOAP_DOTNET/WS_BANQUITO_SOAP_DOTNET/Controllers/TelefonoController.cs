using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WS_BANQUITO_SOAP_DOTNET.BD;
using WS_BANQUITO_SOAP_DOTNET.Models;

namespace WS_BANQUITO_SOAP_DOTNET.Controllers
{
    public class TelefonoController
    {
        private readonly Conexión conexion = new Conexión();

        // Modelo para los datos de teléfono


        public class TelefonoUpdateRequest
        {
            public int CodTel { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string Foto { get; set; }  // Foto en base64
        }

        public string InsertarTelefono(Telefono telefono)
        {
            try
            {
                int nuevoCodigoTel;

                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    using (SqlCommand cmdVerificar = new SqlCommand(
                        "SELECT COUNT(1) FROM TELEFONO WHERE NOMBRE = @Nombre", connection))
                    {
                        cmdVerificar.Parameters.AddWithValue("@Nombre", telefono.NOMBRE);

                        int existe = (int)cmdVerificar.ExecuteScalar();

                        if (existe > 0)
                        {
                            return "Ya existe un teléfono con el mismo nombre.";
                        }
                    }

                    using (SqlCommand cmdMaxId = new SqlCommand("SELECT ISNULL(MAX(COD_TEL), 0) + 1 FROM TELEFONO", connection))
                    {
                        nuevoCodigoTel = (int)cmdMaxId.ExecuteScalar();
                    }

                    using (SqlCommand cmdInsertar = new SqlCommand(
                        "INSERT INTO TELEFONO (COD_TEL, NOMBRE, PRECIO, FOTO) VALUES (@CodTel, @Nombre, @Precio, @Foto)", connection))
                    {
                        cmdInsertar.Parameters.AddWithValue("@CodTel", nuevoCodigoTel);
                        cmdInsertar.Parameters.AddWithValue("@Nombre", telefono.NOMBRE);
                        cmdInsertar.Parameters.AddWithValue("@Precio", telefono.PRECIO);
                        cmdInsertar.Parameters.AddWithValue("@Foto", telefono.FOTO);

                        var filasAfectadas = cmdInsertar.ExecuteNonQuery();

                        return filasAfectadas > 0 ? "Teléfono insertado exitosamente." : "No se pudo insertar el teléfono en la base de datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al insertar el teléfono: " + ex.Message;
            }
        }

        public List<Telefono> ObtenerTodosTelefonos()
        {
            var telefonos = new List<Telefono>();

            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COD_TEL, NOMBRE, PRECIO, FOTO FROM TELEFONO ORDER BY COD_TEL ASC", connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                telefonos.Add(new Telefono
                                {
                                    COD_TEL = reader["COD_TEL"] != DBNull.Value ? Convert.ToInt32(reader["COD_TEL"]) : 0,
                                    NOMBRE = reader["NOMBRE"] != DBNull.Value ? reader["NOMBRE"].ToString() : string.Empty,
                                    PRECIO = reader["PRECIO"] != DBNull.Value ? Convert.ToDecimal(reader["PRECIO"]) : 0,
                                    FOTO = reader["FOTO"] != DBNull.Value ? reader["FOTO"].ToString() : string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error en la base de datos al obtener los teléfonos: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error general al obtener los teléfonos: " + ex.Message);
            }

            return telefonos;
        }


        public string ActualizarTelefono(TelefonoUpdateRequest request)
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

                        return filasAfectadas > 0 ? "Teléfono actualizado exitosamente." : "No existe un teléfono con ese id.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al actualizar el teléfono: " + ex.Message;
            }
        }

        public string EliminarTelefono(int codTel)
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

                        return filasAfectadas > 0 ? "Teléfono eliminado exitosamente." : "No existe un teléfono con ese id.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al eliminar el teléfono: " + ex.Message;
            }
        }

        public bool ValidarCredenciales(string usuario, string contrasena)
        {
            SqlConnection cn = null;
            try
            {
                cn = conexion.GetConnection();
                cn.Open();

                string sql = "SELECT contrasena FROM CREDENCIALES WHERE usuario = @usuario";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@usuario", usuario);

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return false;
                }

                string storedPassword = reader.GetString(0);
                reader.Close();

                string hashedPassword = GenerateMD5(contrasena);

                return storedPassword == hashedPassword;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                cn?.Close();
            }
        }

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
