using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField idField;
    public TMP_InputField pwField;
    public TMP_Text resultText; // 오류 메시지 출력용 텍스트

    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";

    public void OnClickLogin()
    {
        string userId = idField.text.Trim();
        string password = pwField.text.Trim();

        if (userId == "" || password == "")
        {
            resultText.text = "아이디와 비밀번호를 모두 입력하세요.";
            resultText.color = Color.red;
            return;
        }

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM User WHERE user_id = @id AND pwd = @pw";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@pw", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 로그인 성공
                        UserInfoManager.Instance.userId = userId;
                        SceneManager.LoadScene("AdminPage");
                    }
                    else
                    {
                        resultText.text = "아이디 또는 비밀번호가 올바르지 않습니다.";
                        resultText.color = Color.red;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.LogError("DB 연결 실패: " + ex.Message);
                resultText.text = "서버 연결 실패.";
                resultText.color = Color.red;
            }
        }
    }
}
