using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;

public class SimpleSerialWriter : MonoBehaviour
{
    private SerialPort serialPort;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.None;
            serialPort.Encoding = Encoding.ASCII; // 确保使用ASCII编码
            serialPort.Open();
            Debug.Log("串口已打开，准备发送和接收数据...");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("发生错误： " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // 发送数据
                string message = "200";
                serialPort.WriteLine(message);
                Debug.Log("发送数据: " + message);

                // 读取数据
                string data = serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                {
                    Debug.Log("接收到的数据: " + data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("发送或接收数据时发生错误： " + ex.Message);
            }
        }
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}