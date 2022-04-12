// Colormaps texture generated using this python script:

// from PIL import Image
// import matplotlib.cm as cm
// import numpy as np
// names = ['viridis', 'plasma', 'inferno', 'jet', 'rainbow', 'coolwarm', 'flag', 'gray']
// def cm_array(m, size=256):
//     return cm.ScalarMappable(cmap=getattr(cm, m)).to_rgba(range(size), bytes=True).reshape(1, size, 4)
// Image.fromarray(np.vstack(map(cm_array, names)), mode='RGBA').save('colormaps.png')
// print ','.join(map(lambda x: x.title(), names))

Shader "Custom/DepthShader" {
	Properties {
		
		[PerRendererData]
		_MainTex ("MainTex", 2D) = "black" {}

		_DepthScale("Depth Multiplier Factor to Meters", float) = 0.001 
		_Detail("Level of detail", Range(1, 50)) = 10.0
		
		_MinRange("Min Range(m)", Range(0, 10)) = 0.15
		_MaxRange("Max Range(m)", Range(0, 20)) = 10.0


	}
	SubShader {
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }
		Pass {
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			
			float _MinRange;
			float _MaxRange;
			float _DepthScale;
			float _Detail;
			
			float samplePixel (v2f_img pix, float offset) {
				float z = tex2D(_MainTex, floor(pix.uv * _Detail)/_Detail + offset).r * 0xffff * _DepthScale;
				z = (z - _MinRange) / (_MaxRange - _MinRange);
				if(z <= 0)
					return 0;
				return step(0.1, 1 - (z / _MaxRange));
			}

			half4 frag (v2f_img pix) : SV_Target
			{
				//640 480
				// [0..1] -> ushort -> meters
				float2 rez = float2(640., 480.);
				float2 aPix = float2(1. / (rez.x/ _Detail / 4.0), 1. / (rez.y / _Detail / 4.0));
				//center
				float total = samplePixel(pix, 0);
				total+= samplePixel(pix, aPix.y);
				total+= samplePixel(pix, -aPix.y);
				total+= samplePixel(pix, aPix.x);
				total+= samplePixel(pix, -aPix.x);

				float average = total / 5.;
				return average;
			}
			ENDCG
		}
	}
	FallBack Off
}
