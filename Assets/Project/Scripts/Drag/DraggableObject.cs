using UnityEngine;

public class DraggableObject : MonoBehaviour, IDraggable
{
    [SerializeField] protected  SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnPickUp()
    {
        // Logic for what happens when the object is picked up (e.g., change color)
        //Debug.Log(gameObject.name + " picked up.");
    }

    public virtual void OnDrop()
    {
        // Logic for what happens when the object is dropped (e.g., reset color)
        //Debug.Log(gameObject.name + " dropped.");
    }

    public virtual void UpdateSortingLayer(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
