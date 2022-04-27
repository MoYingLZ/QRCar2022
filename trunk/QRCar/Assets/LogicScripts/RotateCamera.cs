using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCamera : MonoBehaviour
{
    private Vector3 lastMousePos;
    public float SensorRate = 0.1f;
    private bool isOnUI = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverGameObject(Input.mousePosition))
            {
                isOnUI = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isOnUI = false;
        }
        if (Input.GetMouseButtonDown(0) && !isOnUI)
        {
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && !isOnUI)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + (lastMousePos.y - Input.mousePosition.y)* SensorRate,transform.rotation.eulerAngles.y + (lastMousePos.x - Input.mousePosition.x) * SensorRate, transform.eulerAngles.z);
        }
        lastMousePos = Input.mousePosition;
    }

    public void ChangeZ(float z)
    {
        transform.GetChild(0).transform.localPosition = new Vector3(0,0,-2.7f- 3*z);
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
