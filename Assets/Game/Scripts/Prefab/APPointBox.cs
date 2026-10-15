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
    public class APPointBox : LKBehaviour
    {
        [SerializeField]
        private Image _obj_img;

        public int Type { set => this._obj_img.sprite = AssetManager.APPoints[value.Clamp(0, 2)]; }

        public int ID { get; internal set; } = 0;
    }
}
