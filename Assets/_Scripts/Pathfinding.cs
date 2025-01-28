using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tarodev_Pathfinding._Scripts
{

    public static class Pathfinding
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        public static int TileCount;
        

        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            var toSearch = new List<NodeBase>() { startNode };
            var processed = new List<NodeBase>();

            while (toSearch.Any())
            {
                var current = toSearch[0];
                foreach (var t in toSearch)
                    if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                processed.Add(current);
                toSearch.Remove(current);

                // current.SetColor(ClosedColor);

                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    var count = 100;
                    while (currentPathTile != startNode)
                    {
                        if (currentPathTile.OccupateByUnit == false || currentPathTile.OccupateByEnemy == false) path.Add(currentPathTile);
                        else return null;
                                                 
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0) throw new Exception();

                    }

                    if (GameManager.Instance.GameState == GameState.PlayerTurn && MouseManager.Instance.attackPhase == false)
                        foreach (var tile in path) tile.SetColor(PathColor);
                        
                    // startNode.SetColor(PathColor);
                    // Debug.Log(path.Count);
                    TileCount = path.Count;
                    MouseManager.Instance.TakePathToPathfinder(path);
                    return path;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t)))
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch)
                        {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                            // neighbor.SetColor(OpenColor);
                        }
                    }
                }
            }
            return null;
        }
        
        

    }
}