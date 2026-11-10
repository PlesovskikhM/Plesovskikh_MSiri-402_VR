using UnityEngine;

public class RealisticLathe16K20 : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3.5f;
    private Transform playerTransform;

    private Transform chuckTransform;
    private GameObject currentWorkpiece;
    private bool hasWorkpiece = false;
    private bool isRunning = false;

    void Start()
    {
        BuildRealisticLathe();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Игрок с тегом 'Player' не найден на сцене!");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= interactionDistance)
            {
                // Клавиша E: Поставить / Снять заготовку
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!hasWorkpiece) MountWorkpiece();
                    else UnmountWorkpiece();
                }

                // Клавиша Space: Запустить / Остановить шпиндель
                if (Input.GetKeyDown(KeyCode.Space) && hasWorkpiece)
                {
                    isRunning = !isRunning;
                    Debug.Log(isRunning ? "Станок 16К20 запущен." : "Станок остановлен.");
                }
            }
        }

        // Вращение шпинделя и заготовки
        if (isRunning && chuckTransform != null)
        {
            chuckTransform.Rotate(Vector3.forward, 800f * Time.deltaTime);
        }
    }

    // Создание детализированного станка 16К20 из примитивов
    void BuildRealisticLathe()
    {
        // Цвета материалов
        Color bodyColor = new Color(0.12f, 0.15f, 0.18f);     // Серо-синий заводской цвет станка
        Color ironColor = new Color(0.25f, 0.27f, 0.3f);      // Чугунные элементы и станина
        Color chromeColor = new Color(0.75f, 0.78f, 0.8f);    // Полированный металл / шпиндель
        Color panelColor = new Color(0.15f, 0.15f, 0.15f);    // Пульт управления

        // 1. Тумбы станка (основание левое и правое)
        CreatePart(transform, "LeftStand", PrimitiveType.Cube, new Vector3(-0.8f, 0.35f, 0), new Vector3(0.5f, 0.7f, 0.8f), ironColor, 0.8f, 0.1f);
        CreatePart(transform, "RightStand", PrimitiveType.Cube, new Vector3(0.9f, 0.35f, 0), new Vector3(0.4f, 0.7f, 0.8f), ironColor, 0.8f, 0.1f);

        // 2. Массивная станина с направляющими
        GameObject bed = CreatePart(transform, "LatheBed", PrimitiveType.Cube, new Vector3(0.1f, 0.75f, 0), new Vector3(2.5f, 0.25f, 0.9f), bodyColor);

        // 3. Передняя бабка (Коробка скоростей слева)
        GameObject headstock = CreatePart(transform, "Headstock", PrimitiveType.Cube, new Vector3(-0.6f, 1.15f, 0), new Vector3(0.8f, 0.55f, 0.85f), bodyColor);

        // 4. Шпиндель и токарный патрон
        GameObject chuck = CreatePart(headstock.transform, "Chuck", PrimitiveType.Cylinder, new Vector3(0, 0, 0.55f), new Vector3(0.6f, 0.15f, 0.6f), chromeColor);
        chuck.transform.localEulerAngles = new Vector3(90, 0, 0);
        chuckTransform = chuck.transform; // Сохраняем ссылку на патрон для вращения

        // 5. Кулачки патрона (3 штуки для реалистичности)
        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f;
            GameObject jaw = CreatePart(chuck.transform, "Jaw_" + i, PrimitiveType.Cube, new Vector3(0, 0.3f, 0), new Vector3(0.1f, 0.15f, 0.2f), chromeColor);
            jaw.transform.localPosition = Vector3.zero;
            jaw.transform.localEulerAngles = new Vector3(0, 0, angle);
            jaw.transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * 0.2f, Mathf.Cos(angle * Mathf.Deg2Rad) * 0.2f, -0.2f);
        }

        // 6. Суппорт (Каретка резцедержателя)
        GameObject carriage = CreatePart(transform, "Carriage", PrimitiveType.Cube, new Vector3(0.1f, 0.95f, 0), new Vector3(0.7f, 0.2f, 0.8f), ironColor);
        
        // Поперечные салазки и резцедержатель (башенка)
        GameObject crossSlide = CreatePart(carriage.transform, "CrossSlide", PrimitiveType.Cube, new Vector3(0, 0.18f, 0.1f), new Vector3(0.45f, 0.15f, 0.5f), ironColor);
        GameObject toolPost = CreatePart(crossSlide.transform, "ToolPost", PrimitiveType.Cube, new Vector3(0, 0.2f, 0), new Vector3(0.25f, 0.2f, 0.25f), panelColor);

        // 7. Задняя бабка (справа)
        GameObject tailstock = CreatePart(transform, "Tailstock", PrimitiveType.Cube, new Vector3(0.9f, 1.05f, 0), new Vector3(0.5f, 0.45f, 0.65f), bodyColor);

        // 8. Пульт управления / фартук суппорта спереди
        GameObject apron = CreatePart(carriage.transform, "Apron", PrimitiveType.Cube, new Vector3(0, -0.25f, -0.4f), newVector3(0.6f, 0.4f, 0.15f), panelColor);
    }

    // Метод установки заготовки
    void MountWorkpiece()
    {
        if (chuckTransform != null)
        {
            currentWorkpiece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            currentWorkpiece.name = "Workpiece_MetalRod";
            
            currentWorkpiece.transform.SetParent(chuckTransform);
            currentWorkpiece.transform.localPosition = new Vector3(0, 0, 0.65f);
            currentWorkpiece.transform.localEulerAngles = Vector3.zero;
            currentWorkpiece.transform.localScale = new Vector3(0.35f, 0.9f, 0.35f); // Пропорциональный стальной пруток
            
            SetMaterial(currentWorkpiece, new Color(0.8f, 0.82f, 0.85f), 0.9f, 0.1f); // Блестящий металл с отражениями

            hasWorkpiece = true;
            Debug.Log("Заготовка зажата в патроне станка 16К20.");
        }
    }

    // Метод снятия заготовки
    void UnmountWorkpiece()
    {
        if (currentWorkpiece != null)
        {
            isRunning = false;
            Destroy(currentWorkpiece);
            currentWorkpiece = null;
            hasWorkpiece = false;
            Debug.Log("Заготовка извлечена из патрона.");
        }
    }

    // Вспомогательный метод для создания частей станка
    GameObject CreatePart(Transform parent, string name, PrimitiveType primitive, Vector3 localPos, Vector3 scale, Color color, float smoothness = 0.4f, float metallic = 0.6f)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent);
        part.transform.localPosition = localPos;
        part.transform.localEulerAngles = Vector3.zero;
        part.transform.localScale = scale;
        
        SetMaterial(part, color, smoothness, metallic);
        return part;
    }

    // Настройка реалистичного материала под Unity 6 (URP / Standard) с блеском и металличественным эффектом
    void SetMaterial(GameObject obj, Color color, float smoothness, float metallic)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = color;
            
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            
            // Для стандартного шейдера
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            renderer.material = mat;
        }
    }
}