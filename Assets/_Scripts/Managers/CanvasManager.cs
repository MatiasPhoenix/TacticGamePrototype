using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    private void Awake() => Instance = this;

    [Header("HEROE INFORMATION")]
    [SerializeField] private GameObject _heroPanel;
    [SerializeField] private TextMeshProUGUI _factionName;
    [SerializeField] private TextMeshProUGUI _attack;
    [SerializeField] private TextMeshProUGUI _moveRange;

    [Header("MESSAGE PANELS")]
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TextMeshProUGUI _messagePanelText;


    public void SetActiveHeroPanel() => _heroPanel.gameObject.SetActive(!_heroPanel.gameObject.activeSelf);

    public void SetActiveMessagePanel() => _messagePanel.gameObject.SetActive(!_messagePanel.gameObject.activeSelf);

    public void ShowMessageInPanel(string message)
    {
        SetActiveMessagePanel();
        _messagePanelText.text = message;
        Invoke("SetActiveMessagePanel", 1.5f);
    }
    
    public void UpgradePanelHeroInfo(HeroUnit currentHero)
    {
        _factionName.text = currentHero.FactionAndName();
        _attack.text = $"Attack: {currentHero.MaxAttack()}";
        _moveRange.text = $"Movement: {currentHero.CurrentMovement()}";
    }


}
