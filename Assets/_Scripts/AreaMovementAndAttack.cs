using System.Collections.Generic;
using _Scripts.Tiles;
using UnityEngine;

public static class AreaMovementAndAttack
{
    private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);

    public static List<NodeBase> _tempNodes = new List<NodeBase>();

    public static List<NodeBase> FloodFill(NodeBase startNode, int moveRange)
    {
        var filledNodes = new List<NodeBase>();
        var toFill = new Queue<NodeBase>();

        startNode.SetG(0);

        toFill.Enqueue(startNode);
        filledNodes.Add(startNode);

        while (toFill.Count > 0)
        {
            var current = toFill.Dequeue();

            foreach (var neighbor in current.Neighbors)
            {
                if (!MouseManager.Instance.attackPhase)
                {
                    if (!filledNodes.Contains(neighbor) && IsFillable(neighbor))
                    {
                        int costToNeighbor = (int)current.G + 1;

                        if (costToNeighbor <= moveRange)
                        {
                            neighbor.SetG(costToNeighbor);
                            neighbor.VisualizeFloodFill();
                            toFill.Enqueue(neighbor);
                            filledNodes.Add(neighbor);
                        }
                    }
                }else
                {
                    if (!filledNodes.Contains(neighbor))
                    {
                        int costToNeighbor = (int)current.G + 1;

                        if (costToNeighbor <= moveRange)
                        {
                            neighbor.SetG(costToNeighbor);
                            neighbor.VisualizeFloodFill();
                            toFill.Enqueue(neighbor);
                            filledNodes.Add(neighbor);
                        }
                    }
                }
            }
        }

        foreach (var node in filledNodes) _tempNodes.Add(node);

        return filledNodes;
    }

    private static bool IsFillable(NodeBase node) => node.Walkable;

    public static void ResetFloodFill()
    {
        foreach (var node in _tempNodes) node.HideFloodFill();
        _tempNodes.Clear();
    }


}
