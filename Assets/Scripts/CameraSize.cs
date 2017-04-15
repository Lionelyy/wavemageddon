using UnityEngine;

public class CameraSize : MonoBehaviour
{
    private void Start ()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = ((float)GameManager.instance.columns / 2) / cam.aspect;
    }
}
