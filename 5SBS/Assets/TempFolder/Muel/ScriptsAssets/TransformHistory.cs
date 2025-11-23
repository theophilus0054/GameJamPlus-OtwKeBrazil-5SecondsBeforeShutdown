using System.Collections.Generic;
using UnityEngine;

public class TransformHistory : MonoBehaviour
{
    [Header("Position History Target")]
    public Transform parentTarget;      // parent berisi child → posisi akan disimpan

    [Header("Object History Target")]
    public Transform parentObject;      // parent dari objek yang di-spawn
    public Transform playerPosition;    // posisi spawn

    [Header("Prefab untuk Spawn")]
    public GameObject spawnPrefab;


    // -------------------------------
    // DATA HISTORY
    // -------------------------------

    // history posisi child2
    private List<List<Vector3>> positionHistory = new List<List<Vector3>>();

    // history object yang pernah dibuat
    private List<GameObject> objectHistory = new List<GameObject>();

    // ============================================
    // SAVE LOG: Simpan child positions + spawn object
    // ============================================
    public void SaveLog()
    {
        if (parentTarget != null)
            SavePositionLog();

        if (parentObject != null && spawnPrefab != null && playerPosition != null)
            SaveObjectLog();
    }


    // simpan posisi semua child parentTarget
    private void SavePositionLog()
    {
        List<Vector3> snapshot = new List<Vector3>();

        foreach (Transform child in parentTarget)
            snapshot.Add(child.localPosition);

        positionHistory.Add(snapshot);

        Debug.Log("Saved Position Log. Total: " + positionHistory.Count);
    }


    // spawn object baru dan simpan ke history
    private void SaveObjectLog()
    {
        GameObject newObj = Instantiate(
            spawnPrefab,
            playerPosition.position,
            Quaternion.identity,
            parentObject
        );

        objectHistory.Add(newObj);

        Debug.Log("Spawned & Saved Object Log. Total: " + objectHistory.Count);
    }


    // ============================================
    // UNDO: kembalikan child positions + hapus last object
    // ============================================
    public void Undo()
    {
        UndoPosition();
        UndoObject();
    }


    private void UndoPosition()
    {
        if (positionHistory.Count == 0)
        {
            Debug.Log("No position history.");
            return;
        }

        List<Vector3> lastSnapshot = positionHistory[positionHistory.Count - 1];

        int index = 0;
        foreach (Transform child in parentTarget)
        {
            child.localPosition = lastSnapshot[index];
            index++;
        }

        positionHistory.RemoveAt(positionHistory.Count - 1);

        Debug.Log("Undo Position. Remaining: " + positionHistory.Count);
    }


    private void UndoObject()
    {
        if (objectHistory.Count == 0)
        {
            Debug.Log("No object history.");
            return;
        }

        GameObject lastObj = objectHistory[objectHistory.Count - 1];

        if (lastObj != null)
            Destroy(lastObj);

        objectHistory.RemoveAt(objectHistory.Count - 1);

        Debug.Log("Undo Object. Remaining: " + objectHistory.Count);
    }


    public void ClearHistory()
    {
        positionHistory.Clear();
        objectHistory.Clear();

        Debug.Log("Cleared all history.");
    }

    public void ClearDeadBodies()
    {
        foreach (GameObject obj in objectHistory)
        {
            if (obj != null)
                Destroy(obj);
        }

        objectHistory.Clear();

        Debug.Log("All dead bodies despawned.");
    }

}