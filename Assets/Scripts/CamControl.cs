using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float orthoBorderSize;
    public float moveSpeed;
    [Range(0, 1)]
    public float smoothTime;
    public Transform playerTransform;
    [HideInInspector]
    public int worldSize;
    public float orthoSize;

    public void Spawn(Vector3 pos)
    {
        GetComponent<Transform>().position = pos;
        orthoSize = GetComponent<Camera>().orthographicSize;
    }

    public void FixedUpdate(){

        //stop camera from showing out of bounds
        Vector3 pos = GetComponent<Transform>().position;

        pos.x = Mathf.Lerp(pos.x, playerTransform.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, playerTransform.position.y + 2, smoothTime);

        pos.x = Mathf.Clamp(pos.x, 0 + (orthoSize * orthoBorderSize), worldSize - (orthoSize * orthoBorderSize));

        GetComponent<Transform>().position = pos;
    }
}