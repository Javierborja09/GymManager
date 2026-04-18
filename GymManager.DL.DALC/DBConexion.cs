using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.DL.DALC
{
    public class DBConexion
    {
        private static string cadena = "Server=localhost;Database=GymManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        public static SqlConnection obtenerConexion()
        {
            SqlConnection conn = new SqlConnection(cadena);
            conn.Open();
            return conn;
        }

    }
}
