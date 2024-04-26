using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class MonsterUI : MonoBehaviour
    {
        enum EState {Idle, Attacking, CloseEnemies, Recovering}

        [SerializeField] private GameObject _container;
        [SerializeField] private Animator _anim;
        
        [Space]
        [SerializeField] private PlayerShip _player;
        [SerializeField] private PointOfInterestManager _pointOfInterestManager;

        private EState _state;
        private static readonly int Attacking_AnimHash = Animator.StringToHash("Attacking");
        private static readonly int Recovering_AnimHash = Animator.StringToHash("Recovering");
        private static readonly int CloseEnemy_AnimHash = Animator.StringToHash("CloseEnemy");

        void Start()
        {
            if (_player == null)
                _player = GameManager.Instance.Player;
            
            _container.SetActive(false);
            
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdate;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdate;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshConfig;
        }

        void Update()
        {
            if(!_container.activeSelf)
                return;
            
            UpdateState();
            
            _anim.SetBool(Recovering_AnimHash, _state == EState.Recovering);
            _anim.SetBool(Attacking_AnimHash, _state == EState.Attacking);
            _anim.SetBool(CloseEnemy_AnimHash, _state == EState.CloseEnemies);
        }

        private void UpdateState()
        {
            if (_player.IsRecovering)
            {
                _state = EState.Recovering;
                return;
            }

            if (_player.IsShooting)
            {
                _state = EState.Attacking;
                return;
            }

            if (_pointOfInterestManager.HasPointsInRange)
            {
                _state = EState.CloseEnemies;
                return;
            }

            _state = EState.Idle;
        }
        
        private void OnConfigUpdate(EConfigKey key)
        {
            if(key is EConfigKey.Monster)
                RefreshConfig();
        }
        
        private void RefreshConfig()
        {
            _container.SetActive(GameManager.Instance.GetConfigValue(EConfigKey.Monster));
        }
    }
}
