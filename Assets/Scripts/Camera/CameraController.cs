using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;

    [Header("Settings")]
    public Vector2 SizeRange = new Vector2(1f, 100f);
    [Range(0f, 1f)]
    public float SizeSensitivity = 0.1f;

    [Header("Controls")]
    public float TargetSize = 25f;

    [Header("Lerping")]
    public float SizeLerp = 5f;

    private Vector2 mouseStart, camStart;

    private void Awake()
    {
        InvokeRepeating("Tick", 0f, 1f / 60f);
    }

    private void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Vector3 newPos = new Vector3(x, y, z);

        if (Input.GetMouseButtonDown(2))
        {
            mouseStart = Input.mousePosition;
            camStart = new Vector2(x, y);
        }
        if (Input.GetMouseButton(2))
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - mouseStart;
            float xChange = -(mouseDelta.x / Screen.width) * InputManager.CameraBounds.size.x;
            float yChange = -(mouseDelta.y / Screen.height) * InputManager.CameraBounds.size.y;

            Vector2 updated = camStart + new Vector2(xChange, yChange);
            newPos.x = updated.x;
            newPos.y = updated.y;
        }

        transform.position = newPos;
    }

    private void Tick()
    {
        // Input collection and application.
        float scroll = Input.mouseScrollDelta.y;
        if(scroll != 0f)
            TargetSize *= scroll < 0f ? 1f + SizeSensitivity : 1f - SizeSensitivity;
        TargetSize = Mathf.Clamp(TargetSize, SizeRange.x, SizeRange.y);        

        // Lerping.
        const float DELTA = 1f / 60f;
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, TargetSize, DELTA * SizeLerp);
    }
}
