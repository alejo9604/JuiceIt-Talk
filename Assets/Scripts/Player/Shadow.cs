using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private Transform _shadow;
        private Vector2 _baseOffset;

        private void Start()
        {
            _baseOffset = _shadow.localPosition;
        }
        
        private void Update()
        {
            _shadow.localPosition = Quaternion.Euler(0, 0, -transform.eulerAngles.z) * _baseOffset;
        }
    }
}