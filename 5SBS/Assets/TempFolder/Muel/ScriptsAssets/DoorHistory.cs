using System.Collections.Generic;
using UnityEngine;

public class DoorHistory : MonoBehaviour
{
    [Header("Door References (Auto-filled by GameManager)")]
    public List<GameObject> doors = new List<GameObject>();
    public Sprite openSprite;
    public Sprite closedSprite;

    private Stack<List<bool>> undoStack = new Stack<List<bool>>();

    // ======================================================
    // SAVE STATE (Dipanggil setiap PlayerDeath & Start)
    // ======================================================
    public void SaveState()
    {
        if (doors == null || doors.Count == 0)
        {
            Debug.LogWarning("DoorHistory: Doors masih kosong, tidak menyimpan state.");
            return;
        }

        List<bool> snapshot = new List<bool>();

        foreach (GameObject door in doors)
        {
            if (door == null)
            {
                snapshot.Add(false);
                continue;
            }

            Collider2D col = door.GetComponent<Collider2D>();
            bool isOpen = (col != null && col.enabled == false);

            snapshot.Add(isOpen);
        }

        undoStack.Push(snapshot);
        // Debug.Log("DoorHistory: Saved snapshot.");
    }

    // ======================================================
    // UNDO STATE
    // ======================================================
    public void UndoState()
    {
        if (undoStack.Count <= 1)
        {
            Debug.Log("DoorHistory: Tidak ada history untuk undo.");
            return;
        }

        // Buang snapshot sekarang
        undoStack.Pop();

        // Ambil snapshot sebelumnya
        List<bool> previousSnapshot = undoStack.Peek();
        ApplySnapshot(previousSnapshot);
    }

    // ======================================================
    // APPLY SNAPSHOT
    // ======================================================
    private void ApplySnapshot(List<bool> snapshot)
    {
        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i] == null) continue;

            Collider2D col = doors[i].GetComponent<Collider2D>();
            SpriteRenderer sr = doors[i].GetComponent<SpriteRenderer>();

            bool isOpen = snapshot[i];

            if (col != null)
                col.enabled = !isOpen;     // open => collider off

            if (sr != null)
                sr.sprite = isOpen ? openSprite : closedSprite;
        }

        Physics2D.SyncTransforms();
        // Debug.Log("DoorHistory: Snapshot applied.");
    }
}
