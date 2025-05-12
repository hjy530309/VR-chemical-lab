using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;

public class UserListManager : MonoBehaviour
{
    public TMP_InputField searchField;
    public GameObject blockPrefab;      // 사용자 블록 프리팹
    public Transform contentTransform;  // ScrollView의 Content

    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";

    void Start()
    {
        LoadUserList();
    }

    void LoadUserList()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT name, DATE_FORMAT(create_time, '%Y-%m-%d') AS created_date FROM room";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            int Count = 0;

            while (reader.Read())
            {
                string name = reader["name"].ToString();
                string create_time = reader["created_date"].ToString();

                // 블록 생성
                GameObject block = Instantiate(blockPrefab, contentTransform);
                block.name = Count.ToString();
                block.transform.Find("title").GetComponent<TextMeshProUGUI>().text = name;
                block.transform.Find("date").GetComponent<TextMeshProUGUI>().text = create_time;

                Count++;
            }

            // Content 크기 조정 (Vertical Layout Group과 Content Size Fitter가 활성화된 경우 자동으로 조정)
            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            float height = Count * blockPrefab.GetComponent<RectTransform>().sizeDelta.y;  // 블록 높이에 맞게 조정
            contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // Content Size Fitter를 사용하여 동적 크기 변경
            ContentSizeFitter contentSizeFitter = contentTransform.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter != null)
            {
                contentSizeFitter.SetLayoutVertical();
            }
        }
    }
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
                        sql += " WHERE name LIKE '%" + search + "%' OR DATE_FORMAT(create_time, '%Y-%m-%d') LIKE '%" + search + "%'";
                    }
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int Count = 0;
                ClearContent();

                while (reader.Read())
            {
                string name = reader["name"].ToString();
                string create_time = reader["created_date"].ToString();

                // 블록 생성
                GameObject block = Instantiate(blockPrefab, contentTransform);
                block.name = Count.ToString();
                block.transform.Find("title").GetComponent<TextMeshProUGUI>().text = name;
                block.transform.Find("date").GetComponent<TextMeshProUGUI>().text = create_time;

                Count++;
            }

            }
            catch (MySqlException ex)
            {
                Debug.LogError("DB 연결 실패: " + ex.Message);
            }
        }
    }

    public void ClearContent()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
