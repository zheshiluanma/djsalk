using System;
using System.Collections.Generic;
using UnityEngine;

public class DeviceData
{
    public double Quaternion0 { get; set; }

    public double Quaternion1 { get; set; }

    public double Quaternion2 { get; set; }

    public double Quaternion3 { get; set; }

    public bool Interpolated { get; set; }

    // 以下字段在某些设备条目中才会出现
    public string OriginalTimestamp { get; set; }

    public double? TimeDiffMs { get; set; }
}

public class DataEntry
{
    public string Timestamp { get; set; }

    public string Datetime { get; set; }

    public Dictionary<string, DeviceData> Devices { get; set; }
}




public class Model : MonoBehaviour
{
    public TextAsset[] jsonFiles; // 在编辑器中拖动初始化
    private List<DataEntry[]> jsonDataList = new List<DataEntry[]>();
    
    public static List<string> DeviceNames = new List<string>()
    {
        "WT901BLE67(E9:8A:1A:DD:D5:28)",
        "WT901BLE67(C1:AA:08:9C:D1:14)",
        "WTSDCL(FE:D5:86:66:1D:7C)",
    };
    
    public void LoadData()
    {
        // 读取JSON文件内容
        foreach (var jsonFile in jsonFiles)
        {
            if (jsonFile != null)
            {
                var jsonContent = jsonFile.text;
                // 解析JSON内容为JsonModel数组
                var jsonData = JsonUtility.FromJson<DataEntry[]>(jsonContent);
                jsonDataList.Add(jsonData);
            }
        }
    }
    
    public DataEntry[] GetFileDataList(int index)
    {
        return jsonDataList[index];
    }
}