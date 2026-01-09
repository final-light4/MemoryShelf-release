using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Scripts;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float fastMovementSpeed = 100f;
    public float freeLookSensitivity = 3f;
    public float zoomSensitivity = 10f;
    public float fastZoomSensitivity = 50f;
    private bool looking = false;

    // 用于限制垂直旋转角度
    private float verticalRotation = 0f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    void Start()
    {
        // 初始化垂直旋转角度为当前相机的X轴旋转
        verticalRotation = transform.localEulerAngles.x;
        // 将角度规范化到-180到180度范围
        if (verticalRotation > 180f)
        {
            verticalRotation -= 360f;
        }
        // 限制在允许的范围内
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        // 移动控制
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (-transform.right * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (transform.right * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = transform.position + (transform.forward * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = transform.position + (-transform.forward * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (transform.up * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (-transform.up * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (Vector3.up * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-Vector3.up * (movementSpeed * Time.deltaTime));
        }

        // 视角控制（任务6的核心要求）
        if (looking)
        {
            // 获取鼠标输入
            float mouseX = Input.GetAxis("Mouse X") * freeLookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * freeLookSensitivity;

            // 垂直旋转：鼠标Y轴控制相机本身的X轴旋转（上下俯仰），并限制角度
            verticalRotation -= mouseY; // 减去是因为鼠标Y轴向上为正，但Unity中向上旋转需要减小角度
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

            // 水平旋转：鼠标X轴控制Y轴旋转（左右平移）
            if (transform.parent != null)
            {
                // 水平旋转应用到父对象（玩家角色对象）的Y轴旋转
                transform.parent.Rotate(0, mouseX, 0, Space.World);
                // 应用垂直旋转到相机的本地X轴旋转，Y轴保持为0（相对于父对象）
                transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
            }
            else
            {
                // 如果没有父对象，水平旋转和垂直旋转都应用到自身
                float currentY = transform.eulerAngles.y;
                float newY = currentY + mouseX;
                // 一次性设置所有旋转
                transform.rotation = Quaternion.Euler(verticalRotation, newY, 0f);
            }
        }

        // 缩放控制
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis > 0)
        {
            GetComponent<Camera>().fieldOfView--;
        }
        else if (axis < 0)
        {
            GetComponent<Camera>().fieldOfView++;
        }

        // 鼠标右键控制视角模式
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void OnDisable()
    {
        StopLooking();
    }

    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}