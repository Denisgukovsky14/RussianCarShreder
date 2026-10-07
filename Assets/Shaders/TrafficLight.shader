Shader "Custom/TrafficLight"
{
    Properties
    {
        _ColorRed ("Red Color", Color) = (1,0,0,1)
        _ColorYellow ("Yellow Color", Color) = (1,1,0,1)
        _ColorGreen ("Green Color", Color) = (0,1,0,1)
        _CurrentState ("Current State", Float) = 0.0
        _BlinkSpeed ("Blink Speed", Float) = 1.0
        _IsBlinking ("Is Blinking", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _ColorRed;
            fixed4 _ColorYellow;
            fixed4 _ColorGreen;
            float _CurrentState;
            float _BlinkSpeed;
            float _IsBlinking;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 finalColor;
                
                // перенести логику смены цвета в vert, это ускорит рендер

                // Выбор цвета в зависимости от состояния
                if (_CurrentState < 0.5) {
                    finalColor = _ColorRed;
                } else if (_CurrentState < 1.5) {
                    finalColor = _ColorYellow;
                } else {
                    finalColor = _ColorGreen;
                }
                
                // Мигание (если включено)
                if (_IsBlinking > 0.5) {
                    float blink = (sin(_Time.y * _BlinkSpeed) + 1.0) * 0.5;
                    finalColor *= blink;
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Diffuse"
}