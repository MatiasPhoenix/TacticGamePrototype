using _Scripts.Tiles;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [Header("Unit Faction & Name")]
    [SerializeField] protected Faction _faction;
    [SerializeField] protected string _unitName;
    
    [Header("Unit Attributes")]
    [SerializeField] protected int _maxHealth;
    protected int _currentHealth;
    [SerializeField] protected int _maxMovement;
    protected int _currentMovement;
    [SerializeField] protected int _maxAttack;
    protected int _currentAttack;
    protected bool _isDead = false;

    private void Awake() => RestartAllStats();

    public string FactionAndName() => _unitName;

    //------Health hero
    public int MaxHealth() => _maxHealth;
    public int CurrentHealth() => _currentHealth;
    public void RestartHealth() => _currentHealth = _maxHealth;
    public int HealthModifier(bool modifier) => modifier ? _currentHealth++ : _currentHealth--;

    //------Movement hero
    public int MaxMovement() => _maxMovement;
    public int CurrentMovement() => _currentMovement;
    public void RestartMovement() => _currentMovement = _maxMovement;
    public int MovementModifier(bool modifier) => modifier ? _maxMovement++ : _currentMovement--;

    //------Attack hero
    public int MaxAttack() => _maxAttack;
    public void RestartAttack() => _currentAttack = _maxAttack;

    //------Death hero
    public bool IsDead() => _isDead;
    public void RestartAllStats()
    {
        RestartHealth();
        RestartMovement();
        RestartAttack();
    }
    
}
