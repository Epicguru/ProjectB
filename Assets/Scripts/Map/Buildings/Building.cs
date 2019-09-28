
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

        public RectInt Position;
        
        public void GoToPosition()
        {
            transform.position = new Vector3(Position.x, Position.y);
        }

        private void Start()
        {
            GoToPosition();
        }
    }
}
