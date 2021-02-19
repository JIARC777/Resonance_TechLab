Shader "Cutter"
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



 



        }

    }

}