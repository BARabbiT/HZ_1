using UnityEngine;

public class SeeAnything : MonoBehaviour
{
    private BeingEventBridge _eventBridge;

    private void Awake()
    {
        _eventBridge = gameObject.GetComponentInParent<BeingEventBridge>();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter: Is triggered");
        if (other.gameObject.name == "Apple")
        {
            _eventBridge.OnSeeEat(other.gameObject.transform.position);
        }
    }
}