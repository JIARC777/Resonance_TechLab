Shader "Basic Info"
{


    SubShader
    {

        ///SBM--------
        Lighting Off
        ZTest greater
        ZWrite On //On: Make BG write to depth, could cause AO issues
        ColorMask 0
        Offset 1, 2
        ///SBM-------


        Pass
        {


            ///SBM-------
            Stencil
            {
                Ref 32
                Comp notequal
                Pass decrsat
                Fail keep //keep//What should happen to the made-visible hiders?
                ZFail decrsat
            }
            ///SBM-------




        }

    }


}