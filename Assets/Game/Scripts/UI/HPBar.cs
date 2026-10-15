using LostKingdom.Manager;
using LostKingdom.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class HPBar : LKBehaviour
    {
        [SerializeField]
        private Image _obj_filler_damage;
        [SerializeField]
        private Image _obj_filler_healing;
        [SerializeField]
        private Image _obj_filler_common;

        public float Percent { get => this.percent1; set => this.SetPercent(value.Clamp(0f, 1f)); }
        public int ValueMax { get => this.ValueMax; set => this.SetValueMax(value.Max(1)); }
        public int Value { get => this.value; set => this.SetValue(value.Min(this.valueMax)); }

        public int SyncFillerAnimDelay { get; set; } = 60;
        public int SyncFillerAnimTime { get; set; } = 60;

        [Range(0f, 1f)]
        [SerializeField]
        private float percent;
        private float percent1;
        private float percent2;
        private float percent3;
        private int valueMax = 1;
        private int value;
        private int timer;

        protected override object HoverItem => GameManager.LocalCharacter;
        /// <summary> 设置生命条上限数值 </summary>
        /// <param name="valueMax"> 生命值上限 </param>
        private void SetValueMax(int valueMax)
        {
            if (this.valueMax != valueMax)
            {
                this.valueMax = valueMax;
                if (this.value > this.valueMax) this.SetValue(valueMax);
            }
        }
        /// <summary> 设置生命条数值 </summary>
        /// <param name="value"> 生命值 </param>
        private void SetValue(int value)
        {
            if (this.value != value)
            {
                this.value = value;
                this.SetPercent((float)this.value / this.valueMax);
            }
        /// <summary> 设置血条上限值 </summary>
        }
        /// <summary> 按百分比形式设置生命条数值 </summary>
        /// <param name="value"> 百分比（0-1） </param>
        private void SetPercent(float value)
        {
            if (this.percent1 != value)
            {
                this.percent2 = this.percent3;
                this.percent1 = value;
                this.timer = -this.SyncFillerAnimDelay;
                this._obj_filler_common.rectTransform.anchorMax = new Vector2(this.percent1, 1f);
            }
        }

        protected override void Awake()
        {
            this.percent3 = this.percent1;
            this.percent2 = this.percent1;
            this.timer = -this.SyncFillerAnimDelay;
        }

        protected override void Update()
        {
            if (this.percent2 < this.percent1) this.percent3 = this.percent2 = this.percent1;
            if (this.timer++ > 0)
            {
                if (this.percent3 == this.percent1) this.percent2 = this.percent1;
                else this.percent3 = ((float)this.timer / this.SyncFillerAnimTime).Lerp(this.percent2, this.percent1);
            }
            this._obj_filler_damage.rectTransform.anchorMax = new Vector2(this.percent3, 1f);
            this._obj_filler_damage.color = new LKColor(1f, 1f, 1f, .5f + LK.GetFloatingSin(4f) * .5f);
            if (this.value != GameManager.LocalCharacter.HP) this.Value = GameManager.LocalCharacter.HP;
            if (this.valueMax != GameManager.LocalCharacter.FinalMaxHP) this.ValueMax = GameManager.LocalCharacter.FinalMaxHP;
            //if (this.Percent != this.percent) GameManager.LocalCharacter.HP = LKMath.Round(GameManager.LocalCharacter.FinalMaxHP * (this.Percent = this.percent));
        }

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.SetText(string.Format("HP: {0}/{1} ({2}%)", this.value, this.valueMax, (this.Percent * 100.0f).Round(1)));
            return base.OnShowHoverText();
        }
    }
}