using LostKingdom.Content;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class HoverTooltip : LKBehaviour
    {
        [SerializeField]
        private Text _obj_title;
        [SerializeField]
        private Text _obj_subtitle;
        [SerializeField]
        private Text _obj_description;
        [SerializeField]
        private Image _obj_divider1;
        [SerializeField]
        private Image _obj_divider2;
        [SerializeField]
        private Image _obj_panel;

        [SerializeField]
        private SymbolNode _prefab_symbolNode;
        [SerializeField]
        private HoverAttribute _prefab_hoverAttribute;

        public string Title { get => this[0]; set => this[0] = value; }
        public string Subtitle { get => this[1]; set => this[1] = value; }
        public string Description { get => this[2]; set => this[2] = value; }
        public LKColor TitleColor { get; private set; }
        public LKColor SubtitleColor { get; private set; }
        public LKColor DescriptionColor { get; private set; }
        public Vector2 Offset { get; set; }
        public Vector4 Border { get; set; } = Vector4.one * 16f;

        /// <summary> 装备面板词条 </summary>
        private List<HoverAttribute> attributes = new List<HoverAttribute>();
        /// <summary> 装备内置技能 </summary>
        private SymbolNode symbolNode = null;
        /// <summary> 装备 </summary>
        private Equipment equipment = null;
        /// <summary> 技能 </summary>
        private Symbol symbol = null;
        private bool onlyText = false;
        private bool updated = false;

        private string this[int type]
        {
            get
            {
                switch (type)
                {
                    case 0: 
                        return this._obj_title.text;
                    case 1:
                        return this._obj_subtitle.text;
                    case 2:
                        return this._obj_description.text;
                }
                return null;
            }
            set
            {
                switch (type)
                {
                    case 0:
                        if (this._obj_title.text == value) return;
                        this._obj_title.text = value;
                        this._obj_title.SetActive(value != null);
                        this.updated = false;
                        break;
                    case 1:
                        if (this._obj_subtitle.text == value) return;
                        this._obj_subtitle.text = value;
                        this._obj_subtitle.SetActive(value != null);
                        this.updated = false;
                        break;
                    case 2:
                        if (this._obj_description.text == value) return;
                        this._obj_description.text = value;
                        this._obj_description.SetActive(value != null);
                        this.updated = false;
                        break;
                }
            }
        }

        protected override bool OnActivate()
        {
            if(!this.updated) this.UpdateSelf();
            if (UIManager.HoverDelayTimer < UIManager.HoverDelay) UIManager.HoverDelayTimer++;
            else UIManager.IsHoverTextShowing = false;
            return !UIManager.IsHoverTextShowing;
        }

        protected override bool OnDeactivate()
        {
            UIManager.IsHoverTextShowing = false;
            UIManager.HoverDelayTimer = 0;
            return true;
        }

        protected override void Update()
        {
            if (UIManager.IsHoverTextMoving)
            {
                Vector2 position = (Vector2)Input.mousePosition + this.Offset;
                Vector2 edge1 = new Vector2(this.Border.x, this.Border.y + this._obj_panel.rectTransform.sizeDelta.y);
                Vector2 edge2 = new Vector2(LK.ScreenWidth - this._obj_panel.rectTransform.sizeDelta.x - this.Border.z, LK.ScreenHeight - this.Border.w);
                if (edge1.x < edge2.x) position.x = LKMath.Clamp(position.x, edge1.x, edge2.x);
                else if (position.x < LK.ScreenWidth / 2f) position.x = edge1.x;
                else position.x = edge2.x;
                if (edge1.y < edge2.y) position.y = LKMath.Clamp(position.y, edge1.y, edge2.y);
                else if (position.y < LK.ScreenHeight / 2f) position.y = edge2.y;
                else position.y = edge1.y;
                this.Position = position;
            }
            else
            {
                Vector2 position = this.Position;
                Vector2 edge1 = new Vector2(this.Border.x, this.Border.y + this._obj_panel.rectTransform.sizeDelta.y);
                Vector2 edge2 = new Vector2(LK.ScreenWidth - this._obj_panel.rectTransform.sizeDelta.x - this.Border.z, LK.ScreenHeight - this.Border.w);
                if (edge1.x < edge2.x) position.x = LKMath.Clamp(position.x, edge1.x, edge2.x);
                else if (position.x < LK.ScreenWidth / 2f) position.x = edge1.x;
                else position.x = edge2.x;
                if (edge1.y < edge2.y) position.y = LKMath.Clamp(position.y, edge1.y, edge2.y);
                else if (position.y < LK.ScreenHeight / 2f) position.y = edge2.y;
                else position.y = edge1.y;
                this.Position = position;
            }
        }

        private void UpdateSelf()
        {
            bool flag1 = this._obj_title.GetActive() || this._obj_subtitle.GetActive();
            bool flag2 = (this.symbolNode.Active.Value || this.attributes.Count > 0) && !this.onlyText;
            bool flag3 = this._obj_description.GetActive();
            this._obj_divider1.SetActive(flag1 && (flag2 || flag3));
            this._obj_divider2.SetActive(flag2 && flag3);
            this.updated = true;
        }

        private void SetTooltip(int index, string name, int value)
        {
            if (this.attributes.Count > index)
            {
                HoverAttribute attribute = this.attributes[index];
                attribute.Name = name;
                attribute.Value = value;
                attribute.Active = true;
            }
            else
            {
                HoverAttribute attribute = HoverAttribute.Instantiate(this._prefab_hoverAttribute);
                attribute.Name = name;
                attribute.Value = value;
                attribute.transform.name = "Hover Attribute - " + index;
                attribute.transform.SetParent(this._obj_panel.transform, false);
                attribute.transform.SetSiblingIndex(this._obj_divider2.transform.GetSiblingIndex());
                this.attributes.Add(attribute);
            }
        }

        public void SetTooltips(Equipment equipment)
        {
            if ((this.equipment = equipment) == null) return;
            this[0] = equipment.DisplayName;
            this[1] = equipment.DisplayTooltip;
            this[2] = equipment.Description;
            List<(string, int)> attributes = new List<(string, int)>
            {
                ("Max Life", equipment.Life),
                ("Max Cost", equipment.Cost),
                (nameof(equipment.Attack), equipment.Attack),
                (nameof(equipment.Defence), equipment.Defence),
            };
            this.onlyText = false;
            this.updated = false;
            this.UpdateSelf();
            this.symbolNode.SetSymbol(equipment.Symbol);
            this.equipment.ModifyTooltip(attributes);
            foreach (HoverAttribute attribute in this.attributes)
                attribute.Active = false;
            for (int i = 0, j = 0; i <  attributes.Count; i++)
                if (attributes[i].Item2 != 0) 
                    this.SetTooltip(j++, attributes[i].Item1, attributes[i].Item2);
        }

        public void SetTooltips(Symbol symbol)
        {
            if ((this.symbol = symbol) == null) return;
            this[0] = symbol.DisplayName;
            this[1] = null;
            this[2] = symbol.GetEffectText() + symbol.Description;
            this.UpdateSelf();
        }

        public void SetText(string text)
        {
            if (text == null)
            {
                this.Active = false;
                return;
            }
            this[0] = null;
            this[1] = null;
            this[2] = text;
            this.symbolNode.Active = false;
            foreach (HoverAttribute attribute in this.attributes)
                attribute.Active = false;
            this.onlyText = true;
            this.updated = false;
            this.UpdateSelf();
            UIManager.HoverDelayTimer = UIManager.HoverDelay;
            UIManager.IsHoverTextMoving = true;
        }

        protected override void Awake()
        {
            this.TitleColor = new LKColor(this._obj_title.color, (c) => { if (c != this._obj_title.color) { this.updated = false; this._obj_title.color = c; } });
            this.SubtitleColor = new LKColor(this._obj_subtitle.color, (c) => { if (c != this._obj_subtitle.color) { this.updated = false; this._obj_subtitle.color = c; } });
            this.DescriptionColor = new LKColor(this._obj_description.color, (c) => { if (c != this._obj_description.color) { this.updated = false; this._obj_description.color = c; } });
            this.symbolNode = SymbolNode.Instantiate(this._prefab_symbolNode);
            this.symbolNode.Active = false;
            this.symbolNode.name = "Extra Symbol";
            this.symbolNode.transform.SetParent(this._obj_panel.transform, false);
            this.symbolNode.transform.SetSiblingIndex(this._obj_divider1.transform.GetSiblingIndex() + 1);
            this.Offset = new Vector2(16, -16);
            this.rectTransform = this._obj_panel.rectTransform;
            this.Active = false;
        }
    }
}