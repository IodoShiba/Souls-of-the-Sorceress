using UnityEngine;
using System.Collections;

namespace ActorFunction
{
    public class Directionable : MonoBehaviour
    {
        public enum Direction
        {
            Right =  1,
            Left  = -1
        }

        [SerializeField] Direction initialDirection;

        Direction currentDirection = Direction.Right;

        private void Awake()
        {
            currentDirection = Direction.Right;
            CurrentDirection = initialDirection;
        }

        public Direction CurrentDirection
        {
            get => currentDirection;
            set
            {
                if(currentDirection == value) { return; }
                currentDirection = value;
                var ls = gameObject.transform.localScale;
                gameObject.transform.localScale = new Vector3((int)value * Mathf.Abs(ls.x), ls.y, ls.z);
            }
        }

        public int CurrentDirectionInt { get => (int)currentDirection; }

        public void ChangeDirection(int direction)
        {
            CurrentDirection = (Direction)direction;
        }

        public void SwitchDirection() { ChangeDirection(-(int)currentDirection); }
    }
}
