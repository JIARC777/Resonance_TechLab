Shader "Depth Mask" {
    SubShader {
        Tags {"Queue" = "Geometry-80" }       
        Lighting Off
        ZTest LEqual
        ZWrite On
        ColorMask R
        Pass {
            Stencil{
                Ref 1
                Comp always
                Pass replace
                Fail zero
                ZFail zero
            }
        }
    }
} 