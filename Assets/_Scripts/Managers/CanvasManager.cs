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

    [Header("MESSAGE PANELS")]
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TextMeshProUGUI _messagePanelText;
    [SerializeField] private TextMeshProUGUI _gameManagerText;

    [Header("PLAYER BUTTONS")]
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _cancelSelectionButton;


    private void Update()
    {
        _gameManagerText.text = GameManager.Instance.GameState.ToString();   

        if (Input.GetKeyDown(KeyCode.Alpha1)) ActionButton("Movement");
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActionButton("Attack");
        if (Input.GetKeyDown(KeyCode.Escape)) ActionButton("CancelSelection");
    }
    public void SetActiveHeroPanel() => _heroPanel.gameObject.SetActive(!_heroPanel.gameObject.activeSelf);

    public void SetActiveMessagePanel() => _messagePanel.gameObject.SetActive(!_messagePanel.gameObject.activeSelf);

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

}
