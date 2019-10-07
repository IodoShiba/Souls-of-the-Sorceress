Shader "Custom/FadeShader"
{
	Properties{
		_Map("Map",2D) = "white" {}
		_MainTex("MainTex",2D) = "white" {}
		_BorderS("BorderS",Range(0,1))=0
		_BorderE("BorderE",Range(0,1))=1
	}
    SubShader {
        Pass {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment frag
			
			sampler2D _Map;
			float _BorderS;
			float _BorderE;

            fixed4 frag(v2f_img i) : COLOR 
			{
				fixed4 c = tex2D (_Map, i.uv);
				return fixed4(c.r,c.g,c.b ,0.5);
            }
            ENDCG
        }
    }
}
