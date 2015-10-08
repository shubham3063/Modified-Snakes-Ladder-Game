using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Snakes_and_Ladders
{
    class Selector
    {
        string connStr = "Server=localhost;Port=3306;Database=snakesladder;UID=root;password=shutdown";
        public MySqlConnection conn;
        public MySqlDataReader fun(string commandText)
        {
            conn = new MySqlConnection(connStr);
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            MySqlDataReader reader = cmd.ExecuteReader();
            
            return reader;

        }
            
    }
}

