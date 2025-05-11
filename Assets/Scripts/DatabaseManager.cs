using System;
using MySql.Data.MySqlClient;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DatabaseManager : MonoBehaviour
{

    public bool isIdChecked = false;
    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";

    // 회원가입 버튼 클릭 시 호출할 메서드
    public void RegisterUser(string userId, string name, string email, string password)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO User (user_id, name, email, pwd) VALUES (@id, @name, @email, @pwd)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pwd", password);

                int result = cmd.ExecuteNonQuery();
                Debug.Log(result > 0 ? "회원가입 성공!" : "회원가입 실패!");
            }
            catch (Exception ex)
            {
                Debug.LogError("DB 오류: " + ex.Message);
            }
        }
    }

    public TMP_InputField inputUser_id;
    public TMP_Text duplicateCheckResultText;

    // 🔍 ID 중복확인
    public void CheckDuplicateId()
    {
        string userId = inputUser_id.text.Trim();

        if (string.IsNullOrEmpty(userId))
        {
            duplicateCheckResultText.text = "아이디를 입력하세요.";
            duplicateCheckResultText.color = Color.red;
            isIdChecked = false;
            return;
        }

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM User WHERE user_id = @userId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    duplicateCheckResultText.text = "이미 사용 중인 아이디입니다.";
                    duplicateCheckResultText.color = Color.red;
                    isIdChecked = false;
                }
                else
                {
                    duplicateCheckResultText.text = "사용 가능한 아이디입니다.";
                    duplicateCheckResultText.color = Color.green;
                    isIdChecked = true;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("중복확인 오류: " + ex.Message);
            }
        }
    }

}
