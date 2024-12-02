using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO.Ports;

public class sctest01 : MonoBehaviour
{
    private Rigidbody rb; // 控制刚体
    private float movementX;
    private float movementY;
    private SerialPort serialPort;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 初始化串口
        serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
        serialPort.Handshake = Handshake.None;
        serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

        try
        {
            serialPort.Open();
            Debug.Log("串口已打开，开始主动接收数据...");
        }
        catch (Exception ex)
        {
            Debug.LogError("发生错误： " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 处理串口数据
    }

    void Onmove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement);
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data = ((SerialPort)sender).ReadExisting();
        Debug.Log("主动接收到的数据： " + data);
    }
}