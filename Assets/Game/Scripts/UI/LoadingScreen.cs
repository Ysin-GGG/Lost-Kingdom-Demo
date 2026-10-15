using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using LostKingdom.Manager;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class LoadingScreen : LKBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image progressBarBackground;
        [SerializeField] private Image progressBarFill;
        [SerializeField] private TextMeshProUGUI tipsText;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Sprite[] backgroundImages;
        
        [Header("Progress Bar Settings")]
        [SerializeField] private float minFillAmount = 0.1f;
        [SerializeField] private float maxFillAmount = 1f;
        [SerializeField] private float animationSpeed = 1f;
        
        private float currentProgress = 0f;
        private bool isAnimating = false;

        protected override void Awake()
        {
            base.Awake();
            if (loadingCanvas != null)
            {
                loadingCanvas.gameObject.SetActive(false);
            }
        }
        
        protected override void Start()
        {
            base.Start();
            progressBarFill.fillAmount = 0f;
        }

        protected override bool OnActivate()
        {
            if (base.OnActivate())
            {
                if (loadingCanvas != null)
                {
                    loadingCanvas.gameObject.SetActive(true);
                }
                progressBarFill.fillAmount = 0f;
                isAnimating = false;
                currentProgress = 0f;

                if (backgroundImages != null && backgroundImages.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, backgroundImages.Length);
                    backgroundImage.sprite = backgroundImages[randomIndex];
                }

                return true;
            }
            return false;
        }

        protected override bool OnDeactivate()
        {
            if (base.OnDeactivate())
            {
                StopAllCoroutines();
                isAnimating = false;
                if (loadingCanvas != null)
                {
                    loadingCanvas.gameObject.SetActive(false);
                }
                return true;
            }
            return false;
        }

        public void SetProgress(float progress)
        {
            currentProgress = Mathf.Clamp01(progress);
            progressBarFill.fillAmount = currentProgress;
            
            if (currentProgress >= 1f)
            {
                progressBarFill.fillAmount = 1f;
                isAnimating = false;
                StopAllCoroutines();
            }
        }
        
        public void SetTip(string tip)
        {
            if (tipsText != null)
            {
                tipsText.text = tip;
            }
        }
        
        private IEnumerator AnimateLoadingBar()
        {
            if (isAnimating) yield break;
            isAnimating = true;
            
            float t = progressBarFill.fillAmount;
            while (t < 1f && isAnimating)
            {
                t += Time.unscaledDeltaTime * animationSpeed;
                progressBarFill.fillAmount = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            progressBarFill.fillAmount = 1f;
            isAnimating = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (InputManager.Instance != null)
            {
                InputManager.Instance.enabled = false;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (InputManager.Instance != null)
            {
                InputManager.Instance.enabled = true;
            }
        }
    }
} 