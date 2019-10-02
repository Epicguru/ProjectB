
using ProjectB.Units;
using UnityEngine;

namespace ProjectB.Buildings
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(Health))]
    public class Building : MonoBehaviour
    {
        public Unit Unit
        {
            get
            {
                if (_unit == null)
                    _unit = GetComponent<Unit>();
                return _unit;
            }
        }
        private Unit _unit;

        public Health Health { get { return Unit.Health; } }
        public string Name { get { return Unit.Name; } }

        public Sprite PreviewSprite;

        private void GL_DrawUnitSelected(Color color)
        {
            Vector2[] points = Unit.SelectionCollider.GetWorldCorners();

            GameCamera.DrawLine(points[0], points[1], color);
            GameCamera.DrawLine(points[1], points[2], color);
            GameCamera.DrawLine(points[2], points[3], color);
            GameCamera.DrawLine(points[3], points[0], color);
        }
    }
}
