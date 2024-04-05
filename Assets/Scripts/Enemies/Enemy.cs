using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Enemy : MonoBehaviour
    {
        protected Transform Player => GameManager.Instance.Player.transform;
    }
}