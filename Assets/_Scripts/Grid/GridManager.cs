using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using NUnit.Framework;
using Tarodev_Pathfinding._Scripts.Grid.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tarodev_Pathfinding._Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;

        [Header("Grid Manager")]
        [SerializeField] private ScriptableGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;
        [SerializeField] private int _gridWidth;
        [SerializeField] private int _gridHeight;


        public Dictionary<Vector2, NodeBase> Tiles { get; private set; }
        private NodeBase _playerNodeBase;
        private NodeBase _goalNodeBase;


        void Awake() => Instance = this;


        private void OnDestroy() => NodeBase.OnHoverTile -= OnTileHover;

        private void OnTileHover(NodeBase nodeBase)
        {
            _goalNodeBase = nodeBase;
            _goalNodeBase.transform.position = _goalNodeBase.Coords.Pos;

            foreach (var t in Tiles.Values) t.RevertTile();

            var path = Pathfinding.FindPath(_playerNodeBase, _goalNodeBase);
        }
        public void GeneratedGrid()
        {
            Tiles = _scriptableGrid.GenerateGrid();
            foreach (var tile in Tiles.Values) tile.CacheNeighbors();
            NodeBase.OnHoverTile += OnTileHover;

            GameManager.Instance.ChangeState(GameState.PlayerSpawn);
        }

        public void SpawnUnitsForGame()
        {
            SpawnManager.Instance.ChooseTileForSpawnUnits();
        }

        public void CreateMapGame()
        {
            var myGridMap = _scriptableGrid as ScriptableSquareGrid;
            myGridMap.ChangeMapDimensions(_gridWidth, _gridHeight);
        }

        
        public NodeBase TileForTeams()
        {
            if (GameManager.Instance.GameState == GameState.PlayerSpawn)
            {
                NodeBase nodeForHero = Tiles.Where(t => t.Key.x < _gridWidth / 4 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
                return nodeForHero;
            } else if (GameManager.Instance.GameState == GameState.EnemySpawn)
            {
                NodeBase nodeForEnemy = Tiles.Where(t => t.Key.x > _gridWidth / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
                return nodeForEnemy;
            }else
            {
                return null;
            }
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => Tiles.TryGetValue(pos, out var tile) ? tile : null;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_drawConnections) return;
            Gizmos.color = Color.red;
            foreach (var tile in Tiles)
            {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
            }
        }
        public void UnitPresentInTile(NodeBase nodeSelected)
        {
            _playerNodeBase = nodeSelected;
        }
        public void UnitDeselected()
        {
            _playerNodeBase = null;
        }

        public bool UnitSelect()
        {
            return _playerNodeBase!= null;
        }
    }
}