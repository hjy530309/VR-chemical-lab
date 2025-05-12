using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class LoadInfoManager : MonoBehaviour
{

    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";


    void LoadRoomInfo()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT name, DATE_FORMAT(create_time, '%Y-%m-%d') AS created_date FROM room";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["name"].ToString();
                string create_time = reader["created_date"].ToString();
            }
        }
    }
}
