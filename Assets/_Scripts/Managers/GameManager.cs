using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() => Instance = this;

    private bool _heroesWin = true;

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
                Debug.Log("---Menu Options");
                break;
            case GameState.GameStart:
                Debug.Log("---Inizia GeneratedGrid!");
                GridManager.Instance.GeneratedGrid();
                GridManager.Instance.CreateMapGame();
                StartCoroutine(CanvasManager.Instance.GameMessageStartOrEnd("Start"));
                break;
            case GameState.PlayerSpawn:
                Debug.Log("---Inizia SpawnHeroes!");
                GridManager.Instance.SpawnUnitsForGame();
                SpawnManager.Instance.PopulateUnitLists();
                ChangeState(GameState.EnemySpawn);
                Debug.Log($"{SpawnManager.Instance.GetHeroList().Count} Spawned!"); 
                break;
            case GameState.EnemySpawn:
                Debug.Log("---Inizia SpawnEnemies!");
                GridManager.Instance.SpawnUnitsForGame();
                SpawnManager.Instance.PopulateUnitLists();
                ChangeState(GameState.PlayerTurn);
                break;
            case GameState.PlayerTurn:
                Debug.Log("--------------------PLAYER TURN!--------------------");
                SpawnManager.Instance.PopulateUnitLists();
                GridManager.Instance.UpdateTiles();
                SpawnManager.Instance.ResetMovementOfUnits();
                CanvasManager.Instance.ShowActiveTurnPanel();
                BattleManager.Instance.BattleWinnerCalculator();
                break;
            case GameState.EnemyTurn:
                Debug.Log("--------------------ENEMY TURN!--------------------");
                SpawnManager.Instance.PopulateUnitLists();
                GridManager.Instance.UpdateTiles();
                EnemyManager.Instance.BeginEnemyTurns();
                SpawnManager.Instance.ResetMovementOfUnits();
                CanvasManager.Instance.ShowActiveTurnPanel();
                BattleManager.Instance.BattleWinnerCalculator();
                break;

            default:
                Debug.LogError("Unhandled GameState: " + newState);
                break;
        }
    }

    public bool BattleResult(bool result) => _heroesWin = result;
    public bool BattleVictory() => _heroesWin;
    public void EndTurn() => ChangeState(GameState.EnemyTurn);
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
