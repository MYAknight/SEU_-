using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Text;

public class SimpleSerialReader : MonoBehaviour
{
    public SerialPort serialPort;
    private StringBuilder dataBuffer = new StringBuilder();
    public static Vector3 velocity = Vector3.zero; // 存储读取到的速度数据
    public static float roll = 0f; // 存储读取到的Roll数据
    public static float pitch = 0f; // 存储读取到的Pitch数据

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.None;
            serialPort.Encoding = Encoding.ASCII; // 确保使用ASCII编码
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
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                {
                    dataBuffer.Append(data);
                    ProcessDataBuffer();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("读取数据时发生错误： " + ex.Message);
            }
        }
    }

    private void ProcessDataBuffer()
    {
        string bufferContent = dataBuffer.ToString();
        int newLineIndex;
        while ((newLineIndex = bufferContent.IndexOf('\n')) >= 0)
        {
            string line = bufferContent.Substring(0, newLineIndex).Trim();
            // Debug.Log("接收到的数据行: " + line);
            ParseJson(line);
            dataBuffer.Remove(0, newLineIndex + 1);
            bufferContent = dataBuffer.ToString();
        }
    }

    private void ParseJson(string jsonString)
    {
        try
        {
            var jsonData = JsonUtility.FromJson<SensorData>(jsonString);
            velocity = new Vector3(jsonData.velocity.x, jsonData.velocity.z, jsonData.velocity.y);
            roll = jsonData.Roll;
            pitch = jsonData.Pitch;
            // Debug.Log($"Velocity: x={jsonData.velocity.x}, y={jsonData.velocity.z}, z={jsonData.velocity.y}");
            // Debug.Log($"Roll: {jsonData.Roll}, Pitch: {jsonData.Pitch}");
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON解析错误： " + ex.Message);
        }
    }
    public void SendData(string data)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine(data);
                Debug.Log("发送数据: " + data);
            }
            catch (Exception ex)
            {
                Debug.LogError("发送数据时发生错误： " + ex.Message);
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

[Serializable]
public class SensorData
{
    public float Roll;
    public float Pitch;
    public Acceleration acceleration;
    public Velocity velocity;
    public Displacement displacement;
}

[Serializable]
public class Acceleration
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Velocity
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Displacement
{
    public float x;
    public float y;
    public float z;
}