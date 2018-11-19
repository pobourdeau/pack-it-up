Shader "Custom/outlineObject"
{
	Properties
	{
		_MainTex("Main Texture (RGB)",2D) = "white" {}//Permet d'avoir une texture 
		_Color("Color",Color) = (1,1,1,1)//Permet de modifier la teinte

		_OutlineTex("Outline Texture (RGB)",2D) = "white" {}//Permet d'avoir une texture 
		_OutlineColor("Outline Color",Color) = (1,1,1,1)//Permet de modifier la teinte
		_OutlineWidth("Outline Width",Range(1.0,10.0))=1.1//Permet de modifier la teinte
	}

	SubShader
	{
		Tags{
			"Queue" = "Transparent"
		}

		Pass{
			Name "OUTLINE"

			ZWrite Off

			CGPROGRAM//Permet la communication entre le shader de unity et la carte graphique
				
				#pragma vertex vert//Contient l'information des vertexs

				#pragma fragment frag//Contient les informations des textures
			
			#include "UnityCG.cginc"

			//Donne l'information au vert
			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv: TEXCOORD0;
			};

			//Donne l'information au frag
			struct v2f
			{
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
			};


			//Reimporter les propriétés de unity la carte graphique

			float _OutlineWidth;
			float4 _OutlineColor;
			sampler2D _OutlineTex; 	
			
			

			//Fonction qui construit l'objet

			v2f vert(appdata IN)
			{
				IN.vertex.xyz *= _OutlineWidth;
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}

			//Colorier l'objet
			fixed4 frag(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_OutlineTex, IN.uv);
				return texColor * _OutlineColor;
			}

			ENDCG
		}

		Pass{
			Name "OBJECT"
			CGPROGRAM//Permet la communication entre le shader de unity et la carte graphique
				
				#pragma vertex vert//Contient l'information des vertexs

				#pragma fragment frag//Contient les informations des textures
			
			#include "UnityCG.cginc"

			//Donne l'information au vert
			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv: TEXCOORD0;
			};

			//Donne l'information au frag
			struct v2f
			{
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
			};


			//Reimporter les propriétés de unity la carte graphique

			float4 _Color;
			sampler2D _MainTex; 	
			
			

			//Fonction qui construit l'objet

			v2f vert(appdata IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}

			//Colorier l'objet
			fixed4 frag(v2f IN) : SV_Target
			{
				float4 texColor = tex2D(_MainTex, IN.uv);
				return texColor * _Color;
			}

			ENDCG
		}
	}
}
