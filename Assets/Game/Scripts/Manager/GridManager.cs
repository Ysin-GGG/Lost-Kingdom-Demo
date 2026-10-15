using LostKingdom.Content;
using LostKingdom.Utility;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using Character = LostKingdom.Content.Character;

namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public sealed class GridManager : LKBehaviour
    {
        public const int GridSize = 64;

        private static Vector2Int offset;
        private static Lattice moving;
        private static Lattice range;
        private static Lattice area;
        private static GridManager instance;
        private HashSet<Vector2Int> map;
        private Vector2 selectPosition;
        private int ap;

        [SerializeField]
        private Tilemap selectLayer;
        [SerializeField]
        private Tilemap movingLayer;
        [SerializeField]
        private Tilemap routeLayer;
        [SerializeField]
        private Tilemap rangeLayer;
        [SerializeField]
        private Tilemap areaLayer;
        [SerializeField]
        private Tilemap rangeLayerSwap;
        [SerializeField]
        private Tilemap areaLayerSwap;
        [SerializeField]
        private RuleTile movingTile;
        [SerializeField]
        private RuleTile routeTile;
        [SerializeField]
        private RuleTile rangeTile;
        [SerializeField]
        private RuleTile areaTile;

        public static Lattice Range { get => GridManager.range; set => GridManager.instance.SetRange(value); }
        public static Lattice Area { get => GridManager.area; set => GridManager.instance.SetArea(value); }
        public static bool ShowSelectBox { get => GridManager.instance.selectLayer.GetActive(); set => GridManager.instance.selectLayer.SetActive(value); }
        public static bool ShowRangeMap { get => GridManager.instance.rangeLayer.GetActive(); set => GridManager.instance.rangeLayer.SetActive(value); }
        public static bool ShowAreaMap { get => GridManager.instance.areaLayer.GetActive(); set => GridManager.instance.areaLayer.SetActive(value); }
        public static bool ShowMovingMap { get => GridManager.instance.movingLayer.GetActive(); set => GridManager.instance.movingLayer.SetActive(value); }
        public static bool ShowRouteMap { get => GridManager.instance.routeLayer.GetActive(); set => GridManager.instance.routeLayer.SetActive(value); }
        public static int OffsetX { get => GridManager.offset.x; set => GridManager.offset.x = value; }
        public static int OffsetY { get => GridManager.offset.y; set => GridManager.offset.y = value; }
        public static Vector2Int Offset { get => GridManager.offset; set => GridManager.offset = value; }
        public static Vector2Int MouseGridWorld => LKMath.Floor(GridManager.instance.selectPosition);

        protected override void Awake()
        {
            GridManager.instance = this;
            GridManager.ShowSelectBox = true;
            GridManager.moving = Lattice.Empty;
            GridManager.range = Lattice.Empty;
            GridManager.area = Lattice.Empty;
            this.map = new HashSet<Vector2Int>();
        }

        public static void SwitchMode()
        {
            GameManager.MovingMode = !GameManager.MovingMode;
            GameManager.SelectedSymbol = null;
            UIManager.Game_HoverText.Active = false;
            UIManager.HUD_APList.RedCount = 0;
            GridManager.instance.ap = 0;
            GridManager.instance.rangeLayer.ClearAllTiles();
            GridManager.instance.areaLayer.ClearAllTiles();
            GridManager.instance.movingLayer.ClearAllTiles();
            GridManager.instance.routeLayer.ClearAllTiles();
        }

        protected override void Update()
        {
            this.selectPosition = LKMath.Floor((LK.MouseScreen - LK.ScreenSize / 2) / GridManager.GridSize + (Vector2)CameraManager.GridCamera.transform.position);
            this.selectLayer.transform.position = LKMath.Lerp(Options.SelectBoxSmoothness, this.selectLayer.transform.position, this.selectPosition + Vector2.one * .5f);
            this.selectLayer.transform.localScale = Vector3.one + Vector3.one * (LK.GetFloatingSin() * .25f);
            if (GameManager.InitiativeEntity is Character character)
            {
                Vector2Int oldMouse = GridManager.MouseGridWorld - character.Position;
                Vector2Int mouse = GridManager.MouseGridWorld - character.Position;
                Vector2Int origMouse = mouse - character.Position;
                if (GameManager.MovingMode)
                {
                    GridManager.ShowRangeMap = false;
                    GridManager.ShowAreaMap = false;
                    GridManager.ShowMovingMap = true;
                    //GridManager.ShowRouteMap = true;
                    GridManager.range = Lattice.Empty;
                    GridManager.area = Lattice.Empty;
                    if (this.ap != character.AP)
                    {
                        this.ap = character.AP;
                        this.SetMoving(this.ap * 3);
                    }
                    GridManager.ShowRouteMap = GridManager.moving[mouse];
                    if (!GridManager.ShowRouteMap)
                    {
                        if (GridManager.moving[oldMouse])
                        {
                            UIManager.IsHoverTextShowing = false;
                            UIManager.Game_HoverText.Active = false;
                            UIManager.HUD_APList.RedCount = 0;
                        }
                    }
                    else
                    {
                        this.SetRoute(mouse);
                        int dist1 = mouse.x.Abs() + mouse.y.Abs();
                        int dist2 = LKMath.Ceiling(dist1 / 3.0);
                        UIManager.HUD_APList.RedCount = dist2;
                        if (!this.IsPointerOverGameObject(LK.MouseScreen))
                        {
                            UIManager.Game_HoverText.SetText($"{dist1}格（-{dist2} AP）");
                            UIManager.IsHoverTextShowing = true;
                        }
                    }
                }
                else
                {
                    GridManager.ShowRangeMap = true;
                    //GridManager.ShowAreaMap = true;
                    GridManager.ShowMovingMap = false;
                    GridManager.ShowRouteMap = false;
                    GridManager.ShowAreaMap = GridManager.range[mouse];
                    GridManager.moving = Lattice.Empty;
                    this.rangeLayer.transform.position = (Vector2)character.Position;
                    this.areaLayer.transform.position = (Vector2)GridManager.MouseGridWorld;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (GridManager.ShowAreaMap)
                    {
                        character.EmploySymbol(GameManager.SelectedSymbol);
                    }
                    if (GridManager.ShowRouteMap)
                    {
                        int cost = LKMath.Ceiling((mouse.x.Abs() + mouse.y.Abs()) / 3.0);
                        character.AP -= cost;
                        character.Position = GridManager.MouseGridWorld;
                        this.movingLayer.transform.position = (Vector2)character.Position;
                        this.routeLayer.transform.position = (Vector2)character.Position;
                    }
                }
            }
            else
            {
                GridManager.ShowRangeMap = false;
                GridManager.ShowAreaMap = false;
                GridManager.ShowMovingMap = false;
                GridManager.ShowRouteMap = false;
            }
        }

        /// <summary> 获取可移动区域 </summary>
        /// <param name="length"> 最大距离 </param>
        private void SetMoving(int length)
        {
            this.map.Clear();
            this.movingLayer.ClearAllTiles();
            if (length <= 0) return;
            GridManager.moving = new Lattice(new HashSet<Vector2Int>());
            Vector3Int position = Vector3Int.zero;
            Vector2Int data = Vector2Int.zero;
            Vector2Int source = (Vector2Int)this.movingLayer.transform.position.Ceiling();
            int[] dir = { 0, 1, 0, -1, 0 };
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            visited.Add(data);
            queue.Enqueue(position);
            while (queue.Count > 0)
            {
                position = queue.Dequeue();
                data = (Vector2Int)position;
                GridManager.moving[data] = true;
                this.map.Add(data);
                this.movingLayer.SetTile((Vector3Int)data, this.movingTile);
                for (int i = 0; i < 4; i--)
                {
                    Vector3Int next3i = position + new Vector3Int(dir[i++], dir[i++], 1);
                    Vector2Int next2i = (Vector2Int)next3i;
                    if (!(next3i.z > length || visited.Contains(next2i) || FieldManager.IsBarrier(next2i + source))) queue.Enqueue(next3i);
                    visited.Add(next2i);
                }
            }
        }

        private void SetRoute(Vector2Int position)
        {
            Vector2Int[] route = GridManager.GetRoute(map, Vector2Int.zero, position);
            this.routeLayer.ClearAllTiles();
            if (route != null)
                foreach (Vector2Int pos in route)
                {
                    this.map.Add(pos);
                    this.routeLayer.SetTile((Vector3Int)pos, this.routeTile);
                }
        }

        private void SetRange(Lattice lattice)
        {
            //if (GridManager.range.Equals(lattice)) return;
            GridManager.range = lattice;
            this.rangeLayer.ClearAllTiles();
            foreach (Vector2Int pos in lattice)
                this.rangeLayer.SetTile((Vector3Int)pos, this.rangeTile);
        }

        private void SetArea(Lattice lattice)
        {
            //if (GridManager.area.Equals(lattice)) return;
            GridManager.area = lattice;
            this.areaLayer.ClearAllTiles();
            foreach (Vector2Int pos in lattice)
                this.areaLayer.SetTile((Vector3Int)pos, this.areaTile);
        }

        /// <summary> 获取最短路径 </summary>
        /// <param name="map"> 可行动区域 </param>
        /// <param name="source"> 起点坐标 </param>
        /// <param name="dest"> 终点坐标 </param>
        /// <returns> 路径数组，若无法到达则返回null </returns>
        private static Vector2Int[] GetRoute(HashSet<Vector2Int> map, Vector2Int source, Vector2Int dest)
        {
            // A*
            if (source == dest) return new Vector2Int[1] { dest };
            int[] dir = new int[5] { 0, 1, 0, -1, 0 };
            Dictionary<Vector2Int, Vector2Int> visited = new Dictionary<Vector2Int, Vector2Int>();
            LinkedList<Vector3Int> order = new LinkedList<Vector3Int>();
            order.AddFirst(new Vector3Int(source.x, source.y, (dest.x - source.x).Abs() + (dest.y - source.y).Abs()));
            while (order.Count > 0)
            {
                Vector3Int pos = order.First.Value;
                order.RemoveFirst();
                if (pos.x == dest.x && pos.y == dest.y)
                {
                    List<Vector2Int> route = new List<Vector2Int>();
                    route.Add(source);
                    void Backtrack(Vector2Int curr)
                    {
                        if (curr == source) return;
                        else Backtrack(visited[curr]);
                        route.Add(curr);
                    }
                    Backtrack(dest);
                    return route.ToArray();
                }
                for (int i = 0; i < 4; i--)
                {
                    Vector2Int step = new Vector2Int(pos.x + dir[i++], pos.y + dir[i++]);
                    if (!visited.ContainsKey(step) && map.Contains(step))
                    {
                        visited.Add(step, (Vector2Int)pos);
                        Vector3Int data = new Vector3Int(step.x, step.y, (dest.x - step.x).Abs() + (dest.y - step.y).Abs() + pos.z + 1);
                        LinkedListNode<Vector3Int> curr = order.First;
                        while (curr != null) 
                            if (data.z > curr.Value.z) curr = curr.Next;
                            else break;
                        if (curr == null) order.AddLast(data);
                        else order.AddBefore(curr, data);
                    }
                }
            }
            return null;
            /* BFS
            if (source == dest) return new Vector2Int[1] { dest };
                Dictionary<Vector2Int, Vector2Int> visited = new Dictionary<Vector2Int, Vector2Int>();
                Queue<Vector2Int>[] queue = { new Queue<Vector2Int>(), new Queue<Vector2Int>() };
                Vector2Int position = source;
                queue[0].Enqueue(position);
                for (int j = 0, k = 1; queue[j].Count > 0 || queue[k].Count > 0; k = j, j = (j + 1) % 2) // bfs→a*
                {
                    while (queue[j].Count > 0)
                    {
                        position = queue[j].Dequeue();
                        if (position == dest)
                        {
                            List<Vector2Int> route = new List<Vector2Int>();
                            route.Add(source);
                            void Backtrack(Vector2Int curr)
                            {
                                if (curr == source) return;
                                else Backtrack(visited[curr]);
                                route.Add(curr);
                            }
                            Backtrack(dest);
                            return route.ToArray();
                        }
                        int[] dir = { 0, 1, 0, -1, 0 };
                        for (int i = 0; i < 4; i--)
                        {
                            Vector2Int next = position + new Vector2Int(dir[i++], dir[i++]);
                            if (map.Contains(next) && !visited.ContainsKey(next))
                            {
                                queue[k].Enqueue(next);
                                visited.Add(next, position);
                            }
                        }
                    }
                }
                return null;
            */
        }

        /// <summary> 获取攻击范围内的所有实体 </summary>
        /// <returns> 实体数组，若空则返回长度为0的数组 </returns>
        public static Entity[] GetAreaEntity()
        {
            Enemy[] enemies = GameManager.Enemies;
            Character[] characters = GameManager.Characters;
            List<Entity> entities = new List<Entity>();
            foreach (Enemy enemy in enemies) if (GridManager.area[enemy.Position - GridManager.MouseGridWorld]) entities.Add(enemy);
            foreach (Character character in characters) if (GridManager.area[character.Position - GridManager.MouseGridWorld]) entities.Add(character);
            return entities.ToArray();
        }

        private bool IsPointerOverGameObject(Vector2 mousePosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults.Count > 0;
        }
    }
}