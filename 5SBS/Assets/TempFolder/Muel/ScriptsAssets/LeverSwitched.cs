using UnityEngine;

public class LeverSwitched : MonoBehaviour
{
    public int doorIndex = 0;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.doorInteraction(doorIndex);
                Debug.Log("Opened a door with index: " + doorIndex);
            }
        }
    }
}
