Shader "Custom/Instance" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 _Color;
			sampler2D _MainTex;
			
			struct vsin {
				uint vid : SV_VertexID;
				uint iid : SV_InstanceID;
			};
			struct vs2ps {
				float4 vertex :POSITION;
				float2 uv : TEXCOORD0;
			};
			
			StructuredBuffer<float3> VertexBuf;
			StructuredBuffer<float3> PositionBuf;
			
			vs2ps vert(vsin IN) {
				float3 vertex = VertexBuf[IN.vid];
				vertex += PositionBuf[IN.iid];
				
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, float4(vertex, 1));
				OUT.uv = 0;
				return OUT;
			}
			
			float4 frag(vs2ps IN) : COLOR {
				return _Color * tex2D(_MainTex, IN.uv);
			}
			ENDCG
		}
	} 
	FallBack Off
}
