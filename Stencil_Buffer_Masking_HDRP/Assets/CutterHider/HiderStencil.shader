Shader "Hider" {
    SubShader {
        Tags { "Queue" = "Geometry-10" }       
        Lighting Off
        ZTest GEqual
        ZWrite Off

        ColorMask BA
        Pass {
            Stencil {
                Ref 2
                Comp equal
                Pass keep
                Fail zero
                ZFail keep
            }
        }
    }
}