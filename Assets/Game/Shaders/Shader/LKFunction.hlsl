float4 OutlineSeek(float4 color,float4 upColor,float4 downColor,float4 leftColor,float4 rightColor,float4 _OutlineColor,int _Outline)
{
    if (_Outline > 0 && color.a == 0) {
        if (upColor.a != 0 || downColor.a != 0 || leftColor.a != 0 || rightColor.a != 0) {
            color = _OutlineColor;
        }
    }
    return color;
}