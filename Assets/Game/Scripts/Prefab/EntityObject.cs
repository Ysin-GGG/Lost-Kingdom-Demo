using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Prefab
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EntityObject : LKBehaviour
    {
        public Entity Entity { get => this.entity; set => this.entity = value; }

        public Sprite Sprite { get => this.spriteRenderer.sprite; set => this.spriteRenderer.sprite = value; }

        public LKColor Color { get => this.spriteRenderer.color; set => this.spriteRenderer.color = value; }

        private SpriteRenderer spriteRenderer;
        private PolygonCollider2D polygonCollider;
        private Entity entity;

        protected override object HoverItem => this.entity;
        protected override void Awake()
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
            this.TryGetComponent<PolygonCollider2D>(out this.polygonCollider);
        }

        protected override void Update()
        {
            this.LocalPosition = this.entity.Position;
        }

        protected override bool OnActivate() => this.entity != null && this.entity.Type != 0;

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.SetText(this.Entity.ToString());
            UIManager.IsHoverTextShowing = true;
            UIManager.IsHoverTextMoving = true;
            return base.OnShowHoverText();
        }
    }
}
