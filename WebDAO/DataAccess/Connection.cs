using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebDAO.Interfaces;

namespace WebDAO.DataAccess
{
    public class Connection : IConnection, IDisposable
    {
        private SqlConnection _connection;

        public Connection()
        {
            _connection = new SqlConnection("Data Source = (nome da db local);" +
                "Initial Catalog = (nome da bd);" +
                "Integrated Security = SSPI");
        }

        public void Close()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        public SqlConnection Fetch()
        {
            return this.Open();
        }

        public SqlConnection Open()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }
    }
}
