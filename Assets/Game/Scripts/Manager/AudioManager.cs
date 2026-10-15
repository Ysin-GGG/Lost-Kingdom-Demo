using LostKingdom.DataStructure;
using LostKingdom.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.Manager
{
    /// <summary> 音频管理器 </summary>
    [DisallowMultipleComponent]
    public sealed class AudioManager : LKBehaviour
    {
        [SerializeField]
        private List<AudioClip> interactionAsset;
        [SerializeField]
        private List<AudioClip> musicAsset;
        [SerializeField]
        private float _musicProcess0 = 1f;
        [SerializeField]
        private float _musicProcess1 = 0f;
        [SerializeField]
        private float _musicVolume0 = 0f;
        [SerializeField]
        private float _musicVolume1 = 0f;
        [SerializeField]
        private float _musicFade = .005f;

        /// <summary> 音频资源最大滞留时长 </summary>
        public static long AudioUnusedDuration;
        public static AudioListener Listener;
        public static GameObject MusicPlayer { get; private set; }
        public static GameObject AmbientPlayer { get; private set; }
        public static GameObject SFXPlayer { get; private set; }
        public static GameObject ConstantPlayer { get; private set; }
        private static ObjectPool<AudioSource> Music;
        private static ObjectPool<AudioSource> SFX;
        private static ObjectPool<AudioSource> Ambient;
        private static Dictionary<AudioClip, AudioSource> ConstantAudioAsset;
        private static Dictionary<string, (long, AudioClip)> AudioAsset;
        private static LinkedList<(string, long)> AudioTimeLeft;

        private static AudioManager instance;
        /// <summary> 正在播放的音乐 </summary>
        private static AudioSource music0;
        /// <summary> 正在替换的音乐 </summary>
        private static AudioSource music1;
        /// <summary> 即将替换的音乐 </summary>
        private static AudioSource music2;
        private static float musicProcess0 { get => instance._musicProcess0; set => instance._musicProcess0 = value; }
        private static float musicProcess1 { get => instance._musicProcess1; set => instance._musicProcess1 = value; }
        private static float musicVolume0 { get => instance._musicVolume0; set => instance._musicVolume0 = value; }
        private static float musicVolume1 { get => instance._musicVolume1; set => instance._musicVolume1 = value; }
        private static float musicFade { get => instance._musicFade; set => instance._musicFade = value; }

    protected override void Awake()
        {
            AudioManager.instance = this;
            AudioManager.Listener = this.gameObject.AddComponent<AudioListener>();
            AudioManager.ConstantAudioAsset = new Dictionary<AudioClip, AudioSource>();
            AudioManager.AudioAsset = new Dictionary<string, (long, AudioClip)>();
            AudioManager.AudioTimeLeft = new LinkedList<(string, long)>();

            AudioManager.MusicPlayer = new GameObject(nameof(AudioManager.MusicPlayer));
            AudioManager.AmbientPlayer = new GameObject(nameof(AudioManager.AmbientPlayer));
            AudioManager.SFXPlayer = new GameObject(nameof(AudioManager.SFXPlayer));
            AudioManager.ConstantPlayer = new GameObject(nameof(AudioManager.ConstantPlayer));

            AudioManager.Music = new ObjectPool<AudioSource>(new Func<int, AudioSource>((i) => AudioManager.AddSource(0)), 4, "Vanilla: Music Player");
            AudioManager.SFX = new ObjectPool<AudioSource>(new Func<int, AudioSource>((i) => AudioManager.AddSource(1)), 32, "Vanilla: SFX Player");
            AudioManager.Ambient = new ObjectPool<AudioSource>(new Func<int, AudioSource>((i) => AudioManager.AddSource(2)), 8, "Vanilla: Ambient Player");

            AudioManager.Music.OnRelease += new Action<int, AudioSource>((i, snd) => { snd.Stop(); snd.enabled = false; snd.clip = null; });
        }
        protected override void Start()
        {
            AudioManager.AudioUnusedDuration = 600_000_000; // 600s
            AudioManager.MusicPlayer.SetParent(this);
            AudioManager.AmbientPlayer.SetParent(this);
            AudioManager.SFXPlayer.SetParent(this);
            AudioManager.ConstantPlayer.SetParent(this);
        }

        protected override void FixedUpdate()
        {
            // 音乐过渡切换
            if (AudioManager.music0 != null) AudioManager.music0.volume = LKMath.Lerp(AudioManager.musicProcess0.Pow(2), 0f, AudioManager.musicVolume0);
            if (AudioManager.music1 != null) AudioManager.music1.volume = LKMath.Lerp(2 * AudioManager.musicProcess1 - AudioManager.musicProcess1.Pow(2), 0f, AudioManager.musicVolume1);
            if (AudioManager.music2 != null)
            {
                if (AudioManager.musicProcess1 > .1f)
                {
                    AudioManager.musicProcess0 = 1f.Min(AudioManager.musicProcess0 + AudioManager.musicFade);
                    AudioManager.musicProcess1 = 0f.Max(AudioManager.musicProcess1 - AudioManager.musicFade);
                }
                else
                {
                    if (AudioManager.music1 != null) AudioManager.Music.Release(AudioManager.music1, true);
                    AudioManager.music1 = AudioManager.music2;
                    AudioManager.music2 = null;
                    AudioManager.musicVolume0 = AudioManager.music0?.volume ?? 0f;
                    AudioManager.musicVolume1 = AudioManager.music1?.volume ?? 0f;
                    if (AudioManager.music1 != null)
                    {
                        AudioManager.music1.volume = 0f;
                        AudioManager.music1.Play();
                    }
                    AudioManager.musicProcess0 = 1f;
                    AudioManager.musicProcess1 = 0f;
                }
            }
            else if (AudioManager.music1 != null && (AudioManager.music0 == null || AudioManager.music0.clip != AudioManager.music1.clip))
            {
                if (AudioManager.musicProcess0 > .1f || AudioManager.musicProcess1 < 1f)
                {
                    AudioManager.musicProcess0 = 0f.Max(AudioManager.musicProcess0 - AudioManager.musicFade);
                    AudioManager.musicProcess1 = 1f.Min(AudioManager.musicProcess1 + AudioManager.musicFade);
                }
                else
                {
                    if (AudioManager.music0 != null) AudioManager.Music.Release(AudioManager.music0, true);
                    AudioManager.music0 = AudioManager.music1;
                    AudioManager.music1 = null;
                    AudioManager.musicVolume0 = AudioManager.musicVolume1;
                    AudioManager.musicVolume1 = 0f;
                    AudioManager.musicProcess0 = 1f;
                    AudioManager.musicProcess1 = 0f;
                }
            }
            // 每秒释放一次长时间未用的音频资源
            if (LK.LogicTimeLoop == 0)
            {
                int count = AudioManager.AudioTimeLeft.Count;
                long time = DateTime.Now.Ticks;
                LinkedListNode<(string, long)> node = AudioManager.AudioTimeLeft.First;
                while (node != null)
                {
                    string key = node.Value.Item1;
                    long value = node.Value.Item2;
                    if (time - value > AudioManager.AudioUnusedDuration)
                    {
                        if (AudioManager.AudioAsset[key].Item1 == value) // 释放
                        {
                            AudioManager.AudioAsset.Remove(key);
                            AudioManager.AudioTimeLeft.Remove(node);
                        }
                        else node.Value = (key, AudioManager.AudioAsset[key].Item1);
                    }
                    node = node.Next;
                }
            }
        }

        private static AudioSource AddSource(int type)
        {
            AudioSource source = null;
            switch (type)
            {
                case 0:
                    source = AudioManager.MusicPlayer.gameObject.AddComponent<AudioSource>();
                    break;
                case 1:
                    source = AudioManager.SFXPlayer.gameObject.AddComponent<AudioSource>();
                    break;
                case 2:
                    source = AudioManager.AmbientPlayer.gameObject.AddComponent<AudioSource>();
                    break;
            }
            if (source != null) source.enabled = false;
            return source;
        }

        /// <summary> 读取并播放音频（有生命周期） </summary>
        /// <param name="file"> 音频文件 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static AudioSource PlaySound(string file, float volume = 1f, float pitch = 1f, float pan = 0f)
        {
            long time = DateTime.Now.Ticks;
            if (!AudioManager.AudioAsset.ContainsKey(file))
            {
                AudioManager.AudioTimeLeft.AddLast((file, time));
                AudioManager.AudioAsset.Add(file, (time, LKAsset.Sound.GetAudioClip(file)));
            }
            AudioManager.AudioAsset[file] = (time, AudioManager.AudioAsset[file].Item2);
            return AudioManager.PlaySound(AudioManager.AudioAsset[file].Item2, volume, pitch, pan);
        }

        /// <summary> 播放音频（有生命周期） </summary>
        /// <param name="clip"> 音频资源 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static AudioSource PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, float pan = 0f)
        {
            AudioSource source = AudioManager.SFX.Request((int)(clip.length * 60));
            source.enabled = true;
            source.clip = clip;
            source.volume = volume * Options.SFXVolume * Options.MasterVolume;
            source.pitch = pitch;
            source.panStereo = pan;
            source.Play();
            return source;
        }

        /// <summary> 播放独立音频（不会重复播放） </summary>
        /// <param name="clip"> 音频资源 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static bool PlayConstSound(AudioClip clip, float volume = 1f, float pitch = 1f, float pan = 0f, bool replay = false)
        {
            AudioSource source = null;
            if(!AudioManager.ConstantAudioAsset.ContainsKey(clip)) // 初次播放，记入
            {
                source = AudioManager.ConstantPlayer.AddComponent<AudioSource>();
                source.clip = clip;
                AudioManager.ConstantAudioAsset.Add(clip, source);
            }
            else source = AudioManager.ConstantAudioAsset[clip];
            if (source.isPlaying && !replay) return false;
            source.enabled = true;
            source.volume = volume * Options.MasterVolume;
            source.pitch = pitch;
            source.panStereo = pan;
            source.Play();
            return true;
        }

        /// <summary> 播放BGM </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="pan"></param>
        /// <param name="replay"></param>
        /// <param name="loop"></param>
        public static void PlayMusic(AudioClip clip, float volume = 1f, float pitch = 1f, float pan = 0f, bool replay = false, bool loop = true)
        {
            AudioSource source = AudioManager.Music.Request();
            source.enabled = true;
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo = pan;
            source.loop = loop;
            if (AudioManager.music2 != null) AudioManager.Music.Release(AudioManager.music2);
            AudioManager.music2 = source;
        }

        /// <summary> 播放BGM </summary>
        /// <param name="type">编号</param>
        public static void PlayMusic(int type) => AudioManager.PlayMusic(AudioManager.instance.musicAsset[type]);

        /// <summary> 播放交互音效 </summary>
        /// <param name="type">编号</param>
        public static void PlayInteractionAsset(int type) => AudioManager.PlaySound(AudioManager.instance.interactionAsset[type]);
    }
}
