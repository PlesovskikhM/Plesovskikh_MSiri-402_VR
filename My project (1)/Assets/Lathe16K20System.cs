using UnityEngine;

public class Lathe16K20System : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 3.5f; // Дистанция, с которой игрок может взаимодействовать со станком
    private Transform playerTransform;

    private Transform chuckTransform;       // Ссылка на патрон станка
    private GameObject currentWorkpiece;    // Ссылка на установленную заготовку
    private bool hasWorkpiece = false;      // Установлена ли заготовка
    private bool isRunning = false;         // Включен ли станок (вращается ли шпиндель)

    void Start()
    {
        // 1. Автоматически строим детализированный станок 16К20
        BuildLatheMachine();

        // 2. Ищем игрока по тегу "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Внимание! Игрок с тегом 'Player' не найден на сцене. Проверьте Tag у объекта-игрока.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Считаем расстояние от станка до игрока
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Если игрок подошел достаточно близко
        if (distance <= interactionDistance)
        {
            // Нажатие клавиши 'E' — поставить или снять заготовку
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasWorkpiece)
                {
                    MountWorkpiece();
                }
                else
                {
                    UnmountWorkpiece();
                }
            }

            // Нажатие клавиши 'Space' (Пробел) — запустить или остановить вращение шпинделя
            if (Input.GetKeyDown(KeyCode.Space) && hasWorkpiece)
            {
                isRunning = !isRunning;
                Debug.Log(isRunning ? "Станок 16К20 запущен: шпиндель вращается." : "Станок остановлен.");
            }
        }

        // Если станок работает, вращаем патрон вместе с заготовкой
        if (isRunning && chuckTransform != null)
        {
            chuckTransform.Rotate(Vector3.forward, 800f * Time.deltaTime);
        }
    }

    // Процедурная сборка станка 16К20 из примитивов Unity
    void BuildLatheMachine()
    {
        Color bodyColor = new Color(0.15f, 0.18f, 0.22f);   // Заводской серо-синий цвет корпуса
        Color ironColor = new Color(0.25f, 0.27f, 0.3f);    // Чугунные элементы и станина
        Color chromeColor = new Color(0.75f, 0.78f, 0.8f);  // Полированный металл

        // 1. Тумбы основания (левая и правая)
        CreatePart(transform, "LeftLeg", PrimitiveType.Cube, new Vector3(-0.8f, 0.35f, 0), new Vector3(0.5f, 0.7f, 0.8f), ironColor);
        CreatePart(transform, "RightLeg", PrimitiveType.Cube, new Vector3(0.9f, 0.35f, 0), new Vector3(0.4f, 0.7f, 0.8f), ironColor);

        // 2. Станина с направляющими
        CreatePart(transform, "Bed", PrimitiveType.Cube, new Vector3(0.1f, 0.75f, 0), new Vector3(2.5f, 0.25f, 0.9f), bodyColor);

        // 3. Передняя бабка (коробка скоростей слева)
        GameObject headstock = CreatePart(transform, "Headstock", PrimitiveType.Cube, new Vector3(-0.6f, 1.15f, 0), new Vector3(0.8f, 0.55f, 0.85f), bodyColor);

        // 4. Токарный патрон
        GameObject chuck = CreatePart(headstock.transform, "Chuck", PrimitiveType.Cylinder, new Vector3(0, 0, 0.55f), new Vector3(0.6f, 0.15f, 0.6f), chromeColor);
        chuck.transform.localEulerAngles = new Vector3(90, 0, 0);
        chuckTransform = chuck.transform; // Сохраняем ссылку на патрон

        // 5. Суппорт (каретка)
        GameObject carriage = CreatePart(transform, "Carriage", PrimitiveType.Cube, new Vector3(0.1f, 0.95f, 0), new Vector3(0.7f, 0.2f, 0.8f), ironColor);
        CreatePart(carriage.transform, "CrossSlide", PrimitiveType.Cube, new Vector3(0, 0.18f, 0.1f), new Vector3(0.45f, 0.15f, 0.5f), ironColor);

        // 6. Задняя бабка (справа)
        CreatePart(transform, "Tailstock", PrimitiveType.Cube, new Vector3(0.9f, 1.05f, 0), new Vector3(0.5f, 0.45f, 0.65f), bodyColor);
    }

    // Установка заготовки в патрон
    void MountWorkpiece()
    {
        if (chuckTransform != null)
        {
            currentWorkpiece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            currentWorkpiece.name = "Workpiece";
            
            // Делаем заготовку дочерней для патрона, чтобы она крутилась вместе с ним
            currentWorkpiece.transform.SetParent(chuckTransform);
            currentWorkpiece.transform.localPosition = new Vector3(0, 0, 0.65f);
            currentWorkpiece.transform.localEulerAngles = Vector3.zero;
            currentWorkpiece.transform.localScale = new Vector3(0.35f, 0.9f, 0.35f);
            
            SetMaterialColor(currentWorkpiece, new Color(0.8f, 0.82f, 0.85f), 0.8f, 0.5f);

            hasWorkpiece = true;
            Debug.Log("Заготовка успешно установлена в патрон станка 16К20!");
        }
    }

    // Снятие заготовки со станка
    void UnmountWorkpiece()
    {
        if (currentWorkpiece != null)
        {
            isRunning = false; // Останавливаем станок
            Destroy(currentWorkpiece);
            currentWorkpiece = null;
            hasWorkpiece = false;
            Debug.Log("Заготовка снята со станка.");
        }
    }

    // Вспомогательный метод для создания частей станка
    GameObject CreatePart(Transform parent, string name, PrimitiveType primitive, Vector3 localPos, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent);
        part.transform.localPosition = localPos;
        part.transform.localEulerAngles = Vector3.zero;
        part.transform.localScale = scale;
        
        SetMaterialColor(part, color, 0.4f, 0.5f);
        return part;
    }

    // Настройка материалов под Unity 6 (URP / Standard)
    void SetMaterialColor(GameObject obj, Color color, float smoothness, float metallic)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = color;
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            renderer.material = mat;
        }
    }
}