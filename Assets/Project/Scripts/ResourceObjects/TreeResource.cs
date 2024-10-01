using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeResource : MonoBehaviour, IDroppable
{

    public void OnObjectDropped(GameObject gameObj)
    {
        if (gameObj.GetComponent<AIAgent>() != null)
        {
            AIAgent ai = gameObj.GetComponent<AIAgent>();
        }
    }

}
