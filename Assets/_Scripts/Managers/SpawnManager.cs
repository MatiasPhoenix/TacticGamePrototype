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
    [SerializeField] private bool _oneHero;

    [Header("ENEMY Units")]
    [SerializeField] private List<EnemyUnit> _enemyPrefabs = new List<EnemyUnit>();


    //LIST of characters in game
    private List<HeroUnit> _HeroUnits = new List<HeroUnit>(); //Only heores in game
    private List<EnemyUnit> _EnemyUnits = new List<EnemyUnit>();


    void Awake() => Instance = this;

    void Start()
    {
        SetListOfUnits();
    }

    public List<HeroUnit> GetHeroInGameList()
    {
        foreach (HeroUnit hero in _heroePrefabs) _HeroUnits.Add(hero);
        return _HeroUnits;
    }

    public List<EnemyUnit> GetEnemyInGameList()
    {
        foreach (EnemyUnit enemy in _enemyPrefabs) _EnemyUnits.Add(enemy);
        return _EnemyUnits;
    }

    public int NumberOFHeroes()
    {
        return _HeroUnits.Count;
    }

    public int NumberOfEnemies()
    {
        return _EnemyUnits.Count;
    }

    public bool OnlyOneHeroe()
    {
        return _oneHero;
    }

    public void SetListOfUnits()
    {
        foreach (var hero in _heroePrefabs)
        {
            _HeroUnits.Add(hero);
        }
        foreach (var enemy in _enemyPrefabs)
        {
            _EnemyUnits.Add(enemy);
        }
    }

    public void ChooseTileForSpawnUnits() //Select the tile to spawn the units
    {
        if (GameManager.Instance.GameState == GameState.PlayerSpawn)
        {
            if (!OnlyOneHeroe())
            {
                for (int i = 0; i < _HeroUnits.Count; i++)
                {
                    GridManager.Instance.UpdateTiles();

                    NodeBase playerNodeBase = GridManager.Instance.TileForTeams();
                    Instantiate(_HeroUnits[i], playerNodeBase.Coords.Pos, _HeroUnits[i].transform.rotation);
                }
            }
            else
            {
                NodeBase playerNodeBase = GridManager.Instance.TileForTeams();
                Instantiate(_HeroUnits[0], playerNodeBase.Coords.Pos, _HeroUnits[0].transform.rotation);  
            }
        }

        if (GameManager.Instance.GameState == GameState.EnemySpawn)
        {
            for (int i = 0; i < _EnemyUnits.Count; i++)
            {
                GridManager.Instance.UpdateTiles();

                NodeBase enemyNodeBase = GridManager.Instance.TileForTeams();
                Instantiate(_EnemyUnits[i], enemyNodeBase.Coords.Pos, _EnemyUnits[i].transform.rotation);
                GridManager.Instance.UpdateTiles();
            }
        }
    }

    public void ResetMovementOfHeroes()
    {
        List<HeroUnit> allHeroes = FindObjectsByType<HeroUnit>(FindObjectsSortMode.None).ToList();
        foreach (var hero in allHeroes)
        {
            hero.RestartMovement();
        }
    }

}
