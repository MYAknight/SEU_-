using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeHit1 : MonoBehaviour
{
   public AudioClip collisionSound;
    public float bounceForce = 20.0f;
    private AudioSource audioSource;
    private SimpleSerialReader serialReader;
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        serialReader = FindObjectOfType<SimpleSerialReader>();
        if (serialReader == null)
        {
            Debug.LogError("未找到SimpleSerialReader实例，请确保它已添加到场景中。");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 播放碰撞声音
        audioSource.PlayOneShot(collisionSound);

        // 施加弹起力
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }

        // 输出碰撞信息
        Debug.Log("障碍物碰撞发生: " + collision.gameObject.name);

        
         // 向串口发送数字200，1秒后发送数字0
        if (serialReader != null && serialReader.serialPort != null && serialReader.serialPort.IsOpen)
        {
            Debug.Log("开始发送串口数据");
            StartCoroutine(SendSerialData());
        }else
        {
            Debug.Log("serialReader is null");
        }
    }
    private IEnumerator SendSerialData()
    {
        Debug.Log("发送数据: 120");
        serialReader.SendData("120");
        yield return new WaitForSeconds(0.3f);
        Debug.Log("发送数据: 0");
        serialReader.SendData("0");
    }
}
