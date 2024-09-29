using UnityEngine;

public class DraggableObject : MonoBehaviour, IDraggable
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPickUp()
    {
        // Logic for what happens when the object is picked up (e.g., change color)
        //Debug.Log(gameObject.name + " picked up.");
    }

    public void OnDrop()
    {
        // Logic for what happens when the object is dropped (e.g., reset color)
        //Debug.Log(gameObject.name + " dropped.");
    }

    public void UpdateSortingLayer(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
