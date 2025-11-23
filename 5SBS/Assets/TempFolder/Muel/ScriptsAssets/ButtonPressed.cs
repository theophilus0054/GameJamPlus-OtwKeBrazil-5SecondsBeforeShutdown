using UnityEngine;

public class ButtonPressed : MonoBehaviour
{
    public int doorIndex = 0;
    private bool isPressed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            GameManager.Instance.doorInteraction(doorIndex);
            Debug.Log("Opened a door with index: " + doorIndex);
            isPressed = true;
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.isDoorOpen(doorIndex))
        {
            isPressed = false;
        }
    }
}
