using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LostKingdom.IO
{
    /// <summary> 游戏键位映射 </summary>
    public static class Keymap
    {
        /// <summary> 每个键位的最大映射数 </summary>
        public const byte MaxMultiKeymap = 4;

        /// <summary> 键位数 </summary>
        public const byte MaxKeymap = 12;

        /// <summary> 默认键位映射表 </summary>
        internal static readonly KeyCode[][] DefaultKeymap = new KeyCode[Keymap.MaxKeymap][]
        {
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.W, KeyCode.UpArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.S, KeyCode.DownArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.A, KeyCode.LeftArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.D, KeyCode.RightArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.R, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.B, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.V, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.M, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.E, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.F, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.Z, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.Escape, KeyCode.None, KeyCode.None, KeyCode.None },
        };

        /// <summary> 当前键位映射表 </summary>
        internal static readonly KeyCode[][] CurrentKeymap = new KeyCode[Keymap.MaxKeymap][]
        {
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.W, KeyCode.UpArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.S, KeyCode.DownArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.A, KeyCode.LeftArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.D, KeyCode.RightArrow, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.R, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.B, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.V, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.M, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.E, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.F, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.Z, KeyCode.None, KeyCode.None, KeyCode.None },
            new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.Escape, KeyCode.None, KeyCode.None, KeyCode.None },
        };

        /// <summary> 游戏键位 </summary>
        public enum GameKey
        {
            /// <summary> 角色移动模式：上移一步 </summary>
            CharacterUp,
            /// <summary> 角色移动模式：下移一步 </summary>
            CharacterDown,
            /// <summary> 角色移动模式：左移一步 </summary>
            CharacterLeft,
            /// <summary> 角色移动模式：右移一步 </summary>
            CharacterRight,
            /// <summary> 进入/退出角色移动模式 </summary>
            CharacterMoveMode,
            /// <summary> 打开背包界面 </summary>
            OpenInventoryUI,
            /// <summary> 打开技能界面 </summary>
            OpenSymbolUI,
            /// <summary> 打开地图界面 </summary>
            OpenMapUI,
            /// <summary> 结束回合 </summary>
            EndAction,
            /// <summary> 交互 </summary>
            Interaction,
            /// <summary> 显示/隐藏HUD </summary>
            ShowOrHideHUD,
            /// <summary> 暂停/返回 </summary>
            PauseOrBack,
        }

        /// <summary> 获取游戏键位的映射 </summary>
        public static KeyCode[] GetKeymapGroup(GameKey key) => Keymap.CurrentKeymap[(short)key];

        /// <summary>
        /// 设置游戏键位的按键映射 
        /// <para> 4个按键参数均为null时，则重置为默认映射 </para>
        /// </summary>
        /// <param Name="key"> 要映射的游戏键位 </param>
        /// <param Name="keyboard1"> 键鼠按键1（null则不改变） </param>
        /// <param Name="keyboard2"> 键鼠按键2（null则不改变） </param>
        /// <param Name="keyboard1"> 手柄按键1（null则不改变） </param>
        /// <param Name="keyboard2"> 手柄按键2（null则不改变） </param>
        public static void SetKeymapGroup(GameKey key, KeyCode? keyboard1 = null, KeyCode? keyboard2 = null, KeyCode? gamepad1 = null, KeyCode? gamepad2 = null)
        {
            short keyID = (short)key;
            if (keyboard1.HasValue || keyboard2.HasValue)
            {
                if (keyboard1.HasValue) Keymap.CurrentKeymap[keyID][0] = keyboard1.Value;
                if (keyboard2.HasValue) Keymap.CurrentKeymap[keyID][1] = keyboard2.Value;
                if (gamepad1.HasValue) Keymap.CurrentKeymap[keyID][2] = gamepad1.Value;
                if (gamepad2.HasValue) Keymap.CurrentKeymap[keyID][3] = gamepad2.Value;
            }
            else Keymap.CurrentKeymap[keyID] = Keymap.DefaultKeymap[keyID];
        }

        /// <summary> 查询某个按键映射到的所有游戏键位 </summary>
        /// <param Name="key"> 按键 </param>
        public static GameKey[] GetGameKeys(KeyCode key)
        {
            List<int> list = new List<int>();
            GameKey[] res = null;
            for (int i = 0; i < Keymap.CurrentKeymap.Length; i++)
                if (Keymap.CurrentKeymap[i].Contains(key)) list.Add(i);
            res = new GameKey[list.Count];
            for (int i = 0; i < res.Length; i++)
                res[i] = (GameKey)list[i];
            return res;
        }
    }
}