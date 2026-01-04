using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.x = target.position.x;
        position.y = target.position.y;
        transform.position = position;
    }
}
