using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("HERO Units")]
    [SerializeField] private List<HeroUnit> _heroePrefabs = new List<HeroUnit>(); //LIST of all heroes

    [Header("ENEMY Units")]
    [SerializeField] private List<EnemyUnit> _enemyPrefabs = new List<EnemyUnit>();


    //LIST of characters in game
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();
    private List<HeroUnit> _heroUnits = new List<HeroUnit>();


    void Awake() => Instance = this; 

    public void PopulateUnitLists()
    {
        _enemyUnits = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None).ToList();
        _heroUnits = FindObjectsByType<HeroUnit>(FindObjectsSortMode.None).ToList();
    }

    public List<HeroUnit> GetHeroList() => _heroUnits;
    public List<EnemyUnit> GetEnemyList() => _enemyUnits;


    public void ChooseTileForSpawnUnits() //Select the tile to spawn the units
    {
        if (GameManager.Instance.GameState == GameState.PlayerSpawn)
        {
            for (int i = 0; i < _heroePrefabs.Count; i++)
            {

                NodeBase playerNodeBase = GridManager.Instance.TileForTeams();
                Instantiate(_heroePrefabs[i], playerNodeBase.Coords.Pos, _heroePrefabs[i].transform.rotation);
                GridManager.Instance.UpdateTiles();
            }
        }

        if (GameManager.Instance.GameState == GameState.EnemySpawn)
        {
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                NodeBase enemyNodeBase = GridManager.Instance.TileForTeams();
                Instantiate(_enemyPrefabs[i], enemyNodeBase.Coords.Pos, _enemyPrefabs[i].transform.rotation);
                GridManager.Instance.UpdateTiles();
            }
        }
    }

    public void ResetMovementOfUnits()
    {
        List<HeroUnit> allHeroes = FindObjectsByType<HeroUnit>(FindObjectsSortMode.None).ToList();
        List<EnemyUnit> allEnemies = FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None).ToList();

        if (GameManager.Instance.GameState == GameState.PlayerTurn)
        {
            foreach (var unit in allHeroes)
                unit.RestartMovement();
        }
        else if (GameManager.Instance.GameState == GameState.EnemyTurn)
        {
            foreach (var unit in allEnemies)
                unit.RestartMovement();
        }
    }

}
