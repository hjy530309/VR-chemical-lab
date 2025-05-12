using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using TMPro;

public class SearchManager : MonoBehaviour
{
    public TMP_InputField searchField;
    public GameObject blockPrefab;      // 사용자 블록 프리팹
    public Transform contentTransform;  // ScrollView의 Content

    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";

    public void SearchDB()
    {
        string search = searchField.text.Trim();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT name, DATE_FORMAT(create_time, '%Y-%m-%d') AS created_date FROM room";
                if (search != "")
                    {
                        sql += " WHERE name LIKE '%" + search + "%' OR created_date LIKE '%" + search + "%'";
                    }
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()){
                    Debug.Log("방 이름: " + reader["name"]);
                }

            }
            catch (MySqlException ex)
            {
                Debug.LogError("DB 연결 실패: " + ex.Message);
            }
        }
    }
}
