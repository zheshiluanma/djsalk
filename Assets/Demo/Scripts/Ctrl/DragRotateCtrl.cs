using UnityEngine;

[RequireComponent(typeof(Collider))]  // 可选：确保物体有碰撞器以接收事件
public class DragRotate : MonoBehaviour
{
    [Header("旋转速度")]
    [Tooltip("拖拽时的旋转灵敏度")]
    public float rotationSpeed = 0.2f;

    // 上一帧的输入位置
    private Vector2 _lastPos;

    void Update()
    {
        HandleMouseDrag();
        HandleTouchDrag();
    }

    /// <summary>
    /// PC 端：按住右键拖拽
    /// </summary>
    private void HandleMouseDrag()
    {
        // 右键按下时记录初始位置
        if (Input.GetMouseButtonDown(1))
        {
            _lastPos = Input.mousePosition;
        }

        // 右键持续按住时，计算偏移并旋转
        if (Input.GetMouseButton(1))
        {
            Debug.Log("get mouse");
            Vector2 cur = (Vector2)Input.mousePosition;
            Vector2 delta = cur - _lastPos;

            // 围绕世界 Y 轴水平旋转，围绕世界 X 轴垂直旋转
            transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);

            _lastPos = cur;
        }
    }

    /// <summary>
    /// 移动端：单指拖拽
    /// </summary>
    private void HandleTouchDrag()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 cur = touch.position;
                Vector2 delta = cur - _lastPos;

                transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);

                _lastPos = cur;
            }
        }
    }
}