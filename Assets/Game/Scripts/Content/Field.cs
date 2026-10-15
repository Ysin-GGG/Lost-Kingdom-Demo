using LostKingdom.ID;
using LostKingdom.DataStructure;
using LostKingdom.Utility;
using Unity.VisualScripting;
using LostKingdom.Manager;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor.MemoryProfiler;
using System.Linq;
using System;
using System.IO;
using TreeEditor;
using LostKingdom.Interface;
using LostKingdom.IO;
using System.Text;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.Properties;
using UnityEngine.UIElements;
using Mobsoft.PixelStyleWaterShader;
using LostKingdom.Components;

namespace LostKingdom.Content
{
    public class Field
    {
        public string Name { get; set; }

        private Dictionary<TileBase, HashSet<Vector3Int>> tilemaps;
        private HashSet<FieldTile> tileData;
        private HashSet<FieldObject> objectData;
        private HashSet<Vector2Int> barrierData;

        public FieldTile this[int x, int y]
        {
            get => this.GetTile(x, y);
        }

        public Field()
        {
            this.tilemaps = new Dictionary<TileBase, HashSet<Vector3Int>>();
            this.tileData = new HashSet<FieldTile>();
            this.objectData = new HashSet<FieldObject>();
            this.barrierData = new HashSet<Vector2Int>();
        }

        public Field(string pathFile)
        {
            this.tilemaps = new Dictionary<TileBase, HashSet<Vector3Int>>();
            this.tileData = new HashSet<FieldTile>();
            this.objectData = new HashSet<FieldObject>();
            this.barrierData = new HashSet<Vector2Int>();
            FileStream stream = new FileStream(pathFile, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            this.Read(reader);
            reader.Dispose();
            stream.Dispose();
        }

        internal Field(string name, string path, HashSet<FieldObject> objs, Tilemap ground, Tilemap water, Tilemap surface, Tilemap barrier)
        {
            this.tilemaps = new Dictionary<TileBase, HashSet<Vector3Int>>();
            this.tileData = new HashSet<FieldTile>();
            this.objectData = objs ?? new HashSet<FieldObject>();
            this.barrierData = new HashSet<Vector2Int>();
            FileStream stream = null;
            BinaryWriter writer = null;
            try
            {
                if (!name.EndsWith(".bin")) name += ".bin";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                stream = new FileStream(Path.Combine(path, name), FileMode.Create);
                writer = new BinaryWriter(stream, Encoding.UTF8);
                this.Handle(this.objectData, ground, water, surface, barrier);
                this.Write(writer);
                LKLog.LogMessage("场景文件生成完毕：" + Path.Combine(Path.GetFullPath(path), name), null, null);
            }
            catch (Exception e)
            {
                LKLog.LogError("生成场景文件时出错：" + Path.Combine(Path.GetFullPath(path), name), null, null);
                LKLog.LogError(e, null, null);
            }
            finally
            {
                writer?.Dispose();
                stream?.Dispose();
            }
        }

        public FieldTile GetTile(Vector2Int position) => this[position.x, position.y];

        public FieldTile GetTile(int x, int y)
        {
            FieldTile tile = new FieldTile();
            tile.Position = new Vector2Int(0, 0);
            if (this.tileData.TryGetValue(tile, out tile))
            {

            }
            else
            {

            }
            return tile;
        }

        public FieldTile GetOrAddTile(int x, int y)
        {
            FieldTile tile = new FieldTile();
            tile.Position = new Vector2Int(0, 0);
            if (this.tileData.TryGetValue(tile, out tile))
            {
            }
            else
            {

            }
            return tile;
        }

        public bool IsBarrier(Vector2Int position) => this.barrierData.Contains(position);
        public bool IsBarrier(int x, int y) => this.barrierData.Contains(new Vector2Int(x, y));

        internal HashSet<FieldObject> Initialization(Tilemap ground, Tilemap water, Tilemap surface)
        {
            Tilemap[] tilemaps = { ground, water, surface };
            for (int i = 0; i < tilemaps.Length; i++)
                if (tilemaps[i] != null)
                    tilemaps[i].ClearAllTiles();
            foreach (KeyValuePair<TileBase, HashSet<Vector3Int>> kvp in this.tilemaps)
                foreach (Vector3Int position in kvp.Value)
                    tilemaps[position.z].SetTile(new Vector3Int(position.x, position.y, 0), kvp.Key);
            return this.objectData;
        }

        private void Handle(HashSet<FieldObject> objs, Tilemap ground, Tilemap water, Tilemap surface, Tilemap barrier)
        {
            this.tilemaps.Clear();
            Tilemap[] tilemaps = { ground, water, surface };
            for (int layer = 0; layer < tilemaps.Length; layer++)
                if (tilemaps[layer] != null)
                    foreach (Vector3Int pos3 in tilemaps[layer].cellBounds.allPositionsWithin)
                    {
                        TileBase tile = tilemaps[layer].GetTile(pos3);
                        if (tile != null)
                        {
                            if (!this.tilemaps.ContainsKey(tile))
                                this.tilemaps.Add(tile, new HashSet<Vector3Int>());
                            this.tilemaps[tile].Add(new Vector3Int(pos3.x, pos3.y, layer));
                        }
                    }
            BoundsInt bounds = barrier.cellBounds;
            Vector2Int pos2 = Vector2Int.zero;
            for (pos2.x = bounds.xMin; pos2.x < bounds.xMax; pos2.x++)
                for (pos2.y = bounds.yMin; pos2.y < bounds.yMax; pos2.y++)
                    if (barrier.HasTile((Vector3Int)pos2))
                        this.barrierData.Add(pos2);
        }

        private void Write(BinaryWriter writer)
        {
            DataBuffer buffer = new DataBuffer();
            buffer.Input(this.objectData.Count);
            foreach (IDataSL obj in this.objectData)
                obj.SaveData(buffer);

            buffer.Input(this.tilemaps.Count);
            foreach (KeyValuePair<TileBase, HashSet<Vector3Int>> kvp in this.tilemaps)
                buffer
                    .Input<int>(AssetManager.GetTileBaseID(kvp.Key), kvp.Value.Count)
                    .Input<Vector3Int>(kvp.Value.ToArray());

            buffer
                .Input<int>(this.barrierData.Count)
                .Input<Vector2Int>(this.barrierData.ToArray());
            buffer.Write(writer);
        }

        private void Read(BinaryReader reader)
        {
            DataBuffer buffer = new DataBuffer();
            this.objectData.Clear();
            this.tilemaps.Clear();
            buffer.Read(reader);
            for (int i = buffer.Output<int>(); i > 0; i--)
            {
                FieldObject obj = new FieldObject();
                obj.LoadData(buffer);
                this.objectData.Add(obj);
            }
            for (int i = buffer.Output<int>(); i > 0; i--)
            {
                TileBase tile = AssetManager.GetTileBase(buffer.Output<int>());
                HashSet<Vector3Int> set = new HashSet<Vector3Int>();
                for (int j = buffer.Output<int>(); j > 0; j--)
                    set.Add(buffer.Output<Vector3Int>());
                this.tilemaps.Add(tile, set);
            }
            for (int i = buffer.Output<int>(); i > 0; i--)
                this.barrierData.Add(buffer.Output<Vector2Int>());
        }
    }
}