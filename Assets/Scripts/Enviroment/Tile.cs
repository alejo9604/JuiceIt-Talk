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

        private void OnDisable()
        {
            DOTween.Kill(gameObject);
            _animatedParent.localPosition = Vector3.zero;
            _animatedParent.localScale = Vector3.one;
        }

        public void SetData(int q, int r, int x, int y, Sprite sprite, Color color)
        {
            Axial_Coord = new Vector2Int(q, r);
            OddR_Coord = new Vector2Int(x, y);
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = color;
        }

        public void SetSelected(bool selected) => _selected.SetActive(selected);

        public TextMeshPro debugText;
        
        public void PlayStartAnimation(TileSpawnTuning tuning, float delayMultiplier = 0, float extraDelay = 0)
        {
            DOTween.Kill(gameObject);
            if (tuning.UseScale)
            {
                _animatedParent.DOScale(1f, tuning.ScaleDuration)
                    .From(tuning.ScaleFrom)
                    .SetEase(tuning.ScaleEase)
                    .SetDelay(delayMultiplier * tuning.ScaleDelayPerDistance + extraDelay)
                    .SetId(gameObject);
                
            }

            if (tuning.UseMove)
            {
                _animatedParent.DOLocalMoveY(0, tuning.MoveDuration)
                    .From(tuning.MoveFrom)
                    .SetEase(tuning.MoveEase)
                    .SetDelay(delayMultiplier * tuning.MoveDelayPerDistance + extraDelay)
                    .SetId(gameObject);
            }
        }
    }
}