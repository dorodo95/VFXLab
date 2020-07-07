Shader "Unlit/Billboard"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _OffsetPivot ("Offset From Pivot", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "DisableBatching"="True"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OffsetPivot;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                v.vertex.y -= _OffsetPivot;
                float3 worldPos = mul( (float3x3)unity_ObjectToWorld, v.vertex.xyz);

                float3 worldCoord = float3(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23);
                

                float4 cameraPos = mul(UNITY_MATRIX_V, float4 (worldCoord,1)) + float4(worldPos,0);

                o.vertex = mul(UNITY_MATRIX_P, cameraPos);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}
