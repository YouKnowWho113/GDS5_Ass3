using System.Collections.Generic;
using UnityEngine;

public class RandomDishSpawner : MonoBehaviour
{
    [Header("Dish Prefabs")]
    public List<GameObject> dishPrefabs = new List<GameObject>();

    [Header("Spawn Location")]
    public Transform spawnParent;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public bool spawnOnStart = false;
    public bool destroyPreviousDish = true;

    private GameObject currentDishObject;

    private void Start()
    {
        if (spawnOnStart)
            SpawnRandomDish();
    }

    public void SpawnRandomDish()
    {
        if (dishPrefabs == null || dishPrefabs.Count == 0)
        {
            Debug.LogWarning("[RandomDishSpawner] No dish prefabs assigned.");
            return;
        }

        int randomIndex = Random.Range(0, dishPrefabs.Count);
        SpawnDish(randomIndex);
    }

    public void SpawnDish(int index)
    {
        if (index < 0 || index >= dishPrefabs.Count)
        {
            Debug.LogWarning("[RandomDishSpawner] Invalid dish index: " + index);
            return;
        }

        if (destroyPreviousDish && currentDishObject != null)
            Destroy(currentDishObject);

        GameObject prefab = dishPrefabs[index];

        if (prefab == null)
        {
            Debug.LogWarning("[RandomDishSpawner] Dish prefab is null at index: " + index);
            return;
        }

        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        if (spawnPoint != null)
        {
            position = spawnPoint.position;
            rotation = spawnPoint.rotation;
        }

        Transform parent = spawnParent != null ? spawnParent : null;

        currentDishObject = Instantiate(prefab, position, rotation, parent);

        DishInstance dishInstance = currentDishObject.GetComponent<DishInstance>();

        if (dishInstance == null)
            dishInstance = currentDishObject.GetComponentInChildren<DishInstance>();

        if (dishInstance != null)
        {
            dishInstance.RegisterDish();
        }
        else
        {
            Debug.LogWarning(
                "[RandomDishSpawner] Spawned dish has no DishInstance: " +
                currentDishObject.name
            );
        }

        Debug.Log("[RandomDishSpawner] Spawned dish: " + currentDishObject.name);
    }

    public void ClearCurrentDish()
    {
        if (currentDishObject != null)
        {
            Destroy(currentDishObject);
            currentDishObject = null;
        }

        if (EvidenceNotebook.Instance != null)
            EvidenceNotebook.Instance.ClearEvidence();

        if (DeductionManager.Instance != null)
            DeductionManager.Instance.ResetReport();
    }

    public GameObject GetCurrentDishObject()
    {
        return currentDishObject;
    }
}