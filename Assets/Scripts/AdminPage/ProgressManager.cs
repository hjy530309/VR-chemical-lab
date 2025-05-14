using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MySql.Data.MySqlClient;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefabInstance;
    [SerializeField] private Transform contentTransformInstance;

    static public GameObject blockPrefab;      // 사용자 블록 프리팹
    static public Transform contentTransform;
    static string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";
    
    private void Awake()
    {
        // static에 복사
        blockPrefab = blockPrefabInstance;
        contentTransform = contentTransformInstance;
    }
    public static void ShowProgress()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM room";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int Count = 0;
                while (reader.Read())
            {
                string name = reader["name"].ToString();

                // 블록 생성
                GameObject block = Instantiate(blockPrefab, contentTransform);
                block.name = Count.ToString();
                block.transform.Find("content").GetComponent<TextMeshProUGUI>().text = "testing content 12345678900000";

                Count++;
            }

            }
            catch (MySqlException ex)
            {
                UnityEngine.Debug.LogError("DB 연결 실패: " + ex.Message);
            }
        }
    }
}
