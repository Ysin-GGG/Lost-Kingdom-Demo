using LostKingdom.Content;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class ChatWindow : LKBehaviour
    {
        [SerializeField]
        private TMP_Text _obj_text;
        [SerializeField]
        private GameObject _obj_options;
        [SerializeField]
        private ChatOption _prefab_chatOption;

        public int ColumnPrintDelay { get; set; } = 2;
        public int LinePrintDelay { get; set; } = 20;
        public bool ChatHasEnded { get; private set; } = false;
        public bool QuickChat { get; private set; } = false;

        /// <summary> 选项池（预设容量为4个） </summary>
        private List<ChatOption> chatOptions;
        private ChatSet chatSet = null;
        private string rawText = string.Empty;
        private int textLength = 0;
        private int delay = 0;

        /// <summary> 设置对应选项文本描述 </summary>
        /// <param Name="flag"> 编号 </param>
        /// <returns> 选项文本描述 </returns>
        public string this[int index]
        {
            get => this.chatOptions[index.Clamp(0, 3)].Description;
            set => this.chatOptions[index.Clamp(0, 3)].Description = value;
        }

        public void SetChat(ChatSet chat)
        {
            this.QuickChat = false;
            this.ChatHasEnded = false;
            this.chatSet = chat;
            this.textLength = 0;
            this.delay = 0;
            this._obj_text.text = string.Empty;
            this.Active = chat != null;
            if (chat == null) this.ChatHasEnded = true;
            else this.rawText = this.chatSet.GetChat();
            foreach (ChatOption option in this.chatOptions)
                if (option.Active.Value)
                {
                    option.Description = string.Empty;
                    option.Active = false;
                }
        }

        /// <summary> 加速→跳过该段对话 </summary>
        public void SkipChat()
        {
            if (this.QuickChat)
            {
                this.textLength = this.rawText.Length;
                this._obj_text.text = this.rawText;
            }
            else this.QuickChat = true;
        }

        /// <summary> 开启下一段对话，若无则结束对话 </summary>
        public void NextChat()
        {
            if (!this.ChatHasEnded)
            {
                this.QuickChat = false;
                this.delay = 0;
                string next = this.chatSet.GetChat();
                if (next == null)
                {
                    this.ChatHasEnded = true;
                    string[] options = this.chatSet.Options;
                    int count = 0;
                    foreach (string option in options)
                    {
                        if (option != null)
                        {
                            this.chatOptions[count].Active = true;
                            this.chatOptions[count].Description = option;
                            count++;
                        }
                    }
                    if (count == 0) this.SetChat(null);
                }
                else
                {
                    this.rawText = next;
                    this.textLength = 0;
                    this._obj_text.text = string.Empty;
                }
            }
        }

        public void SelectOption(int index)
        {
            this.SetChat(this.chatSet.SelectOption(index));
        }

        public void OnClick()
        {
            if (this.textLength < this.rawText.Length) this.SkipChat();
            else this.NextChat();
        }

        protected override void Awake()
        {
            this.chatOptions = new List<ChatOption>(4);
            for (int i = 0; i < this.chatOptions.Capacity; i++)
            {
                ChatOption obj = ChatOption.Instantiate(this._prefab_chatOption);
                obj.name = $"Chat Option {i + 1}";
                obj.Parent = this._obj_options;
                obj.SetDefaults(i);
                this.chatOptions.Add(obj);
            }
        }

        protected override void FixedUpdate()
        {
            if (this.QuickChat)
                for (int i = 0; i < 3 && this.textLength < this.rawText.Length; i++)
                    this._obj_text.text += this.rawText[this.textLength++];
            else if (this.textLength < this.rawText.Length && ++this.delay > (this.rawText[this.textLength] == '\n' ? this.LinePrintDelay : this.ColumnPrintDelay))
            {
                this._obj_text.text += this.rawText[this.textLength++];
                this.delay = 0;
            }
        }
    }
}