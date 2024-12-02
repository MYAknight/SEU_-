using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationWall : MonoBehaviour
{
    public float rotationSpeed = 10.0f; // 旋转速度

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   void Update()
    {
        // 沿着X轴旋转平面
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
 // 碰撞检测
    void OnCollisionEnter(Collision collision)
    {
        // 输出碰撞信息
        Debug.Log("碰撞发生: " + collision.gameObject.name);
    }
   
}
