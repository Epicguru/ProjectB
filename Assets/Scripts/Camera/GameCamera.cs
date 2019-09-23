using ProjectB.Units;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectB
{
    [RequireComponent(typeof(CameraController))]
    public class GameCamera : MonoBehaviour
    {
        public static GameCamera Instance { get; private set; }
        public static Camera Camera { get { return Instance.Cam; } }
        public static CameraController Controller { get { return Instance.CamController; } }

        public CameraEvent LinesCameraEvent = CameraEvent.AfterEverything;
        public Material LineMat;

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

            RenderPipelineManager.endCameraRendering += RenderLines;
        }

        private void RenderLines(ScriptableRenderContext context, Camera cam)
        {
            if (!Application.isPlaying)
                return;

            GL.PushMatrix();
            LineMat.SetPass(0);
            GL.LoadOrtho();

            GL.Begin(GL.LINES);

            Unit.GL_DrawAllSelected();

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawLine(Vector2 worldStart, Vector2 worldEnd, Color color)
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 screenStart = Camera.WorldToScreenPoint(worldStart) / screenSize;
            Vector2 screenEnd = Camera.WorldToScreenPoint(worldEnd) / screenSize;

            GL.Color(color);
            GL.Vertex(screenStart);
            GL.Vertex(screenEnd);
        }
    }
}
