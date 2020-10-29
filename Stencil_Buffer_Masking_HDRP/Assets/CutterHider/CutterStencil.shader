Shader "Depth Mask" {
    SubShader {
        Tags {"Queue" = "Geometry-80" }       
        Lighting Off
        ZTest LEqual
        ZWrite On
        ColorMask A
        Pass {
            Stencil{
                Ref 2
                Comp lequal
                Pass replace
                Fail zero
                ZFail zero
            }
        }
    }
} 