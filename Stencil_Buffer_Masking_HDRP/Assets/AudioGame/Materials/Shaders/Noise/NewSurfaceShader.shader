Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct v2f {
            float4 position : SV_POSITION;
            float3 worldPos : TEXCOORD0;
            }

        v2f vert(appdata v) {
            v2f o;
            //calculate the position in clip space to render the object
            o.position = UnityObjectToClipPos(v.vertex);
            //calculate the position of the vertex in the world
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            return o;
        }

        fixed4 frag(v2f i) : SV_TARGET{
            //add different dimensions
            float chessboard = floor(i.worldPos.x);
            //divide it by 2 and get the fractional part, resulting in a value of 0 for even and 0.5 for odd numbers.
            chessboard = frac(chessboard * 0.5);
            //multiply it by 2 to make odd values white instead of grey
            chessboard *= 2;
            return chessboard;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
