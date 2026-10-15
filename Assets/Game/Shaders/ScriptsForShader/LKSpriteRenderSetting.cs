using UnityEngine;

[ExecuteInEditMode]
public class LKSpriteRenderSetting : MonoBehaviour
{
    public Color color = Color.white;   
    [SerializeField] private bool _outline = true;
    private SpriteRenderer spriteRenderer;

    // 属性访问器
    public bool Outline
    {
        get { return _outline; }
        set
        {
            if (_outline != value)
            {
                _outline = value;
                UpdateOutline(_outline);
            }
        }
    }

    // 切换描边状态
    public void ToggleOutline()
    {
        Outline = !Outline;
    }

    // 设置描边状态
    public void SetOutline(bool enable)
    {
        Outline = enable;
    }

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateOutline(_outline);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(_outline);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
