Shader "Depth Mask" {
    SubShader {
        Lighting Off
        ZTest lequal
        ZWrite Off
        ColorMask 0
        Pass {
            Stencil{
                Ref 32
                Comp always
                Pass replace
                Fail zero
                ZFail keep
            }
        }
    }
} 