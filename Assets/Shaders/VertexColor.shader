Shader "Debug/Vertex color"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque"  "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
		    Name "ForwardLit"
            Tags { "LightMode" = "LightweightForward"}
			            // Use same blending / depth states as Standard shader

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //#pragma multi_compile_instancing

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 color: COLOR0;
				float3 normal : NORMAL;
			};

            struct v2f
            {
                float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0; // diffuse lighting color
                //UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };
			/*
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
           */
            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0 * v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
           
            fixed4 frag(v2f i) : SV_Target
            {
                //UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.
                return i.diff;
            }
            ENDCG
        }
    }
}