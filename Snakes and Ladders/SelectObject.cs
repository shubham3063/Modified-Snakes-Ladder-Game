using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Snakes_and_Ladders
{
    class SelectObject
    {
        string connStr = "Server=localhost;Port=3306;Database=practice;UID=root;password=shutdown";
        void fun()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from persons";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["Firstname"].ToString()[0]);
            }
            conn.Close();
            Console.Read();
        }
    }
}
