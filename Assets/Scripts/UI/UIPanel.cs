using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private bool _startHide;

        public virtual void Start()
        {
            Hide(_startHide);
        }

        [ContextMenu("Toggle Hide")]
        public void ToggleHide() => Hide(_canvasGroup.interactable);
        public void Hide(bool hide)
        {
            _canvasGroup.alpha = hide ? 0 : 1;
            _canvasGroup.interactable = !hide;
            _canvasGroup.blocksRaycasts = !hide;
        }

        protected virtual void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
