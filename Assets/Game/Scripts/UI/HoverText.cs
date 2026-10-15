using LostKingdom.Components;
using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class HoverText : LKBehaviour
    {
        [SerializeField]
        private UIText _obj_title;
        [SerializeField]
        private UIText _obj_text;
        [SerializeField]
        private UIText _obj_tip;
        [SerializeField]
        private UIText _obj_subtitle;
        [SerializeField]
        private UIText _obj_textClone;
        [SerializeField]
        private UIText _obj_apcost;
        [SerializeField]
        private GameObject _obj_aps;
        [SerializeField]
        private Image _obj_apbox;
        [SerializeField]
        private Image _obj_panel;
        [SerializeField]
        private List<Image> _list_aps;

        private Symbol symbol;
        private Item item;
        private Equipment equipment;

        public LKColor TitleColor { get; private set; }
        public LKColor TextColor { get; private set; }
        public LKColor TipColor { get; private set; }
        public LKColor SubtitleColor { get; private set; }
        public Vector2 Offset { get; set; } = new Vector2(16f, -16f);
        public Vector4 Border { get; set; } = Vector4.one * 16f;

        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this._obj_title.Text;
                    case 1:
                        return this._obj_text.Text;
                    case 2:
                        return this._obj_tip.Text;
                    case 3:
                        return this._obj_subtitle.Text;
                    default:
                        return null;
                }
            }
            set
            {
                bool flag = value != null;
                switch (index)
                {
                    case 0:
                        this._obj_title.Text = value;
                        LKUtility.SetActive(this._obj_title, flag);
                        break;
                    case 1:
                        this._obj_text.Text = value;
                        this._obj_textClone.Text = value;
                        LKUtility.SetActive(this._obj_text, flag);
                        LKUtility.SetActive(this._obj_textClone, flag);
                        break;
                    case 2:
                        this._obj_tip.Text = value;
                        LKUtility.SetActive(this._obj_tip, flag);
                        break;
                    case 3:
                        this._obj_subtitle.Text = value;
                        LKUtility.SetActive(this._obj_subtitle, flag);
                        break;
                }
            }
        }

        public string Title { get => this[0]; internal set => this[0] = value; }
        public string Text { get => this[1]; internal set => this[1] = value; }
        public string Tip { get => this[2]; internal set => this[2] = value; }
        public string Subtitle { get => this[3]; internal set => this[3] = value; }
        public Symbol Symbol { get => this.symbol; set => this.SetSymbol(value); }
        public Item SelectItem { get => this.item; set => this.SetItem(value); }
        public Equipment Equipment { get => this.equipment; set => this.SetEquipment(value); }

        private void SetCost(int cost)
        {
            if (cost < 0)
            {
                LKUtility.SetActive(this._obj_aps, false);
                LKUtility.SetActive(this._obj_apbox, false);
                LKUtility.SetActive(this._obj_apcost, false);
            }
            else
            {
                this._obj_apcost.Text = cost.ToString();
                LKUtility.SetActive(this._obj_aps, true);
                LKUtility.SetActive(this._obj_apbox, true);
                LKUtility.SetActive(this._obj_apcost, true);
                for (int i = this._list_aps.Count; i < cost; i++)
                {
                    Image image = Image.Instantiate(this._list_aps[0]);
                    image.transform.SetParent(this._obj_aps.transform);
                    image.name = this._list_aps[0].name;
                    this._list_aps.Add(image);
                }
                for (int i = 0; i < this._list_aps.Count; i++)
                    this._list_aps[i].SetActive(i < cost);
            }
        }
        public void SetEquipment(Equipment equipment)
        {
            if ((this.equipment = equipment) == null) return;
            this[0] = equipment.DisplayName;
            this[1] = equipment.DisplayTooltip;
            this[2] = equipment.Description;
            this.SetCost(-1);
            this.TitleColor.Set(ColorID.GetColor(this.equipment.Quality.ToString()));
            /*
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
            for (int i = 0, j = 0; i < attributes.Count; i++)
                if (attributes[i].Item2 != 0)
                    this.SetTooltip(j++, attributes[i].Item1, attributes[i].Item2);
            */
        }
        public void SetItem(Item item)
        {
            if ((this.item = item) == null) return;
            this[0] = item.DisplayName;
            this[1] = null;
            this[2] = item.Description;
            this.SetCost(-1);
            this.TitleColor.Set(Color.white);
        }

        private void SetSymbol(Symbol symbol)
        {
            this.symbol = symbol;
            if (this.symbol == null) this.Reset();
            else
            {
                this.Active = true;
                this.SetCost(this.symbol.Cost);
                this[0] = this.symbol.DisplayName;
                this[1] = symbol.GetBasicText() + symbol.GetEffectText();
                this[2] = this.symbol.Description;
                this[3] = this.symbol.Quality.ToString();
                this.TitleColor.Set(ColorID.GetColor(this.symbol.Quality.ToString()));
            }
        }

        public void SetText(string text)
        {
            this.Symbol = null;
            this.Text = text;
            this.Active = true;
        }

        private void Reset()
        {
            this[0] = null;
            this[1] = null;
            this[2] = null;
            this[3] = null;
            this.SetCost(-1);
            this.Active = false;
        }

        protected override void Awake()
        {
            this.TitleColor = new LKColor(this._obj_title._tmp.color, (c) => { if (c != this._obj_title._tmp.color) { this._obj_title._tmp.color = c; } });
            this.TextColor = new LKColor(this._obj_text._tmp.color, (c) => { if (c != this._obj_text._tmp.color) { this._obj_text._tmp.color = c; } });
            this.TipColor = new LKColor(this._obj_tip._tmp.color, (c) => { if (c != this._obj_tip._tmp.color) { this._obj_tip._tmp.color = c; } });
            this.SubtitleColor = new LKColor(this._obj_subtitle._tmp.color, (c) => { if (c != this._obj_subtitle._tmp.color) { this._obj_subtitle._tmp.color = c; } });
            this.rectTransform = this._obj_panel.rectTransform;
            this.Active = false;
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
        protected override bool OnActivate()
        {
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
    }
}