using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float speedMultiplier = 1.0f; // 速度倍增器

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 使用从串口读取到的速度数据来控制小球的运动
        Vector3 velocity = SimpleSerialReader.velocity;
        transform.Translate(velocity * speedMultiplier * Time.deltaTime);
    }
}