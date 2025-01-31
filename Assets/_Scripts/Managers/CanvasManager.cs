using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    private void Awake() => Instance = this;

    [Header("HEROE INFORMATION")]
    [SerializeField] private GameObject _heroPanel;
    [SerializeField] private TextMeshProUGUI _healthPoints;
    [SerializeField] private TextMeshProUGUI _factionName;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private TextMeshProUGUI _moveRange;

    [Header("ENEMY INFORMATION")]
    [SerializeField] private GameObject _enemyPanel;
    [SerializeField] private TextMeshProUGUI _enemyHealthPoints;
    [SerializeField] private TextMeshProUGUI _enemyFactionName;
    [SerializeField] private TextMeshProUGUI _enemyAttack;
    [SerializeField] private TextMeshProUGUI _enemyMoveRange;

    [Header("MESSAGE & TURN CONTROL PANELS")]
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private GameObject _turnPanel;
    [SerializeField] private GameObject _turnButton;
    [SerializeField] private TextMeshProUGUI _messagePanelText;
    [SerializeField] private TextMeshProUGUI _gameManagerText;


    [Header("GAME MANAGER MESSAGE")]
    [SerializeField] private GameObject _panelStartOrEnd;
    [SerializeField] private TextMeshProUGUI _startOrEndMessageText;
    [SerializeField] private TextMeshProUGUI _subtitleStartEndMessageText;


    [Header("PLAYER BUTTONS")]
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _cancelSelectionButton;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActionButton("Movement");
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActionButton("Attack");
        if (Input.GetKeyDown(KeyCode.Escape)) ActionButton("CancelSelection");
    }

    public void SetActiveHeroPanel() => _heroPanel.gameObject.SetActive(!_heroPanel.gameObject.activeSelf);
    public void SetActiveEnemyPanel() => _enemyPanel.gameObject.SetActive(!_enemyPanel.gameObject.activeSelf);
    public bool EnemyPanelIsActive() => _enemyPanel.gameObject.activeSelf;

    public void SetActiveMessagePanel() => _messagePanel.gameObject.SetActive(!_messagePanel.gameObject.activeSelf);

    public void SetActiveTurnPanel() => _turnPanel.gameObject.SetActive(!_turnPanel.gameObject.activeSelf);

    public void ActionButton(string action) => MouseManager.Instance.ActiveFloodFill(action);

    public void ShowMessageInPanel(string message)
    {
        SetActiveMessagePanel();
        _messagePanelText.text = message;
        Invoke("SetActiveMessagePanel", 1.5f);
    }

    public void UpgradePanelHeroInfo(HeroUnit currentHero)
    {
        _factionName.text = currentHero.FactionAndName();
        _healthPoints.text = $"Health: {currentHero.CurrentHealth()}/{currentHero.MaxHealth()}";
        _attack.text = $"Attack range: {currentHero.MaxAttack()}";
        _moveRange.text = $"Movement: {currentHero.CurrentMovement()}/{currentHero.MaxMovement()}";
    }
    public void UpgradePanelEnemyInfo(EnemyUnit currentEnemy)
    {
        _enemyFactionName.text = currentEnemy.FactionAndName();
        _enemyHealthPoints.text = $"Health: {currentEnemy.CurrentHealth()}/{currentEnemy.MaxHealth()}";
        _enemyAttack.text = $"Attack range: {currentEnemy.MaxAttack()}";
        _enemyMoveRange.text = $"Movement: {currentEnemy.MaxMovement()}";
    }

    public void ShowActiveTurnPanel()
    {
        SetActiveTurnPanel();
        _gameManagerText.text = GameManager.Instance.GameState.ToString();
        _turnButton.gameObject.SetActive(!_turnButton.gameObject.activeSelf);
        Invoke("SetActiveTurnPanel", 1.2f);
        
        if(MouseManager.Instance.GetWorkgingNode() != null)
            MouseManager.Instance.GetWorkgingNode().UnitDeselectedInNodeBase();
    }

    public IEnumerator GameMessageStartOrEnd(string message)
    {
        switch (message)
        {
            case "Start":
            _panelStartOrEnd.gameObject.SetActive(true);
            _startOrEndMessageText.text = "START BATTLE!";
            yield return new WaitForSeconds(3f);
            _panelStartOrEnd.gameObject.SetActive(false);
            ShowActiveTurnPanel();
            break;
            
            case "End":
            _panelStartOrEnd.gameObject.SetActive(true);
            _startOrEndMessageText.text = "BATTLE ENDED!";
            Debug.Log($"BATTLE ENDED!");
            if (!GameManager.Instance.BattleVictory())
                _subtitleStartEndMessageText.text = "Heroes victory!";
            else
                _subtitleStartEndMessageText.text = "Evils victory!";

            yield return new WaitForSeconds(1f);
            Time.timeScale = 0;
            break;

            default:
            break;
        }
    }

}
