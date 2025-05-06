using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    void Start()
    {
        string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            Debug.Log("MySQL 연결 성공!");

            string query = "SELECT * FROM User";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Debug.Log(rdr[0] + " - " + rdr[1]);
            }

            rdr.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("DB 연결 실패: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}
