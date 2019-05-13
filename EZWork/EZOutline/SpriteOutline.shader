Shader "EZShader/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" { }
        [MaterialToggle] _OutlineToggle ("Outline Toggle", Float) = 0
        _OutlineColor ("Outline Colour", Color) = (1, 1, 1, 1)
        // _TintColor ("Tint Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        
        Cull Off
        Lighting Off
        ZTest Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
                // Render 顶点颜色
                fixed4 color : COLOR;
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 uv: TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            sampler2D _MainTex;
            
            v2f vert(appdata v)
            {
                // 读取 Render 数据
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }
            
            fixed4 _OutlineColor;
            // Unity 内置变量，等价于 Vector4(1 / width, 1 / height, width, height)
            float4 _MainTex_TexelSize;
            fixed _OutlineToggle;

            fixed4 frag(v2f i): SV_Target
            {
                // 采样
                half4 c = tex2D(_MainTex, i.uv);
                // 注意： GPU是无记忆的，frag 是不断地在一遍遍重头执行的
                // 即：原始图片 c.a 是1的像素，在每次执行 frag 时，都是1。然后重复一遍遍处理
                // 这里缓存后，用于描边判断：实现原图淡出，描边仍然存在
                fixed cAlpha = c.a;
                c *= i.color;
                c.rgb *= c.a;

                if (_OutlineToggle == 0)
                    return c;

                // 描边处理 1: 边缘不透明部分直接加描边
                // 上边缘
                if(i.uv.y + _MainTex_TexelSize.y > 1 && cAlpha == 1)
                    return _OutlineColor;
                // 下边缘
                if(i.uv.y - _MainTex_TexelSize.y < 0 && cAlpha == 1)
                    return _OutlineColor;
                // 右边缘
                if(i.uv.x + _MainTex_TexelSize.x > 1 && cAlpha == 1)
                    return _OutlineColor;
                // 左边缘
                if(i.uv.x - _MainTex_TexelSize.x < 0 && cAlpha == 1)
                    return _OutlineColor;

                // 描边处理 2.1: 内部像素处理
                fixed upAlpha = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).a;
                fixed downAlpha = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).a;
                fixed rightAlpha = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).a;
                fixed leftAlpha = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).a;
                fixed mulAlpha = upAlpha * downAlpha * rightAlpha * leftAlpha;

                // 描边处理 2.2: 描边判断 TODO：尽量不要使用 if else
                _OutlineColor.rgb *= _OutlineColor.a;
                if(cAlpha ==  1) {
                    if(mulAlpha ==  1)
                        return c;
                    else 
                        return _OutlineColor;
                } else if(cAlpha == 0){
                    return c;
                } else {
                    return c;
                }
            }
            ENDCG
            
        }
    }
}
