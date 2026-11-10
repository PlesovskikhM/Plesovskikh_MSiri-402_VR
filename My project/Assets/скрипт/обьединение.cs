using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        CombineCurrentMeshes();
    }

    [ContextMenu("Объединить меши (Combine)")]
    public void CombineCurrentMeshes()
    {
        // Находим все компоненты MeshFilter у дочерних объектов
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        Matrix4x4 matrix = transform.worldToLocalMatrix;
        int i = 0;

        while (i < meshFilters.Length)
        {
            // Исключаем корневой объект, если у него тоже есть MeshFilter
            if (meshFilters[i].gameObject == gameObject)
            {
                i++;
                continue;
            }

            combine[i].mesh = meshFilters[i].sharedMesh;
            // Переводим координаты вершин в локальное пространство родителя
            combine[i].transform = matrix * meshFilters[i].transform.localToWorldMatrix;
            
            // Отключаем или удаляем старые дочерние объекты
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        // Создаем новый общий меш
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combine, true, true);

        // Присваиваем объединенный меш текущему объекту
        transform.GetComponent<MeshFilter>().mesh = finalMesh;

        // Включаем отображение на главном объекте
        if (transform.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        Debug.Log("Меши успешно объединены в один!");
    }
}