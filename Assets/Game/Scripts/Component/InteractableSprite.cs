using LostKingdom.Manager;
using LostKingdom.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace LostKingdom.Components
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractableSprite : LKBehaviour, IPointerClickHandler
    {
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private int clickEventID;
        public int ClickEventID { get => this.clickEventID; set => this.clickEventID = value; }

        protected override object HoverItem => this.spriteRenderer;

        protected override void Awake()
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        protected override bool OnShowHoverText()
        {
            FieldManager.SetOutline(this.spriteRenderer);
            return false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (this.spriteRenderer.sprite.name == "Candle")
                this.GetComponent<Light2D>().enabled = !this.GetComponent<Light2D>().enabled;
            switch(this.ClickEventID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    GameManager.SetChat(this.ClickEventID);
                    break;
            }
        }

    }
}
