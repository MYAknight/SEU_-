#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_ADXL345_U.h>

// 创建ADXL345对象
Adafruit_ADXL345_Unified accel = Adafruit_ADXL345_Unified(12345);
// 定义连接振动电机的引脚
const int motorPin = 9;

// 定义时间变量
unsigned long previousTime = 0;
float deltaTime;

// 定义速度和位移变量
float velocityX = 0, velocityY = 0, velocityZ = 0;
float displacementX = 0, displacementY = 0, displacementZ = 0;

// 定义初始加速度变量
float initialAccelX = 0, initialAccelY = 0, initialAccelZ = 0;

// 定义滤波器参数
float alpha = 0.8; // 高通滤波器系数
float gravityX = 0, gravityY = 0, gravityZ = 0;

// 定义初始稳定阶段的采样次数
const int stabilizationSamples = 100;
int sampleCount = 0;

// 偏移量
float offsetRoll = 0;
float offsetPitch = 0;

// 定义全局延迟时间变量
const int delayTime = 10;

void setup() {
  Serial.begin(9600);
  pinMode(motorPin, OUTPUT); // 设置引脚为输出模式

  if (!accel.begin()) {
    Serial.println("No ADXL345 detected ... Check your wiring!");
    while (1);
  }
  accel.setRange(ADXL345_RANGE_16_G);
  Serial.println("ADXL345 initialized.");

  // 计算偏移量
  for (int i = 0; i < 100; i++) {
    sensors_event_t event;
    accel.getEvent(&event);

    float accelX = event.acceleration.x;
    float accelY = event.acceleration.y;
    float accelZ = event.acceleration.z;

    float roll = fmod((-atan2(accelY, accelZ) * 180.0 / PI) + 360.0, 360.0) - 180.0;
    float pitch = -atan2(-accelX, sqrt(accelY * accelY + accelZ * accelZ)) * 180.0 / PI;

    offsetRoll += roll;
    offsetPitch += pitch;
    delay(delayTime);
  }
  offsetRoll /= 100;
  offsetPitch /= 100;

  // 采集200次数据，计算初始加速度并滤除重力加速度
  for (int i = 0; i < stabilizationSamples; i++) {
    sensors_event_t event;
    accel.getEvent(&event);

    // 高通滤波器消除重力加速度
    gravityX = alpha * gravityX + (1 - alpha) * event.acceleration.x;
    gravityY = alpha * gravityY + (1 - alpha) * event.acceleration.y;
    gravityZ = alpha * gravityZ + (1 - alpha) * event.acceleration.z;

    float correctedAccelX = event.acceleration.x - gravityX;
    float correctedAccelY = event.acceleration.y - gravityY;
    float correctedAccelZ = event.acceleration.z - gravityZ;

    initialAccelX += correctedAccelX;
    initialAccelY += correctedAccelY;
    initialAccelZ += correctedAccelZ;

    delay(delayTime); // 每次采集间隔10毫秒
  }
  initialAccelX /= stabilizationSamples;
  initialAccelY /= stabilizationSamples;
  initialAccelZ /= stabilizationSamples;

  previousTime = millis();
}

void loop() {
  sensors_event_t event;
  accel.getEvent(&event);

  // 获取当前时间并计算时间差
  unsigned long currentTime = millis();
  deltaTime = (currentTime - previousTime) / 1000.0; // 转换为秒
  previousTime = currentTime;

  // 高通滤波器消除重力加速度
  gravityX = alpha * gravityX + (1 - alpha) * event.acceleration.x;
  gravityY = alpha * gravityY + (1 - alpha) * event.acceleration.y;
  gravityZ = alpha * gravityZ + (1 - alpha) * event.acceleration.z;

  float correctedAccelX = event.acceleration.x - gravityX;
  float correctedAccelY = event.acceleration.y - gravityY;
  float correctedAccelZ = event.acceleration.z - gravityZ;

  // 应用滤波器，消除小于0.1的加速度
  if (abs(correctedAccelX) < 0.1) correctedAccelX = 0;
  if (abs(correctedAccelY) < 0.1) correctedAccelY = 0;
  if (abs(correctedAccelZ) < 0.1) correctedAccelZ = 0;

  // 计算速度 (v = u + at) 并应用阻力系数
  velocityX = (velocityX + correctedAccelX * deltaTime) * 0.9;
  velocityY = (velocityY + correctedAccelY * deltaTime) * 0.9;
  velocityZ = (velocityZ + correctedAccelZ * deltaTime) * 0.9;

  // 计算位移 (s = ut + 0.5 * a * t^2)
  displacementX += velocityX * deltaTime + 0.5 * correctedAccelX * deltaTime * deltaTime;
  displacementY += velocityY * deltaTime + 0.5 * correctedAccelY * deltaTime * deltaTime;
  displacementZ += velocityZ * deltaTime + 0.5 * correctedAccelZ * deltaTime * deltaTime;

  // 获取加速度值
  float accelX = event.acceleration.x;
  float accelY = event.acceleration.y;
  float accelZ = event.acceleration.z;

  // 计算角度并减去偏移量
  float roll = fmod((-atan2(accelY, accelZ) * 180.0 / PI) + 360.0, 360.0) - 180.0 - offsetRoll;
  float pitch = -atan2(-accelX, sqrt(accelY * accelY + accelZ * accelZ)) * 180.0 / PI - offsetPitch;

  // 打印角度、加速度、速度和位移，以JSON格式输出
  Serial.print("{");
  Serial.print("\"Roll\": "); Serial.print(roll); Serial.print(", ");
  Serial.print("\"Pitch\": "); Serial.print(pitch); Serial.print(", ");
  Serial.print("\"acceleration\": {\"x\": "); Serial.print(correctedAccelX); Serial.print(", \"y\": "); Serial.print(correctedAccelY); Serial.print(", \"z\": "); Serial.print(correctedAccelZ); Serial.print("}, ");
  Serial.print("\"velocity\": {\"x\": "); Serial.print(velocityX); Serial.print(", \"y\": "); Serial.print(velocityY); Serial.print(", \"z\": "); Serial.print(velocityZ); Serial.print("}, ");
  Serial.print("\"displacement\": {\"x\": "); Serial.print(displacementX); Serial.print(", \"y\": "); Serial.print(displacementY); Serial.print(", \"z\": "); Serial.print(displacementZ); Serial.print("}");
  Serial.println("}");

  // 尝试从串口接收一个0-255的数字，并根据数字控制电机运动
  if (Serial.available() > 0) {
    int motorIntensity = Serial.parseInt();
    if (motorIntensity >= 0 && motorIntensity <= 255) {
      setMotorIntensity(motorIntensity);
    }
  }

  delay(delayTime); // 延迟10毫秒
}

// 设置电机的震动强度
void setMotorIntensity(int intensity) {
  analogWrite(motorPin, intensity);
}
