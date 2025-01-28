using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private void Awake() => Instance = this;

    public IEnumerator StartBattle(BaseUnit attacker, BaseUnit target)
    {
        Debug.Log($"{attacker.FactionAndName()} attacca {target.FactionAndName()}");

        AnimationManager.Instance.PlayAttackAnimation(attacker);
        yield return new WaitForSeconds(0.5f);
        AnimationManager.Instance.TakeDamageAnimation(target);   
    }

    
}
