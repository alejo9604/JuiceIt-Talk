using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class TraumaUI : UIPanel
    {
        [SerializeField] private RectTransform _traumaBar; 
        [SerializeField] private RectTransform _shakeBar; 

        void Update()
        {
            _traumaBar.localScale = new Vector3(1, GameManager.Instance.TraumaValue, 1);
            _shakeBar.localScale = new Vector3(1, GameManager.Instance.ShakeValue, 1);
        }
    }
}
