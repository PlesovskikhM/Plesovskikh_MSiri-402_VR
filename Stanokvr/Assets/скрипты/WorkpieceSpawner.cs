using UnityEngine;

public class WorkpieceSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject workpiecePrefab; // Префаб заготовки из папки Assets
    public Transform spawnPoint;       // Точка на столе (SpawnPoint)
    public float respawnDelay = 1.5f;  // Задержка перед появлением новой детали
    public float spawnRadius = 0.5f;   // Радиус проверки: если заготовка унесена дальше этого расстояния, спавним новую

    private GameObject currentSpawnedObject;
    private float timer = 0f;
    private bool needsSpawn = false;

    void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        SpawnNewWorkpiece();
    }

    void Update()
    {
        bool isTakenOrMissing = false;

        if (currentSpawnedObject == null)
        {
            isTakenOrMissing = true;
        }
        else
        {
            // Если объект существует, но игрок унес его далеко от точки спавна (или засунул в станок/в руку)
            float distance = Vector3.Distance(currentSpawnedObject.transform.position, spawnPoint.position);
            
            // Проверяем: если деталь далеко от точки спавна ИЛИ у нее есть родитель (значит она в руке или в патроне)
            if (distance > spawnRadius || currentSpawnedObject.transform.parent != null)
            {
                isTakenOrMissing = true;
            }
        }

        // Если деталь забрали со стола, запускаем таймер для создания новой
        if (isTakenOrMissing)
        {
            if (!needsSpawn)
            {
                needsSpawn = true;
                timer = 0f;
            }

            timer += Time.deltaTime;
            if (timer >= respawnDelay)
            {
                SpawnNewWorkpiece();
                needsSpawn = false;
            }
        }
        else
        {
            // Пока деталь лежит на месте, сбрасываем таймер
            needsSpawn = false;
        }
    }

    void SpawnNewWorkpiece()
    {
        if (workpiecePrefab != null && spawnPoint != null)
        {
            currentSpawnedObject = Instantiate(workpiecePrefab, spawnPoint.position, spawnPoint.rotation);
            currentSpawnedObject.transform.localScale = new Vector3(100f, 100f, 100f);
            currentSpawnedObject.tag = "Workpiece";
        }
    }
}