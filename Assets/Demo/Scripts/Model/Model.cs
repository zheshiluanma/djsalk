using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonModel 
{
    public string Timestamp { get; set; }

    public string DeviceName { get; set; }

    public float AccelerationX_g { get; set; }

    public float AccelerationY_g { get; set; }

    public float AccelerationZ_g { get; set; }

    public float AngularVelocityX_deg_per_s { get; set; }

    public float AngularVelocityY_deg_per_s { get; set; }

    public float AngularVelocityZ_deg_per_s { get; set; }

    public float AngleX_deg { get; set; }

    public float AngleY_deg { get; set; }

    public float AngleZ_deg { get; set; }

    public float MagneticFieldX_uT { get; set; }

    public float MagneticFieldY_uT { get; set; }

    public float MagneticFieldZ_uT { get; set; }

    public float Quaternion0 { get; set; }

    public float Quaternion1 { get; set; }

    public float Quaternion2 { get; set; }

    public float Quaternion3 { get; set; }

    public float Temperature_C { get; set; }

    public string Version { get; set; }

    public int BatteryLevel_percent { get; set; }
}



public class Model : MonoBehaviour
{
    public TextAsset[] jsonFiles; // 在编辑器中拖动初始化
    private List<JsonModel[]> jsonDataList = new List<JsonModel[]>();
    
    public void LoadData()
    {
        // 读取JSON文件内容
        foreach (var jsonFile in jsonFiles)
        {
            if (jsonFile != null)
            {
                var jsonContent = jsonFile.text;
                // 解析JSON内容为JsonModel数组
                var jsonData = JsonUtility.FromJson<JsonModel[]>(jsonContent);
                jsonDataList.Add(jsonData);
            }
        }
    }
}