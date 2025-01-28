using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "--MY Scriptable Unit--")]
public class ScriptableUnit : ScriptableObject
{
    [Header("Faction Selection")]
    public Faction Faction;
    public BaseUnit UnitPrefab;

}


public enum Faction
{
    Hero = 1,
    Enemy = 2,
    Neutral = 3
}
