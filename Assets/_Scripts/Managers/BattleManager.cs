using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private void Awake() => Instance = this;

    public IEnumerator StartBattle(BaseUnit attacker, BaseUnit target)
    {
        AnimationManager.Instance.PlayAttackAnimation(attacker);
        yield return new WaitForSeconds(0.2f);
        DamageCalculator(attacker, target);

    }

    void DamageCalculator(BaseUnit attacker, BaseUnit target)
    {
        Debug.Log($"{target.FactionAndName()} prende danno da {attacker.FactionAndName()}");
        target.HealthModifier(false);

        if (GameManager.Instance.GameState == GameState.PlayerTurn)
            CanvasManager.Instance.UpgradePanelEnemyInfo((EnemyUnit)target);

        if (target.CurrentHealth() <= 0)
            StartCoroutine(CharacterDied(target));
        else if (target.CurrentHealth() > 0)
            AnimationManager.Instance.TakeDamageAnimation(target);

    }

    public IEnumerator CharacterDied(BaseUnit unit)
    {
        AnimationManager.Instance.DeadAnimation(unit);
        yield return new WaitForSeconds(0.7f);
        AnimationManager.Instance.CharacterIsDead(unit);
        Destroy(unit.gameObject);
    }

    public void BattleWinnerCalculator()
    {
        if (SpawnManager.Instance.GetHeroList().Count == 0)
        {
            GameManager.Instance.BattleResult(true);
            StartCoroutine(CanvasManager.Instance.GameMessageStartOrEnd("End"));
        }else if (SpawnManager.Instance.GetEnemyList().Count == 0)
        {
            GameManager.Instance.BattleResult(false);
            StartCoroutine(CanvasManager.Instance.GameMessageStartOrEnd("End"));
        }
    }
}
