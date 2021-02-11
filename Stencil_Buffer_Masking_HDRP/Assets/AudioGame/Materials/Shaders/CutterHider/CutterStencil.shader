Shader "Depth Mask"
{
 

    SubShader
    {
        
        ///SBM--------
        Lighting Off
        ZTest lequal
        ZWrite Off
        ColorMask 0
        ///SBM-------
        

        Pass
        {
            ///SBM-------
            Stencil
            {
                Ref 32
                Comp always
                Pass replace
                Fail zero
                ZFail keep
            }
            ///SBM-------
            
            Name "ForwardOnly"
            Tags { "LightMode" = "ForwardOnly" }
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCINssgas

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL

        }

    }

}
