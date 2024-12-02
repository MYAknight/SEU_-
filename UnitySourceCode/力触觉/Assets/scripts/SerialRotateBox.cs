using UnityEngine;

public class SerialRotateBox : MonoBehaviour
{
    public float rotationMultiplier = 1.0f; // 旋转倍增器
    public float lerpSpeed = 5.0f; // 插值速度

    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // 使用从串口读取到的pitch和roll数据来控制Box的旋转
        float pitch = SimpleSerialReader.pitch;
        float roll = SimpleSerialReader.roll;

        // 计算目标旋转角度，反转roll的值
        Vector3 targetEulerAngles = new Vector3(pitch, 0, -roll) * rotationMultiplier;
        targetRotation = Quaternion.Euler(targetEulerAngles);

        // 逐渐插值到目标旋转角度
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);
    }
}