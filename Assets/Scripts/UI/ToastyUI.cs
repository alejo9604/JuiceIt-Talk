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

            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdate;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
            GameManager.Instance.GameDelegates.OnEnemyDeath += Play;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdate;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshConfig;
            GameManager.Instance.GameDelegates.OnEnemyDeath -= Play;
        }

        private void Play()
        {
            if(!_toasty.gameObject.activeSelf)
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
        
        private void OnConfigUpdate(EConfigKey key)
        {
            if(key is EConfigKey.Toasty)
                RefreshConfig();
        }
        
        private void RefreshConfig()
        {
            _toasty.gameObject.SetActive(GameManager.Instance.GetConfigValue(EConfigKey.Monster));
        }
    }
}
