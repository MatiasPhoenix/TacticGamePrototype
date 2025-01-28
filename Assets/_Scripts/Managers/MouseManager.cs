using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using Tarodev_Pathfinding._Scripts;
using Tarodev_Pathfinding._Scripts.Grid;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    public HeroUnit HeroUnit;
    private NodeBase _workingNode;
    public bool unitMoving = false;
    
    private List<NodeBase> _path = new List<NodeBase>();

    private void Awake() => Instance = this;

    public void MouseInteraction(NodeBase node, HeroUnit heroUnit)
    {
        unitMoving = true;
        _workingNode = node;
        SetHeroUnit(heroUnit);
        GridManager.Instance.UnitPresentInTile(node);
    
        AreaMovementAndAttack.FloodFill(node, HeroUnit.CurrentMovement());
        CanvasManager.Instance.UpgradePanelHeroInfo(HeroUnit);

    }

    public void SetHeroUnit(HeroUnit unit)
    {
        string targetFactionAndName = unit.FactionAndName();

        HeroUnit[] allHeroes = FindObjectsByType<HeroUnit>(FindObjectsSortMode.None);

        HeroUnit foundHero = allHeroes.FirstOrDefault(hero => hero.FactionAndName() == targetFactionAndName);

        if (foundHero == null) { Debug.LogError("Eroe non trovato"); return; }

        HeroUnit = foundHero;
        Debug.Log($"Eroe selezionato: {HeroUnit}");

    }

    public void MethodToMoveUnit() => StartCoroutine(MoveHero()); 
    
    public IEnumerator MoveHero()
    {
        AreaMovementAndAttack.ResetFloodFill();
        GridManager.Instance.UnitDeselected();
        _workingNode.RevertTile();
        _path.Reverse();
        unitMoving = false;
        foreach (var tile in _path)
        {
            Vector2 pos = tile.Coords.Pos;
            yield return new WaitForSeconds(0.3f);
            HeroUnit.transform.position = pos;
            tile.RevertTile();
            HeroUnit.MovementModifier(false);
        }
        HeroUnit = null;
        _workingNode = null;
        Pathfinding.TileCount = 0;   
    }
    public void CancelSelectedUnit()
    {
        HeroUnit = null;
        _workingNode = null;
        Pathfinding.TileCount = 0;
        foreach (var tile in _path)
        {
            tile.RevertTile();
        }   
    }

    public void TakePathToPathfinder(List<NodeBase> path) => _path = path;
    
}
