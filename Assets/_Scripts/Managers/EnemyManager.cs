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
    

    public void BeginEnemyTurns() => StartCoroutine(ProcessEnemyTurns());

    private IEnumerator ProcessEnemyTurns()
    {
        foreach (var enemy in SpawnManager.Instance.GetEnemyList())
        {
            yield return new WaitUntil(() => !_enemyTurnFinished);
            Debug.Log($"Turno del nemico: {enemy.FactionAndName()}");
            StartCoroutine(TargetHeroToCombat(enemy));
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    public IEnumerator TargetHeroToCombat(EnemyUnit enemy)
    {
        Dictionary<HeroUnit, List<NodeBase>> paths = new Dictionary<HeroUnit, List<NodeBase>>();
        foreach (HeroUnit hero in SpawnManager.Instance.GetHeroList())
        {
            NodeBase enemyLocation = GridManager.Instance.GetTileAtPosition(enemy.transform.position);
            NodeBase heroLocation = GridManager.Instance.GetTileAtPosition(hero.transform.position);
            var path = Pathfinding.FindPath(enemyLocation, heroLocation);
            if (path != null)
                paths.Add(hero, path);
        }

        if (paths.Count == 0)
        {
            Debug.Log($"Nessun bersaglio raggiungibile per {enemy.FactionAndName()}");
            yield break; 
        }

        var shortestPath = paths.OrderBy(p => p.Value.Count).First();
        _heroTarget = shortestPath.Key;

        Debug.Log($"---Bersaglio scelto: {_heroTarget.FactionAndName()} con percorso di {shortestPath.Value.Count} passi.");
        yield return StartCoroutine(GoToTarget(enemy));
    }



    public IEnumerator GoToTarget(EnemyUnit currentEnemy)
    {

        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, true);
        _enemyTurnFinished = true;

        NodeBase targetNode = GridManager.Instance.GetTileAtPosition(_heroTarget.transform.position);
        NodeBase enemyLocation = GridManager.Instance.GetTileAtPosition(currentEnemy.transform.position);

        var path = Pathfinding.FindPath(enemyLocation, targetNode);

        if (path == null)
        {
            Debug.Log($"Nemico {currentEnemy.FactionAndName()} non può raggiungere il bersaglio.");
            GridManager.Instance.UpdateTiles();
            _enemyTurnFinished = false;
            yield break;
        }

        path.Reverse();
        targetNode.RevertTile();
        foreach (var tile in path)
        {
            currentEnemy.MovementModifier(false);
            if (currentEnemy.CurrentMovement() <= 0)
            {
                EnemyFinishedMovement(currentEnemy);
                _enemyTurnFinished = false;
                yield break;
            }

            if (Vector2.Distance(currentEnemy.transform.position, _heroTarget.transform.position) <= currentEnemy.MaxAttack())
            {
                EnemyFinishedMovement(currentEnemy);
                StartCoroutine(BattleManager.Instance.StartBattle(currentEnemy, _heroTarget));
                yield return new WaitForSeconds(0.8f);
                _enemyTurnFinished = false;
                yield break;
            }

            yield return new WaitForSeconds(0.15f);
            currentEnemy.transform.position = tile.Coords.Pos;
            tile.RevertTile();
        }

        EnemyFinishedMovement(currentEnemy);
        _enemyTurnFinished = false;

    }

    void EnemyFinishedMovement(EnemyUnit currentEnemy)
    {
        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, false);
        Debug.Log($"{currentEnemy.FactionAndName()} ha terminato il movimento o ha raggiunto il bersaglio.");
        GridManager.Instance.UpdateTiles();
    }

    

}
