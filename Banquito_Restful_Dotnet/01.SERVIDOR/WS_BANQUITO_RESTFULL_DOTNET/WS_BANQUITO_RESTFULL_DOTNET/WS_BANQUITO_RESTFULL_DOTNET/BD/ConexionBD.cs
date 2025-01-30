﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WS_BANQUITO_RESTFULL_DOTNET.BD
{
    public class ConexionBD
    {
        public SqlConnection GetConnection()
        {
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                //sqlConnection.ConnectionString = "data source=LENOVOLAPTOP;initial catalog=BANQUITO;Integrated Security=True;encrypt=False;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";
                //sqlConnection.ConnectionString = "data source=LENOVOLAPTOP,1433;initial catalog=BANQUITO;User Id=sqlserver;Password=090712;encrypt=False;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";
                sqlConnection.ConnectionString = "data source=LENOVOLAPTOP;initial catalog=BANQUITO;Integrated Security=True;encrypt=False;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";
                sqlConnection.Open();
                Console.WriteLine("Conexión establecida exitosamente.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                    Console.WriteLine("La conexión ha sido cerrada.");
                }
            }
            return sqlConnection;
        }
    }
}