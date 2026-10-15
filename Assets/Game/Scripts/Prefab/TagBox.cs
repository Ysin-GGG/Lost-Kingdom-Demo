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
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Prefab
{
    public class TagBox : LKBehaviour
    {
        [SerializeField]
        private Image _obj_tag;

        private Enum tagKw;

        protected override object HoverItem => this.tagKw;

        public void SetTag()
        {
            this.tagKw = null;
            this.Active = false;
        }

        public void SetTag(DamageID.Number tagID)
        {
            this._obj_tag.sprite = AssetManager.GetTag(tagID);
            this.tagKw = tagID;
            this.Active = true;
        }

        public void SetTag(ElementID.Number tagID)
        {
            this._obj_tag.sprite = AssetManager.GetTag(tagID);
            this.tagKw = tagID;
            this.Active = true;
        }

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.SetText(this.tagKw.ToString());
            return base.OnShowHoverText();
        }
    }
}
