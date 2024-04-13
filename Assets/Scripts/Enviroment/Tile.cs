using TMPro;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Tile : MonoBehaviour
    {
        // OddR Coordinates
        public int X;
        public int Y;
        
        // Axial Coordinates
        public int Q;
        public int R;
        

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _selected;
        [SerializeField] private TextMeshPro _text;

        public void SetData(int x, int y, Sprite sprite, int q, int r)
        {
            X = x;
            Y = y;
            Q = q;
            R = r;
            _spriteRenderer.sprite = sprite;
            //_text.text = $"{Q}, {R}\n{X}, {Y}";
            //_text.text = $"{Q}\n    {R}";
        }

        public void SetSelected(bool selected) => _selected.SetActive(selected);
    }
}