
using Reflex.Extensions;
using Reflex.Injectors;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private Dictionary<GameObject, ObjectPool<GameObject>> m_poolMap;
    private Dictionary<GameObject, GameObject> m_cloneToPrefabMap;
    private Dictionary<GameObject, Transform> m_poolParentMap;

    public void Awake()
    {
        m_poolMap = new Dictionary<GameObject, ObjectPool<GameObject>>();
        m_cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
        m_poolParentMap = new Dictionary<GameObject, Transform>();
    }

    public T SpawnObject<T>(T prefab) where T: Component
    {
        return SpawnObject<T>(prefab.gameObject);
    }

    public T SpawnObject<T>(GameObject prefab) where T: Object
    {
        if(!m_poolMap.ContainsKey(prefab))
        {
            CreatePool(prefab);
        }

        GameObject poolObject = m_poolMap[prefab].Get();

        if (poolObject != null)
        {
            // return pooled game object
            if (typeof(T) == typeof(GameObject))
                return poolObject as T;

            // Return desired component of pooled object
            T component = poolObject.GetComponent<T>();
            if(component != null)
                return component;

            Debug.LogError($"Type T [{typeof(T)}] is missing from prefab [{prefab.name}]");
        }

        return null;
    }

    public void ReleaseObject(GameObject pooledObject)
    {
        if(m_cloneToPrefabMap.TryGetValue(pooledObject, out GameObject prefabKey))
        {
            if(m_poolMap.TryGetValue(prefabKey, out ObjectPool<GameObject> pool))
            {
                pool.Release(pooledObject);
            }
            else
            {
                Debug.LogError($"Unable to find pool for {prefabKey.name}");
            }
        }
        else
        {
            Debug.LogError($"Unable to find prefab key for cloned object [{pooledObject.name}]");
        }
    }

    private void CreatePool(GameObject prefab)
    {
        m_poolMap.Add(prefab, new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleasedObject,
                actionOnDestroy: OnDestroyObject
            ));

        var poolParent = new GameObject(prefab.name + " pool");
        poolParent.transform.parent = transform;
        m_poolParentMap.Add(prefab, poolParent.transform);
    }

    private GameObject CreateObject(GameObject prefab)
    {
        prefab.SetActive(false);
        GameObject newObject = Instantiate(prefab);
        prefab.SetActive(true);

        newObject.transform.parent = m_poolParentMap[prefab];
        m_cloneToPrefabMap.Add(newObject, prefab);

        // Inject to Root Container so that this can be used on DontDestroyOnLoad
        GameObjectInjector.InjectObject(newObject, Reflex.Core.Container.RootContainer);

        return newObject;
    }

    private void OnGetObject(GameObject pooledObject)
    {
        pooledObject.SetActive(true);
    }

    private void OnReleasedObject(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    private void OnDestroyObject(GameObject pooledObject)
    {
        m_cloneToPrefabMap.Remove(pooledObject);
        Destroy(pooledObject);
    }
}
