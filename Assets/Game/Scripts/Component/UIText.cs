using LostKingdom.Manager;
using LostKingdom.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostKingdom.Components
{
    public class UIText : LKBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private string hoverText;
        [SerializeField]
        internal TMP_Text _tmp;

        public string Text { get => this.GetText(); set => this.SetText(value); }
        public string HoverText { get => this.hoverText; set => this.SetHoverText(value); }
        protected override object HoverItem => this.hoverText;

        protected override void Awake()
        {
            if (this._tmp == null) this._tmp = this._tmp ?? this.GetComponent<TMP_Text>() ?? this.GetComponentInChildren<TMP_Text>(true);
        }

        protected override void Start()
        {
            if (this._tmp != null && this.Text.StartsWith("@"))
                this.Text = LKLocalization.GetText(this.Text.Substring(1));
        }

        protected override bool OnShowHoverText() => this.SetHoverText(this.hoverText);

        private bool SetHoverText(string text)
        {
            if (text == null || text.Length <= 0) return false;
            this.hoverText = text.StartsWith("@") ? LKLocalization.GetText(text.Substring(1)) : text;
            UIManager.Game_HoverText.SetText(this.hoverText);
            AudioManager.PlayInteractionAsset(0);
            return true;
        }

        private void SetText(string text)
        {
            if (this._tmp != null)
            {
                this._tmp.SetActive(true);
                this._tmp.text = text;
            }
        }

        private string GetText()
        {
            return this._tmp == null ? null : this._tmp.text;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Game_HoverText.Active = false;
            AudioManager.PlayInteractionAsset(1);
        }
    }
}
