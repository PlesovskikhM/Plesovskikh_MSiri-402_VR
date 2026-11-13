using UnityEngine;

public class LatheSocket : MonoBehaviour
{
    [Header("Настройки обработки")]
    public float processingTime = 5f;          
    
    [Header("Точные размеры заготовки")]
    public Vector3 rawScale = new Vector3(100f, 100f, 100f);       // Размер сырой заготовки
    public Vector3 processedScale = new Vector3(60f, 60f, 100f);   // Размер после проточки

    [Header("Параметры материала (URP Lit)")]
    public Color silverColor = new Color(0.8f, 0.8f, 0.8f);      
    public float metalness = 0.9f;                               
    public float smoothness = 0.8f;                              

    private GameObject currentWorkpiece;
    private float timer = 0f;
    private bool isProcessed = false;
    private LatheController latheController;

    public GameObject CurrentWorkpiece => currentWorkpiece;

    void Start()
    {
        latheController = FindFirstObjectByType<LatheController>();
    }

    void Update()
    {
        // Плавная привязка к позиции и вращению патрона (без изменения масштаба через родителей)
        if (currentWorkpiece != null)
        {
            currentWorkpiece.transform.position = transform.position;
            currentWorkpiece.transform.rotation = transform.rotation;

            // Логика обработки станка
            if (latheController != null && latheController.IsRunning && !isProcessed)
            {
                timer += Time.deltaTime;

                if (timer >= processingTime)
                {
                    ProcessWorkpiece();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Workpiece") && currentWorkpiece == null)
        {
            AttachWorkpiece(other.gameObject);
        }
    }

    public void AttachWorkpiece(GameObject workpiece)
    {
        currentWorkpiece = workpiece;
        timer = 0f;

        // Проверяем по цвету, была ли деталь уже обработана
        Renderer rend = currentWorkpiece.GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            Color col = rend.material.GetColor("_BaseColor");
            isProcessed = (col.r < 0.9f); 
        }
        else
        {
            isProcessed = false;
        }

        HandInteraction hands = FindFirstObjectByType<HandInteraction>();
        if (hands != null)
        {
            hands.ForceRelease(currentWorkpiece);
        }

        Rigidbody rb = currentWorkpiece.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Collider colld = currentWorkpiece.GetComponent<Collider>();
        if (colld != null)
        {
            colld.isTrigger = false;
        }

        // Жестко выставляем нужный размер в зависимости от состояния
        currentWorkpiece.transform.localScale = isProcessed ? processedScale : rawScale;
    }

    public GameObject ReleaseToHand()
    {
        if (currentWorkpiece == null) return null;

        GameObject detachedObj = currentWorkpiece;
        
        detachedObj.tag = "Workpiece";

        Rigidbody rb = detachedObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        Collider col = detachedObj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true; 
        }

        // Сохраняем актуальный размер при выдаче в руку
        detachedObj.transform.localScale = isProcessed ? processedScale : rawScale;

        currentWorkpiece = null;
        timer = 0f;

        return detachedObj;
    }

    private void ProcessWorkpiece()
    {
        isProcessed = true;
        Debug.Log("Обработка завершена! Деталь проточена.");

        if (currentWorkpiece != null)
        {
            // Уменьшаем диаметр (проточка)
            currentWorkpiece.transform.localScale = processedScale;

            Renderer rend = currentWorkpiece.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", silverColor);

                if (mat.HasProperty("_Metallic"))
                    mat.SetFloat("_Metallic", metalness);

                if (mat.HasProperty("_Smoothness"))
                    mat.SetFloat("_Smoothness", smoothness);
            }
        }
    }
}