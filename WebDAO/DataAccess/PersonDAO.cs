using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebDAO.Interfaces;
using WebDAO.Models;

namespace WebDAO.DataAccess
{
    public class PersonDAO : IDAO<Person>, IDisposable
    {
        private IConnection _connection;

        public PersonDAO(IConnection connection)
        {
            _connection = connection;
        }

        public Person Create(Person model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Person (FirstName, LastName, Age, Email)" +
                    "VALUES (@fn, @ln, @age, @email);" +
                    "SELECT @@Indetity";

                cmd.Parameters.Add("@fn", SqlDbType.Text).Value = model.FirstName;
                cmd.Parameters.Add("@ln", SqlDbType.Text).Value = model.LastName;
                cmd.Parameters.Add("@age", SqlDbType.Int).Value = model.Age;
                cmd.Parameters.Add("@email", SqlDbType.Text).Value = model.Email;

                model.Id = int.Parse(cmd.ExecuteScalar().ToString());
            }

            return model;
        }

        public bool Delete(Person model)
        {
            bool deleted = false;

            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Person WHERE Id = @id";

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;

                if (cmd.ExecuteNonQuery() > 0)
                {
                    deleted = true;
                }
            }

            return deleted;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Person FindById(params object[] keys)
        {
            Person p = null;

            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, FirstName, LastName, Age, Email FROM PERSON" +
                    "WHERE Id = @id";

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = keys[0];
                //cmd.Parameters.Add("@id2", SqlDbType.Int).Value = keys[1];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        p = new Person();
                        reader.Read();
                        p.Id = reader.GetInt32(0);
                        p.FirstName = reader.GetString(1);
                        p.LastName = reader.GetString(2);
                        p.Age = reader.GetInt32(3);
                        p.Email = reader.GetString(4);
                    }
                }
            }

            return p;
        }

        public Collection<Person> GetAll()
        {
            Collection<Person> people = new Collection<Person>();

            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, FirstName, LastName, Age, Email FROM PERSON" +
                    "ORDER BY FirstName";

                using (SqlDataAdapter adpt = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adpt.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        Person p = new Person
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Age = int.Parse(row["Age"].ToString()),
                            Email = row["Email"].ToString(),
                        };

                        people.Add(p);
                    }
                }
            }

            return people;
        }

        public void Update(Person model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Person SET FirstName=@fn, LastName=@ln, Age=@age, Email=@email" +
                    "WHERE Id=@id";

                cmd.Parameters.Add("@fn", SqlDbType.Text).Value = model.FirstName;
                cmd.Parameters.Add("@ln", SqlDbType.Text).Value = model.LastName;
                cmd.Parameters.Add("@age", SqlDbType.Int).Value = model.Age;
                cmd.Parameters.Add("@email", SqlDbType.Text).Value = model.Email;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
