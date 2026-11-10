using UnityEngine;

public class SpindleRotator : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Скорость вращения в градусах в секунду")]
    public float rotationSpeed = 500f;

    [Tooltip("Ось, вокруг которой будет вращаться шпиндель")]
    public Vector3 rotationAxis = Vector3.right; // Изменено на Vector3.right (ось X)

    [Header("Управление")]
    [Tooltip("Вращается ли шпиндель прямо сейчас")]
    public bool isRotating = true;

    void Update()
    {
        if (isRotating)
        {
            // Вращение вокруг локальной оси X
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }
}