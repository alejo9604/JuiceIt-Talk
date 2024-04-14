using TMPro;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Tile : MonoBehaviour
    {
        // OddR Coordinates
        public Vector2Int OddR_Coord;

        // Axial Coordinates
        public Vector2Int Axial_Coord;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _selected;
        [SerializeField] private TextMeshPro _text;

        public void SetData(int x, int y, Sprite sprite, int q, int r)
        {
            OddR_Coord = new Vector2Int(x, y);
            Axial_Coord = new Vector2Int(q, r);
            _spriteRenderer.sprite = sprite;
            //_text.text = $"{Q}, {R}\n{X}, {Y}";
            //_text.text = $"{Q}\n    {R}";
        }

        public void SetSelected(bool selected) => _selected.SetActive(selected);
    }
}