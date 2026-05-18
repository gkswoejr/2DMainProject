using UnityEngine;


public enum EntiyAnimState
{
    None = 0,
    Idle,
    Walk,
    Attack
}
public class HJD_AnimatorCotroller : MonoBehaviour
{
    [SerializeField] private Animator Animator_Entity;

    private EntiyAnimState _currentAnimState;


    public void SetState(EntiyAnimState newState)
    {
        if (newState == EntiyAnimState.Idle && _currentAnimState == EntiyAnimState.Idle)
        {
            return;
        }

        _currentAnimState = newState;

        switch (_currentAnimState)
        {
            case EntiyAnimState.Idle:
                TAllAnimParameters();
                break;
            case EntiyAnimState.Walk:
                Animator_Entity.SetBool("IsWalk", true);
                break;
            case EntiyAnimState.Attack:
                Animator_Entity.SetTrigger("IsAttack");
                break;
         
           
            default:
                break;
        }

    }

    private void TAllAnimParameters()
    {
        Animator_Entity.SetBool("IsWalk", false);
    }

}
