using UnityEngine;

public class ButtonPressed : MonoBehaviour
{
    public int doorIndex = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.doorInteraction(doorIndex);
            Debug.Log("Opened a door with index: " + doorIndex);
        }
    }
}
