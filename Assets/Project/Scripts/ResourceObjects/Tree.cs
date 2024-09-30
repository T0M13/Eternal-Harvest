using UnityEngine;

public class Tree : MonoBehaviour, IDroppable
{
    public void OnObjectDropped(GameObject gameObj)
    {
       if(gameObj.GetComponent<AIAgent>() != null)
        {
            AIAgent ai = gameObj.GetComponent<AIAgent>();
            //ai.TransitionToState();
        }
    }
}
