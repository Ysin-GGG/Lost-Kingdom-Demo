using LostKingdom.Content;
using LostKingdom.DataStructure;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LostKingdom.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObjectMemory : LKBehaviour
    {
        public bool HasLight => this._light != null;
        public bool HasRenderer => this._renderer != null;
        public bool HasCollider => this._collider != null;
        public bool HasInteraction => this._interaction != null;
        public Light2D Light => this._light ?? (this._light = this.AddComponent<Light2D>());
        public SpriteRenderer Renderer => this._renderer ?? (this._renderer = this.AddComponent<SpriteRenderer>());
        public PolygonCollider2D Collider => this._collider ?? (this._collider = this.AddComponent<PolygonCollider2D>());
        public InteractableSprite Interaction
        {
            get
            {
                if (!this.HasInteraction)
                {
                    if (!this.HasCollider) this._collider = this.AddComponent<PolygonCollider2D>();
                    this._interaction = this.AddComponent<InteractableSprite>();
                }
                return this._interaction;
            }
        }

        public FieldObject Tile { get; private set; }

        private Light2D _light;
        private SpriteRenderer _renderer;
        private PolygonCollider2D _collider;
        private InteractableSprite _interaction;

        protected override object HoverItem => this.Renderer;

        protected override void Awake()
        {
            this._renderer = this.GetComponent<SpriteRenderer>();
            if (!this.TryGetComponent(out this._collider)) this._collider = null;
            if (!this.TryGetComponent(out this._interaction)) this._interaction = null;
            if (!this.TryGetComponent(out this._light)) this._light = null;
        }

        protected override bool OnDeactivate()
        {
            this.Tile = null;
            if (this.HasLight) this.Light.enabled = false;
            if (this.HasCollider) this.Collider.enabled = false;
            if (this.HasInteraction) this.Interaction.enabled = false;
            return base.OnDeactivate();
        }
    }
}