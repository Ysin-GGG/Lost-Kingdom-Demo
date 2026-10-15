using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace LostKingdom.Manager
{
    public class SceneLoader : LKBehaviour
    {
        [SerializeField] private string[] loadingTips = new string[]
        {
            "天天摸鱼中...",
            "准备开始冒险...",
            "正在跨越埃尔瑞萨尔废墟...",
            "穿越耀光谷森林中...",
        };
        [SerializeField] private float minimumLoadingTime = 2f; // 最小加载时间
        
        private static SceneLoader instance;
        public static SceneLoader Instance => instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                var go = new GameObject("SceneLoader");
                instance = go.AddComponent<SceneLoader>();
                DontDestroyOnLoad(go);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("测试/加载界面/显示加载界面 %#l")]  // Ctrl+Shift+L
        private static void TestLoadingScreenMenuItem()
        {
            TestLoadingScreen();
        }
#endif

        /// <summary>
        /// 测试加载界面的方法
        /// </summary>
        public static void TestLoadingScreen()
        {
            if (instance != null)
            {
                instance.StartCoroutine(instance.SimulateLoading());
            }
            else
            {
                Debug.LogError("SceneLoader instance not found!");
            }
        }

        private IEnumerator SimulateLoading()
        {
            if (UIManager.LoadingScreen != null)
            {
                UIManager.LoadingScreen.Active = true;
                
                // 随机显示一个提示
                if (loadingTips != null && loadingTips.Length > 0)
                {
                    string randomTip = loadingTips[Random.Range(0, loadingTips.Length)];
                    UIManager.LoadingScreen.SetTip(randomTip);
                }

                float progress = 0f;
                float simulationTime = 0f;
                
                // 模拟加载过程
                while (simulationTime < minimumLoadingTime)
                {
                    simulationTime += Time.unscaledDeltaTime;  // 使用unscaledDeltaTime
                    progress = simulationTime / minimumLoadingTime;
                    UIManager.LoadingScreen.SetProgress(progress);
                    yield return null;
                }

                // 确保进度达到100%
                UIManager.LoadingScreen.SetProgress(1f);
                
                // 等待一小段时间展示完成动画
                yield return new WaitForSecondsRealtime(0.5f);  // 使用WaitForSecondsRealtime
                
                // 关闭加载界面
                UIManager.LoadingScreen.Active = false;
            }
            else
            {
                Debug.LogError("LoadingScreen reference not found in UIManager!");
            }
        }
        
        public static void LoadScene(string sceneName)
        {
            if (instance != null)
            {
                instance.StartCoroutine(instance.LoadSceneAsync(sceneName));
            }
            else
            {
                Debug.LogError("SceneLoader instance not found!");
            }
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // 确保加载界面已激活
            if (UIManager.LoadingScreen != null)
            {
                UIManager.LoadingScreen.Active = true;
                
                // 随机显示一个提示
                if (loadingTips != null && loadingTips.Length > 0)
                {
                    string randomTip = loadingTips[Random.Range(0, loadingTips.Length)];
                    UIManager.LoadingScreen.SetTip(randomTip);
                }
                
                float startTime = Time.time;
                
                // 开始异步加载场景
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                asyncLoad.allowSceneActivation = false;
                
                while (!asyncLoad.isDone)
                {
                    // 更新进度条
                    float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                    UIManager.LoadingScreen.SetProgress(progress);
                    
                    // 检查是否达到最小加载时间和90%加载进度
                    if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minimumLoadingTime)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                    
                    yield return null;
                }
                
                // 等待新场景完全加载
                yield return new WaitForSeconds(0.5f);
                
                // 关闭加载界面
                UIManager.LoadingScreen.Active = false;
            }
            else
            {
                Debug.LogError("LoadingScreen reference not found in UIManager!");
                // 直接加载场景
                SceneManager.LoadScene(sceneName);
            }
        }
    }
} 