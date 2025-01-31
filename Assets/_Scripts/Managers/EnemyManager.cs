using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using Tarodev_Pathfinding._Scripts;
using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private void Awake() => Instance = this;

    private HeroUnit _heroTarget;
    private bool _enemyTurnFinished = false;
    private bool _isAttacking = false; // Flag per evitare attacchi multipli

    public void BeginEnemyTurns() => StartCoroutine(ProcessEnemyTurns());

    private IEnumerator ProcessEnemyTurns()
    {
        foreach (var enemy in SpawnManager.Instance.GetEnemyList())
        {
            _enemyTurnFinished = false;
            _isAttacking = false;

            Debug.Log($"Turno del nemico: {enemy.FactionAndName()}");
            yield return StartCoroutine(TargetHeroToCombat(enemy));

            yield return new WaitUntil(() => _enemyTurnFinished);
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    private IEnumerator TargetHeroToCombat(EnemyUnit enemy)
    {
        Dictionary<HeroUnit, List<NodeBase>> paths = new Dictionary<HeroUnit, List<NodeBase>>();
        SpawnManager.Instance.PopulateUnitLists();
        foreach (HeroUnit hero in SpawnManager.Instance.GetHeroList())
        {
            var enemyLocation = GridManager.Instance.GetTileAtPosition(enemy.transform.position);
            var heroLocation = GridManager.Instance.GetTileAtPosition(hero.transform.position);
            var path = Pathfinding.FindPath(enemyLocation, heroLocation);
            
            if (path != null)
                paths.Add(hero, path);
        }

        if (paths.Count == 0)
        {
            Debug.Log($"Nessun bersaglio raggiungibile per {enemy.FactionAndName()}");
            _enemyTurnFinished = true;
            yield break;
        }

        var shortestPath = paths.OrderBy(p => p.Value.Count).First();
        _heroTarget = shortestPath.Key;

        Debug.Log($"---Bersaglio scelto: {_heroTarget.FactionAndName()} con percorso di {shortestPath.Value.Count} passi.");
        yield return StartCoroutine(GoToTarget(enemy));
    }

    private IEnumerator GoToTarget(EnemyUnit currentEnemy)
    {
        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, true);
        _enemyTurnFinished = false;

        var targetNode = GridManager.Instance.GetTileAtPosition(_heroTarget.transform.position);
        var enemyLocation = GridManager.Instance.GetTileAtPosition(currentEnemy.transform.position);
        var path = Pathfinding.FindPath(enemyLocation, targetNode);

        if (path == null)
        {
            Debug.Log($"Nemico {currentEnemy.FactionAndName()} non può raggiungere il bersaglio.");
            GridManager.Instance.UpdateTiles();
            _enemyTurnFinished = true;
            yield break;
        }

        path.Reverse();
        targetNode.RevertTile();

        foreach (var tile in path)
        {
            if (Vector2.Distance(currentEnemy.transform.position, _heroTarget.transform.position) <= currentEnemy.MaxAttack())
            {
                yield return StartCoroutine(AttackAction(currentEnemy));
                yield break;
            }

            if (currentEnemy.CurrentMovement() <= 0)
            {
                break; // Se il nemico ha finito il movimento, esce dal ciclo
            }

            yield return new WaitForSeconds(0.15f);
            currentEnemy.transform.position = tile.Coords.Pos;
            tile.RevertTile();
            currentEnemy.MovementModifier(false);
        }

        EnemyFinishedMovement(currentEnemy);
        _enemyTurnFinished = true;
    }

    private IEnumerator AttackAction(EnemyUnit currentEnemy)
    {
        if (_isAttacking) yield break; // Evita attacchi multipli
        _isAttacking = true;

        EnemyFinishedMovement(currentEnemy);
        yield return StartCoroutine(BattleManager.Instance.StartBattle(currentEnemy, _heroTarget));

        yield return new WaitForSeconds(0.8f);
        _enemyTurnFinished = true;
    }

    private void EnemyFinishedMovement(EnemyUnit currentEnemy)
    {
        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, false);
        Debug.Log($"{currentEnemy.FactionAndName()} ha terminato il movimento o ha raggiunto il bersaglio.");
        GridManager.Instance.UpdateTiles();
    }
}
