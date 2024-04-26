using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PresentationUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _animParent;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private float _timeToHide = 2.5f;

        private float _hideTimer = 0;

        private void Start()
        {
            _animParent.anchoredPosition = Vector2.up * _animParent.rect.height * 2;
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdate;
        }

        private void Update()
        {
            if (_hideTimer > 0)
            {
                _hideTimer -= Time.deltaTime;
                if (_hideTimer <= 0)
                    PlayExit();
            }
        }


        private void OnConfigUpdate(EConfigKey key)
        {
            _titleText.text = GameManager.Instance.GetConfigLabel(key);
            PlayEnter();
        }


        private void PlayEnter()
        {
            float from = _animParent.rect.height;
            DOTween.Kill(gameObject);
            _animParent.DOAnchorPosY(-33, .65f).From(Vector2.up * from).SetEase(Ease.OutBounce)
                .SetUpdate(UpdateType.Normal, true).SetId(gameObject);;
            _hideTimer = _timeToHide + .65f;
        }

        private void PlayExit()
        {
            DOTween.Kill(gameObject);
            _animParent.DOAnchorPosY(_animParent.rect.height, .4f).SetEase(Ease.InBack)
                .SetUpdate(UpdateType.Normal, true).SetId(gameObject);;
        }

    }
}
