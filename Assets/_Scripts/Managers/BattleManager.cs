using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private void Awake() => Instance = this;

    public IEnumerator StartBattle(BaseUnit attacker, BaseUnit target)
    {
        AnimationManager.Instance.PlayAttackAnimation(attacker);
        yield return new WaitForSeconds(0.4f);
        AnimationManager.Instance.TakeDamageAnimation(target);

        DamageCalculator(attacker,target);
    }

    void DamageCalculator(BaseUnit attacker, BaseUnit target)
    {
        Debug.Log($"{target.FactionAndName()} prende danno da {attacker.FactionAndName()}");
        target.HealthModifier(false);

        if (target.CurrentHealth() <= 0)
            StartCoroutine(CharacterDied(target));
    }

    public IEnumerator CharacterDied(BaseUnit unit)
    {
        
        AnimationManager.Instance.DeadAnimation(unit);
        yield return new WaitForSeconds(1f);
        AnimationManager.Instance.CharacterIsDead(unit);
        Destroy(unit.gameObject);
    }
}
