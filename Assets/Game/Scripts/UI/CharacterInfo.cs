using LostKingdom.Content;
using LostKingdom.DataStructure;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterInfo : LKBehaviour
    {
        [SerializeField]
        private GameObject _obj_symbols;
        [SerializeField]
        private GameObject _obj_equipments;
        [SerializeField]
        private GameObject _obj_deck;
        [SerializeField]
        private GameObject _obj_tags;
        [SerializeField]
        private GameObject[] _obj_pages;
        [SerializeField]
        private Image _obj_symbolIcon;
        [SerializeField]
        private Text _obj_symbolCost;
        [SerializeField]
        private Text _obj_symbolTooltip;
        [SerializeField]
        private Text _obj_symbolDescription;
        [SerializeField]
        private ToggleGroup _obj_toggleContent;
        [SerializeField]
        private EquipmentSlot[] _obj_equipmentSlots;
        [SerializeField]
        private List<Toggle> _obj_toggleContents;
        [SerializeField]
        private List<SymbolBox> _obj_symbolBoxes;

        [SerializeField]
        private EquipmentSlot _prefab_equipmentSlot;
        [SerializeField]
        private SymbolNode _prefab_symbolNode;
        [SerializeField]
        private SymbolBox _prefab_symbolBox;
        [SerializeField]
        private TagBox _prefab_tagBox;

        private ObjectPool<SymbolNode> symbolNodes;
        private ObjectPool<SymbolBox> symbolBoxes;
        private ObjectPool<TagBox> tagBoxes;

        [SerializeField]
        private int toggleID = 0;

        public int ToggleID { get => this.toggleID; set { this._obj_toggleContents[this.toggleID = value.Clamp(0, this._obj_toggleContents.Count - 1)].isOn = true; } }

        protected override void Awake()
        {
            this.symbolNodes = new ObjectPool<SymbolNode>((i) =>
            {
                SymbolNode symbolNode = SymbolNode.Instantiate(this._prefab_symbolNode);
                symbolNode.name = "Symbol Node " + (i + 1);
                symbolNode.SetSymbol(null);
                symbolNode.Parent = this._obj_symbols;
                return symbolNode;
            }, 32, "UI_Game: Symbol Nodes");
            this.symbolBoxes = new ObjectPool<SymbolBox>((i) =>
            {
                SymbolBox symbolBox = SymbolBox.Instantiate(this._prefab_symbolBox);
                symbolBox.name = "Symbol Box " + (i + 1);
                symbolBox.SetSymbol(null);
                symbolBox.Parent = this._obj_deck;
                symbolBox.Active = false;
                return symbolBox;
            }, 128, "UI_Game: Symbol Boxes");
            this.tagBoxes = new ObjectPool<TagBox>((i) =>
            {
                TagBox tagBox = TagBox.Instantiate(this._prefab_tagBox);
                tagBox.name = "Tag " + (i + 1);
                tagBox.SetTag();
                tagBox.Parent = this._obj_tags;
                tagBox.Active = false;
                return tagBox;
            }, 8, "UI_Game: Tag Boxes");
            this._obj_equipmentSlots = new EquipmentSlot[7];
            this._obj_symbolBoxes = new List<SymbolBox>();
            string[] names = Enum.GetNames(typeof(Equipment.SlotType));
            float[] x = { .5f, .5f, .5f, 0f, 1f, 0f, 1f };
            float[] y = { 1f, .5f, 0f, .5f, .5f, 1f, 1f };
            for (int i = 0; i < this._obj_equipmentSlots.Length; i++) {
                this._obj_equipmentSlots[i] = EquipmentSlot.Instantiate(this._prefab_equipmentSlot);
                this._obj_equipmentSlots[i].slot = i;
                this._obj_equipmentSlots[i].name = "Equipment Slot - " + names[i];
                this._obj_equipmentSlots[i].Anchor = new Vector4(x[i], y[i], x[i], y[i]);
                this._obj_equipmentSlots[i].Parent = this._obj_equipments;
                this._obj_equipmentSlots[i].SetEquipment(null);
            }
            this.Active = false;
        }

        protected override bool OnActivate()
        {

            if (this.symbolNodes != null) this.symbolNodes.Clear();
            if (this.symbolBoxes != null) this.symbolBoxes.Clear();
            foreach (Symbol symbol in GameManager.LocalCharacter.Deck)
                this.AddSymbol(symbol);
            return base.OnActivate();
        }

        public void SetEquipment(Equipment equipment)
        {
            if (equipment != null)
            {
                if (equipment.Slot == Equipment.SlotType.Hands)
                {
                    if (this._obj_equipmentSlots[(int)Equipment.SlotType.LeftHand].equipment == null)
                        this._obj_equipmentSlots[(int)Equipment.SlotType.LeftHand].SetEquipment(equipment);
                    else if (this._obj_equipmentSlots[(int)Equipment.SlotType.RightHand].equipment == null)
                        this._obj_equipmentSlots[(int)Equipment.SlotType.RightHand].SetEquipment(equipment);
                }
                else if (equipment.Slot == Equipment.SlotType.Accessory)
                {
                    if (this._obj_equipmentSlots[(int)Equipment.SlotType.LeftAccessory].equipment == null)
                        this._obj_equipmentSlots[(int)Equipment.SlotType.LeftAccessory].SetEquipment(equipment);
                    else if (this._obj_equipmentSlots[(int)Equipment.SlotType.RightAccessory].equipment == null)
                        this._obj_equipmentSlots[(int)Equipment.SlotType.RightAccessory].SetEquipment(equipment);
                }
                else this._obj_equipmentSlots[(int)Equipment.SlotType.LeftHand].SetEquipment(equipment);
            }
        }

        public void SelectSymbol(Symbol symbol)
        {
            if (symbol != null)
            {
                this._obj_symbolIcon.sprite = AssetManager.GetSymbol(symbol.Type);
                this._obj_symbolCost.text = symbol.Cost.ToString();
                this._obj_symbolTooltip.text = symbol.GetEffectText();
                this._obj_symbolDescription.text = symbol.Description;
                this.tagBoxes.Clear();
                this.tagBoxes.Request().SetTag(ElementID.Number.Fire);
            }
        }

        public void AddSymbol(Symbol symbol)
        {
            if (symbol != null)
            {
                this.symbolNodes.Request().SetSymbol(symbol);
                SymbolBox symbolBox = this.symbolBoxes.Request();
                symbolBox.Active = true;
                symbolBox.SetSymbol(symbol);
            }
        }

        public bool RemoveSymbol(Symbol symbol)
        {
            SymbolNode symbolNode = this.symbolNodes.Find((t) => t.symbol == symbol);
            SymbolBox symbolBox = this.symbolBoxes.Find((t) => t.symbol == symbol);
            if (symbolNode != null) {
                this.symbolNodes.Release(symbolNode).SetSymbol(null);
            }
            if (symbolBox != null)
            {
                symbolBox.Active = true;
                symbolBox.SetSymbol(null);
            }
            return symbolNode != null && symbolBox != null;
        }

        protected override void Update()
        {
            
        }

        public void ToggleLabel(int value, bool? loop)
        {
            this.Active = true;
            if (value % this._obj_toggleContents.Count == 0) return;
            this.toggleID += value;
            loop = loop ?? Options.AllowPageLoop;
            if (!loop.Value) this.toggleID = this.toggleID.Clamp(0, this._obj_toggleContents.Count - 1);
            else this.toggleID = (this.toggleID + this._obj_toggleContents.Count) % this._obj_toggleContents.Count;
            this._obj_toggleContents[this.toggleID].isOn = true;
        }

        public void SetLabel(int value)
        {
            this.Active = true;
            this.toggleID = value.Clamp(0, this._obj_toggleContents.Count - 1);
            for (int i = 0; i < this._obj_pages.Length; i++)
                this._obj_pages[i].gameObject.SetActive(this.toggleID == i);
        }
    }
}