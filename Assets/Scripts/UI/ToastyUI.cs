using UnityEngine;
using DG.Tweening;

namespace AllieJoe.JuiceIt
{
    public class ToastyUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _toasty;


        private void Start()
        {
            _toasty.gameObject.SetActive(false);

            GameManager.Instance.GameDelegates.OnEnemyDeath += Play;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnEnemyDeath -= Play;
        }

        private void Play()
        {
            if(!GameManager.Instance.GetConfigValue(EConfigKey.Toasty))
                return;
            
            _toasty.gameObject.SetActive(true);
            float width = _toasty.rect.width;
            
            DOTween.Kill(gameObject);
            _toasty.DOAnchorPosX(0, 0.1f).SetEase(Ease.OutCubic).SetId(gameObject).OnComplete(() =>
            {
                _toasty.DOAnchorPosX(width + 100, 0.35f).SetEase(Ease.InCubic).SetDelay(0.2f).SetId(gameObject);
            });
            
            AudioManager.Instance.PlaySound(AudioLibrary.TOASTY);
        }
    }
}
