using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebDAO.Interfaces
{
    public interface IConnection
    {
        SqlConnection Open();
        SqlConnection Fetch();
        void Close();
    }
}
