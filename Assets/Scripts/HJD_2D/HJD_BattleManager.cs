using UnityEngine;
using UnityEngine.UIElements;

public class HJD_BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    public static HJD_BattleManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;

    }
    public void Attack(Transform transform_Arrow) 
    {
        Instantiate(_bullet, transform_Arrow.position, transform_Arrow.rotation);
    }
}
