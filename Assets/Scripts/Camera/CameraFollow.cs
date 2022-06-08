using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followObject;

    private void Update()
    {
        transform.position = new Vector3(followObject.position.x, followObject.position.y + 4, followObject.position.z);
    }
}