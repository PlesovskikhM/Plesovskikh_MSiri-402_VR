using UnityEngine;

public class LatheController : MonoBehaviour
{
    [Header("Настройки шпинделя")]
    public Transform spindle;
    public float rotationSpeed = 500f;
    
    // Свойство состояния станка, которое проверяет LatheSocket
    public bool IsRunning { get; private set; } = false;

    void Update()
    {
        if (IsRunning && spindle != null)
        {
            spindle.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    public void ToggleLathe()
    {
        IsRunning = !IsRunning;
        Debug.Log("Станок: " + (IsRunning ? "ЗАПУЩЕН" : "ОСТАНОВЛЕН"));
    }
}