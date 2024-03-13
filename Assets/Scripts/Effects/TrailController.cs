using UnityEngine;

public class TrailController : MonoBehaviour
{
    [SerializeField] private GameObject[] _trails;

    private bool _trailEnabled;

    private void Start()
    {
        UpdateTrails(GameManager.Instance.JuiceConfig.TrailEnabled);
    }

    private void Update()
    {
        if(_trailEnabled == GameManager.Instance.JuiceConfig.TrailEnabled)
            return;
        
        UpdateTrails(GameManager.Instance.JuiceConfig.TrailEnabled);
    }

    private void UpdateTrails(bool enable)
    {
        _trailEnabled = enable;
        foreach (var t in _trails)
            t.SetActive(enable);
    }
}