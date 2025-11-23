using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform objectParent;  // Parent untuk semua spawned objects

    // Semua object yang pernah di-spawn
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> deadBodies = new List<GameObject>();
    

    // ==============================================================
    // SPAWN OBJECT
    // ==============================================================
    public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("SpawnObject gagal: prefab kosong!");
            return null;
        }

        GameObject newObj = Instantiate(prefab, position, rotation, objectParent);
        spawnedObjects.Add(newObj);

        return newObj;
    }


    // ==============================================================
    // DESPAWN / DESTROY OBJECT
    // ==============================================================
    public void DespawnObject(GameObject obj)
    {
        if (obj == null) return;

        if (spawnedObjects.Contains(obj))
            spawnedObjects.Remove(obj);

        Destroy(obj);
    }


    // ==============================================================
    // CLEAR ALL OBJECTS IN LEVEL
    // ==============================================================
    public void ClearAllObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
    }


    // ==============================================================
    // CHECK COLLISION WITH DEAD BODIES
    // Untuk memastikan player respawn tidak numpuk objek
    // ==============================================================
    public bool IsCollidingWithDeadBodies(Vector3 checkPos, GameObject player)
    {
        if (player == null) return false;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return false;

        // Check only dead bodies now (dead bodies are tracked separately)
        foreach (var obj in deadBodies)
        {
            if (obj == null) continue;

            Collider2D objCol = obj.GetComponent<Collider2D>();
            if (objCol == null) continue;

            // Simulate overlap test
            if (objCol.bounds.Contains(checkPos))
                return true;
        }

        return false;
    }


    // ==============================================================
    // DEAD-BODY HELPERS
    // ==============================================================
    public GameObject SpawnDeadBody(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject dead = SpawnObject(prefab, position, rotation);
        if (dead != null)
        {
            // Try to match collider and rigidbody to the player's components (size/shape)
            GameObject player = GameManager.Instance != null ? GameManager.Instance.player : null;
            if (player != null)
            {
                Collider2D playerCol = player.GetComponent<Collider2D>();
                if (playerCol != null)
                {
                    // Remove any existing collider on the dead prefab to replace with a matching one
                    Collider2D existing = dead.GetComponent<Collider2D>();
                    if (existing != null) Destroy(existing);

                    // Copy common collider types
                    if (playerCol is BoxCollider2D boxCol)
                    {
                        var c = dead.AddComponent<BoxCollider2D>();
                        c.size = boxCol.size;
                        c.offset = boxCol.offset;
                        c.sharedMaterial = boxCol.sharedMaterial;
                        // usedByComposite is deprecated; do not copy it here.
                        // If you rely on composite behaviour, update to use compositeOperation or
                        // configure CompositeCollider2D on the parent.
                    }
                    else if (playerCol is CircleCollider2D circCol)
                    {
                        var c = dead.AddComponent<CircleCollider2D>();
                        c.radius = circCol.radius;
                        c.offset = circCol.offset;
                        c.sharedMaterial = circCol.sharedMaterial;
                    }
                    else if (playerCol is CapsuleCollider2D capCol)
                    {
                        var c = dead.AddComponent<CapsuleCollider2D>();
                        c.size = capCol.size;
                        c.offset = capCol.offset;
                        c.direction = capCol.direction;
                        c.sharedMaterial = capCol.sharedMaterial;
                    }
                    else if (playerCol is PolygonCollider2D polyCol)
                    {
                        var c = dead.AddComponent<PolygonCollider2D>();
                        c.pathCount = polyCol.pathCount;
                        for (int i = 0; i < polyCol.pathCount; i++)
                        {
                            c.SetPath(i, polyCol.GetPath(i));
                        }
                        c.sharedMaterial = polyCol.sharedMaterial;
                    }
                    // if other collider types exist, keep prefab's collider (if any)
                }

                // Rigidbody2D: copy basic properties or add a static body if none
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                Rigidbody2D deadRb = dead.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    if (deadRb == null) deadRb = dead.AddComponent<Rigidbody2D>();
                    deadRb.bodyType = playerRb.bodyType;
                    deadRb.mass = playerRb.mass;
                    deadRb.gravityScale = playerRb.gravityScale;
                    deadRb.constraints = playerRb.constraints;
                    deadRb.interpolation = playerRb.interpolation;
                }
                else
                {
                    // If player has no Rigidbody, make dead body static obstacle
                    if (deadRb == null) deadRb = dead.AddComponent<Rigidbody2D>();
                    deadRb.bodyType = RigidbodyType2D.Static;
                }
            }

            deadBodies.Add(dead);
        }
        return dead;
    }

    public GameObject GetLastDeadBody()
    {
        if (deadBodies.Count == 0) return null;
        return deadBodies[deadBodies.Count - 1];
    }

    public void RemoveLastDeadBody()
    {
        if (deadBodies.Count == 0) return;
        GameObject last = deadBodies[deadBodies.Count - 1];
        if (last != null)
        {
            if (spawnedObjects.Contains(last)) spawnedObjects.Remove(last);
            Destroy(last);
        }
        deadBodies.RemoveAt(deadBodies.Count - 1);
    }

    public void ClearDeadBodies()
    {
        foreach (var b in deadBodies)
        {
            if (b != null)
            {
                if (spawnedObjects.Contains(b)) spawnedObjects.Remove(b);
                Destroy(b);
            }
        }
        deadBodies.Clear();
    }


    // ==============================================================
    // GET LAST SPAWNED OBJECT (Optional Helper)
    // ==============================================================
    public GameObject GetLastObject()
    {
        if (spawnedObjects.Count == 0) return null;
        return spawnedObjects[spawnedObjects.Count - 1];
    }

    // Akses Dead Bodies
    public int GetDeadBodyCount()
    {
        return deadBodies.Count;
    }

    // public List<GameObject> GetAllDeadBodies()
    // {
    //     return new List<GameObject>(deadBodies);
    // }
}
