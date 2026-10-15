using LostKingdom.ID;
using LostKingdom.DataStructure;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using LostKingdom.Utility;
using LostKingdom.Manager;

namespace LostKingdom.Content
{
    public class ChatSet
    {
        private LinkedList<string> texts;
        private LinkedListNode<string> head;
        private LinkedListNode<string> curr;
        private LinkedListNode<string> tail;
        private readonly string[] options;
        public int Type { get; private set; } = -1;
        public string[] Options => this.options;

        /// <summary> 设置对应选项文本描述 </summary>
        /// <param Name="flag"> 编号 </param>
        /// <returns> 选项文本描述 </returns>
        public string this[int index]
        {
            get => this.options[index.Clamp(0, 3)];
            set => this.options[index.Clamp(0, 3)] = value;
        }

        public ChatSet(int type)
        {
            this.options = new string[4] { null, null, null, null };
            this.texts = new LinkedList<string>(this.SetDefaults(type));
            this.head = texts.First;
            this.curr = texts.First;
            this.tail = texts.Last;
        }

        public ChatSet(string text)
        {
            this.options = new string[4] { null, null, null, null };
            this.texts = new LinkedList<string>(new string[1] { text });
            this.head = texts.First;
            this.curr = texts.First;
            this.tail = texts.Last;
        }

        public string[] SetDefaults(int type)
        {
            int options = 0;
            int chats = 1;
            switch(this.Type = type)
            {
                case 1:
                case 2:
                    options = 2;
                    break;
                case 3:
                case 4:
                    break;
                default:
                    return null;
            }
            string[] strs = new string[chats];
            for (int i = 0; i < chats; i++)
                strs[i] = LKLocalization.GetText($"ChatSet.Context_{this.Type}.{i}");
            for (int i = 0; i < options; i++)
                this[i] = LKLocalization.GetText($"ChatSet.Option_{this.Type}.{i}");
            return strs;
        }

        /// <summary> 获取下一段对话 </summary>
        /// <returns> 若对话已结束，则返回null，否则返回下一段对话 </returns>
        public string GetChat()
        {
            if (this.curr == null) return null;
            string chat = this.curr.Value;
            this.curr = this.curr.Next;
            return chat;
        }

        public ChatSet SelectOption(int index)
        {
            switch (this.Type)
            {
                case 1:
                    if (index == 0) FieldManager.Field = new Field(@".\Assets\Game\Fields\HuntersHouse.bin");
                    return null;
                case 2:
                    if (index == 0) FieldManager.Field = new Field(@".\Assets\Game\Fields\HuntersHouseField.bin");
                    else return new ChatSet("还是再仔细检查一下这个地方吧。");
                    return null;
            }
            return null;
        }
    }
}