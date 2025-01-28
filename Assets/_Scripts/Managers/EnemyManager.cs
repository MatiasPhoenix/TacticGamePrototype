using System;
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
    private bool unitMoving = false;

    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();
    private List<HeroUnit> _heroTargetUnits = new List<HeroUnit>();

    public void PopulateUnitLists()
    {
        _enemyUnits = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None).ToList();
        _heroTargetUnits = FindObjectsByType<HeroUnit>(FindObjectsSortMode.None).ToList();
    }

    public void BeginEnemyTurns()
    {
        StartCoroutine(ProcessEnemyTurns());
    }

    private IEnumerator ProcessEnemyTurns()
    {
        foreach (var enemy in _enemyUnits)
        {
            Debug.Log($"Turno del nemico: {enemy.FactionAndName()}");
            StartCoroutine(TargetHeroToCombat(enemy));
            yield return new WaitUntil(() => !unitMoving);
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    public IEnumerator TargetHeroToCombat(EnemyUnit enemy)
    {
        Dictionary<HeroUnit, List<NodeBase>> paths = new Dictionary<HeroUnit, List<NodeBase>>();
        foreach (HeroUnit hero in _heroTargetUnits)
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
            yield break; // Termina se non ci sono bersagli raggiungibili
        }

        // Ordina i percorsi per lunghezza e seleziona il bersaglio con il percorso più breve
        var shortestPath = paths.OrderBy(p => p.Value.Count).First();
        _heroTarget = shortestPath.Key;

        Debug.Log($"Bersaglio scelto: {_heroTarget.FactionAndName()} con percorso di {shortestPath.Value.Count} passi.");

        // Avvia il movimento verso il bersaglio scelto
        yield return StartCoroutine(GoToTarget(enemy));
    }



    public IEnumerator GoToTarget(EnemyUnit currentEnemy)
    {

        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, true);
        unitMoving = true;

        NodeBase targetNode = GridManager.Instance.GetTileAtPosition(_heroTarget.transform.position);
        NodeBase enemyLocation = GridManager.Instance.GetTileAtPosition(currentEnemy.transform.position);

        var path = Pathfinding.FindPath(enemyLocation, targetNode);

        if (path == null)
        {
            Debug.Log($"Nemico {currentEnemy.FactionAndName()} non può raggiungere il bersaglio.");
            GridManager.Instance.UpdateTiles();
            unitMoving = false;
            yield break;
        }
        path.Reverse();
        targetNode.RevertTile();
        foreach (var tile in path)
        {
            if (Vector2.Distance(currentEnemy.transform.position, _heroTarget.transform.position) <= 1)
            {
                Debug.Log($"{currentEnemy.FactionAndName()} ha raggiunto il bersaglio.");
                
                AnimationManager.Instance.PlayWalkAnimation(currentEnemy, false);
                StartCoroutine(BattleManager.Instance.StartBattle(currentEnemy, _heroTarget));
                GridManager.Instance.UpdateTiles();
                yield return new WaitForSeconds(0.8f);
                unitMoving = false;
                yield break;
            }

            yield return new WaitForSeconds(0.15f);
            currentEnemy.transform.position = tile.Coords.Pos;
            tile.RevertTile();
        }

        AnimationManager.Instance.PlayWalkAnimation(currentEnemy, false);
        Debug.Log($"{currentEnemy.FactionAndName()} ha terminato il movimento.");
        GridManager.Instance.UpdateTiles();
        unitMoving = false;
    }

}
