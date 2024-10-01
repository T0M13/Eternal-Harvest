using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionDraggableObject : DraggableObject
{
    [SerializeField] private AIAgent aIAgent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        aIAgent = GetComponent<AIAgent>();
    }

    public override void OnPickUp()
    {
        aIAgent.StateStop();

    }

    public override void OnDrop()
    {
        aIAgent.StateStart();
    }

    public override void UpdateSortingLayer(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
