using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance { get; private set; }
    public static Camera Camera { get { return Instance.Cam; } }
    public static CameraController Controller { get { return Instance.CamController; } }

    public Camera Cam
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;
    public CameraController CamController
    {
        get
        {
            if (_controller == null)
                _controller = GetComponent<CameraController>();
            return _controller;
        }
    }
    private CameraController _controller;

    private void Awake()
    {
        Instance = this;
    }
}
