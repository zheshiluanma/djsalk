using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;

[Serializable]
public class DeviceEntry {
    public string deviceId;
    public Transform target;
}

[Serializable]
public class Data {
    public float quaternion_0;
    public float quaternion_1;
    public float quaternion_2;
    public float quaternion_3;
    public bool interpolated;
}

[Serializable]
public class DataPoint {
    public double timestamp;
    public Dictionary<string, Data> devices;
}

/// <summary>
/// 支持自动播放并可通过滑动条手动打断的设备轨迹播放器
/// </summary>
public class DeviceTrajectoryPlayer : MonoBehaviour {
    [Header("JSON 文件名 (放在 Resources 下，不含后缀)")]
    public string jsonFileName = "rotationData";

    [Header("设备 ID 与 Transform 的对应关系")]
    public List<DeviceEntry> deviceEntries = new List<DeviceEntry>();

    [Header("UI 滑动条 (Slider)")]
    public Slider timeSlider;

    private List<DataPoint> _dataPoints;
    private Dictionary<string, Transform> _deviceMap;
    private double _startTime, _endTime, _duration;
    private double _playbackTime;
    private bool _isDragging = false;

    void Awake() {
        // 构建设备映射
        _deviceMap = deviceEntries.ToDictionary(e => e.deviceId, e => e.target);

        // 加载并解析 JSON
        TextAsset ta = Resources.Load<TextAsset>(jsonFileName);
        if (ta == null) {
            Debug.LogError($"[TrajectoryPlayer] 找不到 Resources/{jsonFileName}.json");
            return;
        }

        JArray array = JArray.Parse(ta.text);
        _dataPoints = new List<DataPoint>(array.Count);
        foreach (JObject frame in array) {
            var dp = new DataPoint {
                timestamp = frame.Value<double>("timestamp"),
                devices   = new Dictionary<string, Data>()
            };
            foreach (var prop in ((JObject)frame["devices"]).Properties()) {
                var d = prop.Value;
                dp.devices[prop.Name] = new Data {
                    quaternion_0 = d.Value<float>("quaternion_0"),
                    quaternion_1 = d.Value<float>("quaternion_1"),
                    quaternion_2 = d.Value<float>("quaternion_2"),
                    quaternion_3 = d.Value<float>("quaternion_3"),
                    interpolated = d.Value<bool>("interpolated")
                };
            }
            _dataPoints.Add(dp);
        }

        if (_dataPoints.Count < 2) {
            Debug.LogWarning("数据点不足，无法播放轨迹。");
            return;
        }

        // 记录起始和结束时间，以及时长
        _startTime    = _dataPoints.First().timestamp;
        _endTime      = _dataPoints.Last().timestamp;
        _duration     = _endTime - _startTime;
        _playbackTime = _startTime;

        // 初始化滑动条并监听 Pointer 事件
        if (timeSlider != null) {
            timeSlider.minValue = 0f;
            timeSlider.maxValue = 1f;
            timeSlider.value    = 0f;
            timeSlider.onValueChanged.AddListener(OnSliderChanged);

            var trigger = timeSlider.gameObject.GetComponent<EventTrigger>()
                          ?? timeSlider.gameObject.AddComponent<EventTrigger>();

            // 鼠标按下/触摸开始暂停
            var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entryDown.callback.AddListener((data) => { _isDragging = true; });
            trigger.triggers.Add(entryDown);

            // 鼠标松开/触摸结束恢复
            var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entryUp.callback.AddListener((data) => {
                _isDragging = false;
                // 同步播放时间到滑动条位置
                _playbackTime = Mathf.Lerp((float)_startTime, (float)_endTime, timeSlider.value);
                // 立刻应用并保持滑块
                SeekToTime(_playbackTime);
                if (timeSlider != null)
                    timeSlider.value = (float)((_playbackTime - _startTime) / _duration);
            });
            trigger.triggers.Add(entryUp);
        }

        // 初始定位并启动自动播放
        SeekToTime(_startTime);
        StartCoroutine(AutoPlay());
    }

    private void OnSliderChanged(float normalized) {
        if (_isDragging) {
            double t = Mathf.Lerp((float)_startTime, (float)_endTime, normalized);
            SeekToTime(t);
        }
    }

    private IEnumerator AutoPlay() {
        while (_playbackTime <= _endTime) {
            if (!_isDragging) {
                SeekToTime(_playbackTime);
                if (timeSlider != null)
                    timeSlider.value = (float)((_playbackTime - _startTime) / _duration);
                _playbackTime += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void SeekToTime(double t) {
        if (t <= _startTime) {
            ApplyFrame(0);
            return;
        }
        if (t >= _endTime) {
            ApplyFrame(_dataPoints.Count - 1);
            return;
        }

        int i1 = _dataPoints.FindIndex(dp => dp.timestamp >= t);
        int i0 = i1 - 1;
        var dp0  = _dataPoints[i0];
        var dp1  = _dataPoints[i1];
        float alpha = (float)((t - dp0.timestamp) / (dp1.timestamp - dp0.timestamp));

        foreach (var kv in _deviceMap) {
            if (!dp0.devices.TryGetValue(kv.Key, out var d0) || !dp1.devices.TryGetValue(kv.Key, out var d1))
                continue;
            var q0 = Quaternion.Normalize(new Quaternion(d0.quaternion_0, d0.quaternion_1, d0.quaternion_2, d0.quaternion_3));
            var q1 = Quaternion.Normalize(new Quaternion(d1.quaternion_0, d1.quaternion_1, d1.quaternion_2, d1.quaternion_3));
            kv.Value.rotation = Quaternion.Slerp(q0, q1, alpha);
        }
    }

    private void ApplyFrame(int index) {
        var dp = _dataPoints[index];
        foreach (var kv in _deviceMap) {
            if (!dp.devices.TryGetValue(kv.Key, out var d)) continue;
            var q = Quaternion.Normalize(new Quaternion(d.quaternion_0, d.quaternion_1, d.quaternion_2, d.quaternion_3));
            kv.Value.rotation = q;
        }
    }
}
