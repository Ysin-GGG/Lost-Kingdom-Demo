using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostKingdom.Prefab
{
    public class DropdownItem : LKBehaviour
    {
        [SerializeField]
        private Text _obj_text;
        [SerializeField]
        private Button _obj_button;

        private Dropdown _obj_owner;

        public string Text { get => this._obj_text.text; set => this._obj_text.text = value; }

        private int id;

        public void SetOwner(Dropdown dropdown, int id)
        {
            this.id = id;
            this._obj_owner = dropdown;
            this.Active = true;
        }

        public void OnClick()
        {
            this._obj_owner.SetValue(this.id);
        }
    }
}
