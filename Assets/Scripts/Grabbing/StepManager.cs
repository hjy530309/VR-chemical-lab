using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class StepManager : MonoBehaviour
{
    public int nowStage;
    string connStr = "Server=127.0.0.1;Database=vrlab_db;User ID=root;Password=0000;Pooling=false;Port=3308;";
    public List<StepData> stepList = new List<StepData>();

    public class StepData
    {
        public string stepId;      // ex: "1-1"
        public string content; // ex: "삼각 플라스크에 과산화수소 (약 100mL)를 붓는다"
    }

    public void CallStageContent()
    {
        
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM ScenarioSteps where stage = " + nowStage;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()){
                    stepList.Add(new StepData { stepId = reader["step_id"].ToString(), content = reader["content"].ToString() });
                }

            }
            catch (MySqlException ex)
            {
                Debug.LogError("DB 연결 실패: " + ex.Message);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CallStageContent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
