using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Prefab
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    public class CombatText : LKBehaviour
    {
        [SerializeField]
        private TMP_Text tmp;
        [SerializeField]
        private Vector2 velocity;
        private int timeLeft;

        public string Text { get => this.tmp.text; set => this.tmp.text = value; }
        public float VelocityX { get => this.velocity.x; set => this.velocity.x = value; }
        public float VelocityY { get => this.velocity.y; set => this.velocity.y = value; }
        public Vector2 Velocity { get => this.velocity; set => this.velocity = value; }
        public LKColor Color { get; private set; }
        public int Style { get; set; }

        protected override object HoverItem => this.tmp;

        protected override void Awake()
        {
            if (this.tmp == null) this.tmp = this.tmp ?? this.GetComponent<TMP_Text>() ?? this.GetComponentInChildren<TMP_Text>(true);
            this.Color = new LKColor(this.tmp.color, (c) => { if (c != this.tmp.color) { this.tmp.color = c; } });
            this.rectTransform = this.tmp.rectTransform;
        }

        protected override void FixedUpdate()
        {
            this.Position += this.Velocity;
            if (this.VelocityX.Abs() < 1e-4f) this.VelocityX = 0f;
            if (this.VelocityY.Abs() < 1e-4f) this.VelocityY = 0f;
            if (this.timeLeft-- == 0)
            {
                this.Active = false;
                GameManager.KillCombatText(this);
            }

            switch (this.Style)
            {
                case 0:
                    if (timeLeft < 32)
                    {
                        this.Color.A -= 4;
                        this.Scale -= Vector2.one * .01f;
                    }
                    else if (timeLeft > 54)
                    {
                        this.Scale -= Vector2.one * .025f;
                        this.Color.g -= .08f;
                        this.Color.b -= .08f;
                        this.VelocityY *= .9f;
                    }
                    break;
            }
        }

        protected override bool OnActivate()
        {
            this.Scale = Vector2.one * .5f;
            this.Velocity = Vector2.zero;
            this.LocalRotation = 0;
            switch (this.Style)
            {
                case 0:
                    this.Color.Set(255, 255, 255, 255);
                    this.timeLeft = 64;
                    this.VelocityY = .05f;
                    this.Scale = Vector2.one * .7f;
                    return true;
                case 1:
                    return true;
            }
            return false;
        }
    }
}
