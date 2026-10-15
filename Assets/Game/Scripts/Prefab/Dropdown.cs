using LostKingdom.Content;
using LostKingdom.Components;
using LostKingdom.ID;
using LostKingdom.Localization;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace LostKingdom.Prefab
{
    public class Dropdown : LKBehaviour
    {
        [SerializeField]
        private GameObject _obj_list;
        [SerializeField]
        private UIText _obj_text;
        [SerializeField]
        private Button _obj_button;
        [SerializeField]
        private DropdownItem _prefab_dropdownItem;

        private List<string> items;

        public int ID { get; private set; }
        public int type;

        public event Action<int> OnValueChanged;

        protected override void Awake()
        {
            this.items = new List<string>();
        }

        protected override void Start()
        {
            this.SetDefaults();
        }

        protected override void LateUpdate()
        {
            if (this._obj_list != null)
            {
                GameObject obj = EventSystem.current.currentSelectedGameObject;
                bool flag = obj == this.gameObject || (obj != null && obj.GetComponent<DropdownItem>() && obj.transform.parent == this._obj_list.transform);
                this._obj_list.SetActive(flag);
            }
        }

        public void SetValue(int id) 
        {
            this.ID = id.Clamp(0, this.items.Count - 1);
            this._obj_text.Text = this.items[id];
            switch (this.type)
            {
                case 1:
                    Resolution resolution = Screen.resolutions[id];
                    Options.ScreenResolution = new Vector2(resolution.width, resolution.height);
                    break;
                case 2:
                    int fps = new int[] { 30, 60, 120, 144, 165, 240 }[id] ;
                    Options.FramePerSecond = fps;
                    break;
                case 3:
                    if (this.items.Count > 1) Options.LanguageID = (byte)(id + 1);
                    break;
            }
            this._obj_list.SetActive(false);
            this.OnValueChanged?.Invoke(id);
        }

        public void AddItem(string label) 
        {
            this.items.Add(label);
            if (this.items.Count == 1) this.SetValue(0);
            DropdownItem item = DropdownItem.Instantiate(this._prefab_dropdownItem);
            item.Parent = this._obj_list;
            item.name = "Dropdown Item " + this.items.Count;
            item.Text = label;
            item.SetOwner(this, this.items.Count - 1);
        }

        public void SetDefaults()
        {
            switch (this.type)
            {
                case 1:
                    Resolution[] arr = Screen.resolutions;
                    for (int i = arr.Length - 1; i >= 0; i--)
                        this.AddItem($"{arr[i].width}x{arr[i].height}");
                    this._obj_text.Text = $"{Options.ScreenResolution.x}x{Options.ScreenResolution.y}";
                    break;
                case 2:
                    foreach (int fps in new int[] { 30, 60, 120, 144, 165, 240 })
                        this.AddItem($"{fps}");
                    this._obj_text.Text = $"{Options.FramePerSecond}";
                    break;
                case 3:
                    foreach (GameCulture.CultureInfo lang in GameCulture.GameCultureInfo.Keys)
                        this.AddItem(LKLocalization.GetText($"UIText.Language.{lang.ToString()}"));
                    this._obj_text.Text = $"{LKLocalization.GetText($"UIText.Language.{((GameCulture.CultureInfo)Options.LanguageID).ToString()}")}";
                    break;
            }
        }
    }
}
