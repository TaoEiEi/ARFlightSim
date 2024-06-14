using UnityEngine;

public class Test1 : MonoBehaviour
{
    // ประกาศตัวแปร GameObject ของตัวเอง
    public GameObject gameObjectToTilt;

    // ปรับความเร็วของการเอียง
    public float tiltSpeed = 5.0f;

    void Update()
    {
        // รับค่า y จาก Joystick (เราไม่ต้องใช้ค่า x)
        float y = Input.GetAxis("VerticalJoy");

        // เอียง GameObject ตามค่า y ที่ได้รับ
        TiltGameObject(y);
    }

    void TiltGameObject(float y)
    {
        // คำนวณมุมการเอียง
        float tiltX = y * tiltSpeed; // เอียงตามแกน X เมื่อโยก Joystick ไปข้างหน้าและหลัง

        // กำหนดมุมเอียงของ GameObject
        Vector3 currentAngles = gameObjectToTilt.transform.eulerAngles;
        gameObjectToTilt.transform.eulerAngles = new Vector3(tiltX, currentAngles.y, currentAngles.z);
    }
}