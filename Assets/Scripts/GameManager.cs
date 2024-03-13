using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public JuiceConfigSO JuiceConfig;

    private void Awake()
    {
        //Quick "singleton"
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }
}
