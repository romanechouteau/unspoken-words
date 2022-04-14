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
		_Detail("Level of detail", Integer) = 10

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
			int _Detail;

			float2 rez = float2(640., 480.);
			float2 pixel;
			float2 centerOffset;
			float2 quarterPixel;

			float samplePixel (v2f_img pix, float2 offset) {
					// rotate texture
					float2 uv = float2(1., 1.) - pix.uv;
					// remove detail to get origin of big pixel
					float2 origin = floor(uv * float(_Detail)) / float(_Detail);

					// get depth value at pixel position
					float z = tex2D(_MainTex, origin + centerOffset + offset * quarterPixel).r * 0xffff * _DepthScale;

					// apply min and max range
					z = (z - _MinRange) / (_MaxRange - _MinRange);

					if(z <= 0)
						return 0;

					// reverse color
					return step(0.1, 1. - z);
				}

			half4 frag (v2f_img pix) : SV_Target
			{
				// get pixel size
				pixel = float2((rez.x / float(_Detail)) / rez.x, (rez.y / float(_Detail)) / rez.y);
				centerOffset = pixel * 0.5;
				quarterPixel = pixel * 0.25;

				// get average color
				float total = samplePixel(pix, float2(0., 0.));
				total+= samplePixel(pix, float2(0., -1));
				total+= samplePixel(pix, float2(0., 1));
				total+= samplePixel(pix, float2(-1, 0.));
				total+= samplePixel(pix, float2(1, 0.));

				float average = total / 5.;
				return average;
			}
			ENDCG
		}
	}
	FallBack Off
}
