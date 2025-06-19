using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoCtrl : MonoBehaviour
{
    public Dropdown deviceDropdown;
    public Dropdown fileDropdown;
    private Model _model;
    private int _nowSelectedDeviceIndex = 0;
    public Mesh[] deviceMeshs;
    public MeshFilter deviceMeshFilter;
    
    public TMP_Text quaternion0_Text;
    public TMP_Text quaternion1_Text;
    public TMP_Text quaternion2_Text;
    public TMP_Text quaternion3_Text;
    public TMP_Text lineNumber_Text;
    
    
    
    private void Start()
    {
        _model= GetComponent<Model>();
        deviceDropdown.onValueChanged.AddListener(ChangeDevice);
        fileDropdown.onValueChanged.AddListener(ChangeFile);
    }

    public void ChangeFile(int value)
    {
        var data = _model.GetFileDataList(value);
    }
    
    public void ChangeDevice(int value)
    {
        _nowSelectedDeviceIndex = value;
        deviceMeshFilter.mesh = deviceMeshs[_nowSelectedDeviceIndex];
    }

    private IEnumerator LoopShow(DataEntry model)
    {
        lineNumber_Text.text = model.Timestamp;
        
        yield return new WaitForSeconds(10);
        
    }
    
}