using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 4f;
    public float sensitivity = 2f;

    private float yaw = 0f;   // Угол поворота влево/вправо
    private float pitch = 0f; // Угол поворота вверх/вниз

    void Start()
    {
        // Блокируем курсор в центре экрана при старте игры
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Считываем текущие углы камеры, чтобы не было резкого рывка при старте
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        // 1. Движение на клавиши WASD относительно того, куда смотрит камера
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(moveX, 0, moveZ);

        // 2. Плавное вращение мышкой (и влево/вправо, и вверх/вниз)
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        
        // Ограничиваем угол обзора по вертикали, чтобы камера не переворачивалась
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Применяем вращение к камере
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Нажмите Escape, чтобы освободить курсор мыши
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}