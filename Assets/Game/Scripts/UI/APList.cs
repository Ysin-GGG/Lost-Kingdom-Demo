using LostKingdom.DataStructure;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class APList : LKBehaviour
    {
        [SerializeField]
        private APPointBox _prefab_apPointBox;

        private ObjectPool<APPointBox> APPointBoxes;

        private List<APPointBox> APPointBoxesList;

        public int RedCount { get; set; }

        protected override object HoverItem => GameManager.LocalCharacter;

        protected override void Awake()
        {
            this.APPointBoxesList = new List<APPointBox>();
            this.APPointBoxes = new ObjectPool<APPointBox>((i) =>
            {
                APPointBox apPointBox = APPointBox.Instantiate(this._prefab_apPointBox);
                apPointBox.Parent = this.gameObject;
                apPointBox.name = "AP Point " + (i + 1);
                apPointBox.ID = i;
                apPointBox.Active = false;
                return apPointBox;
            }, 12, "UI_HUD: AP Point Boxes");
        }

        protected override void Update()
        {
            if(GameManager.LocalCharacter != null)
            {
                int gray = GameManager.LocalCharacter.FinalMaxAP - GameManager.LocalCharacter.AP;
                int red = (GameManager.SelectedSymbol == null ? 0 : GameManager.SelectedSymbol.Cost) + this.RedCount;
                for (int i = this.APPointBoxesList.Count; i-- > 0;)
                    if (gray-- > 0) this.APPointBoxesList[i].Type = 0;
                    else if (red-- > 0) this.APPointBoxesList[i].Type = 2;
                    else this.APPointBoxesList[i].Type = 1;
            }
        }

        internal void UpdateValue()
        {
            if (this.APPointBoxes.Count != GameManager.LocalCharacter.FinalMaxAP)
            {
                this.APPointBoxes.Clear();
                this.APPointBoxesList.Clear();
                for (int i = 0; i < GameManager.LocalCharacter.FinalMaxAP; i++)
                {
                    APPointBox ap = this.APPointBoxes.Request();
                    ap.Active = true;
                    ap.Type = 1;
                    this.APPointBoxesList.Add(ap);
                }
            }
        }

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.SetText(string.Format("AP: {0}/{1} ({2}%)", GameManager.LocalCharacter.AP, GameManager.LocalCharacter.FinalMaxAP, (100.0f * GameManager.LocalCharacter.AP / GameManager.LocalCharacter.FinalMaxAP).Round(1)));
            return base.OnShowHoverText();
        }
    }
}