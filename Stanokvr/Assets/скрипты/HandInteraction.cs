using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    [Header("Настройки")]
    public Transform holdPoint; 
    public float reachDistance = 3f; 

    private GameObject heldObject;
    private Vector3 savedScale = new Vector3(100f, 100f, 100f);

    void Update()
    {
        // Если объект в руке, постоянно держим его перед камерой
        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = holdPoint.rotation;
            heldObject.transform.localScale = savedScale; // Жестко удерживаем правильный размер
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, reachDistance))
                {
                    // 1. Клик по рычагу станка
                    if (hit.collider.CompareTag("LatheButton"))
                    {
                        LatheController lathe = FindFirstObjectByType<LatheController>();
                        if (lathe != null)
                        {
                            lathe.ToggleLathe();
                        }
                    }
                    // 2. Клик по патрону или детали в нем
                    else if (hit.collider.GetComponent<LatheSocket>() != null)
                    {
                        LatheSocket socket = hit.collider.GetComponent<LatheSocket>();
                        if (socket.CurrentWorkpiece != null)
                        {
                            heldObject = socket.ReleaseToHand();
                            savedScale = heldObject.transform.localScale; // Запоминаем текущий размер (сырой или проточенный)
                        }
                    }
                    else if (hit.collider.CompareTag("Workpiece"))
                    {
                        GameObject targetObj = hit.collider.gameObject;

                        LatheSocket socket = FindFirstObjectByType<LatheSocket>();
                        if (socket != null && socket.CurrentWorkpiece == targetObj)
                        {
                            heldObject = socket.ReleaseToHand();
                            savedScale = heldObject.transform.localScale;
                        }
                        else
                        {
                            heldObject = targetObj;
                            savedScale = heldObject.transform.localScale;

                            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                rb.isKinematic = true;
                            }

                            Collider col = heldObject.GetComponent<Collider>();
                            if (col != null)
                            {
                                col.isTrigger = true;
                            }
                        }
                    }
                }
            }
            else
            {
                DropObject();
            }
        }
    }

    public void DropObject()
    {
        if (heldObject != null)
        {
            Collider col = heldObject.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = false; 
            }

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; 
            }

            heldObject.transform.localScale = savedScale;
            heldObject = null;
        }
    }

    public void ForceRelease(GameObject obj)
    {
        if (heldObject == obj)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = false;
            }
            heldObject = null;
        }
    }
}