Shader "Hider" {
    SubShader {
        Tags { "Queue" = "Geometry-10" }       
        Lighting Off
        ZTest GEqual
        ZWrite Off
        ColorMask A
        Pass {
            Stencil {
                Ref 1
                Comp equal
                Pass keep
                Fail keep
                ZFail keep
            }
        }
    }
}