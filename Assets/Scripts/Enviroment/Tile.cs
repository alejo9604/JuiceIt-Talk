using TMPro;
using UnityEngine;
using DG.Tweening;

namespace AllieJoe.JuiceIt
{
    public class Tile : MonoBehaviour
    {
        // OddR Coordinates
        public Vector2Int OddR_Coord;

        // Axial Coordinates
        public Vector2Int Axial_Coord;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _animatedParent;
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

        [Header("Scale Animation")] 
        public float _animScaleFrom = 0;
        public float _animScaleDelay = 0;
        public float _animScaleDuration = 0.1f;
        public Ease _animScaleEase = Ease.Linear;
        
        [Header("Scale Animation")] 
        public float _animMoveFrom = -.5f;
        public float _animMoveDelay = 0;
        public float _animMoveDuration = 0.1f;
        public Ease _animMoveEase = Ease.Linear;

        [ContextMenu("Animate")]
        public void DebugAnimate() => Animate();
        
        public void Animate(float delay = 0)
        {
            _animatedParent.DOScale(1f, _animScaleDuration).From(_animScaleFrom).SetEase(_animScaleEase).SetDelay(delay * _animScaleDelay);
            _animatedParent.DOLocalMoveY(0, _animMoveDuration).From(_animMoveFrom).SetEase(_animMoveEase).SetDelay(delay * _animMoveDelay);
        }
    }
}