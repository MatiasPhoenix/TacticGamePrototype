using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() => Instance = this;

    void Start()
    {
        ChangeState(GameState.GameStart);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ChangeState(GameState.PlayerTurn);

        if (Input.GetKeyDown(KeyCode.E))
            ChangeState(GameState.EnemyTurn);
    }

    [Header("Game State Manager")]
    public GameState GameState;


    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.MenuOptions:
                Debug.Log("Menu Options");
                break;
            case GameState.GameStart:
                Debug.Log("Inizia GeneratedGrid!");
                GridManager.Instance.GeneratedGrid();
                GridManager.Instance.CreateMapGame();
                break;
            case GameState.PlayerSpawn:
                Debug.Log("Inizia SpawnHeroes!");
                GridManager.Instance.SpawnUnitsForGame();
                ChangeState(GameState.EnemySpawn);
                break;
            case GameState.EnemySpawn:
                Debug.Log("Inizia SpawnEnemies!");
                GridManager.Instance.SpawnUnitsForGame();
                EnemyManager.Instance.PopulateUnitLists();
                ChangeState(GameState.PlayerTurn);
                break;
            case GameState.PlayerTurn:
                Debug.Log("--------------------PLAYER TURN!--------------------");
                GridManager.Instance.UpdateTiles();
                break;
            case GameState.EnemyTurn:
                Debug.Log("--------------------ENEMY TURN!--------------------");
                GridManager.Instance.UpdateTiles();
                EnemyManager.Instance.BeginEnemyTurns();
                break;


            default:
                Debug.LogError("Unhandled GameState: " + newState);
                break;
        }
    }
}


public enum GameState
{
    GameStart,
    MenuOptions,
    PlayerSpawn,
    EnemySpawn,
    PlayerTurn,
    EnemyTurn,
    PausaGame,

}
