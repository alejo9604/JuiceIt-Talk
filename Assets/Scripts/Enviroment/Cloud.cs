using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Cloud : MonoBehaviour
    {
        public float Height = 100;
        public float Speed = 1;
        public Vector2 MoveDirection;

        public void Move()
        {
            //transform.Translate(MoveDirection * Speed * Time.deltaTime);
            transform.position += (Vector3)MoveDirection * Speed * Time.deltaTime;
        }
    }
}