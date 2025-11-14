Shader "UI/JuliaFractal"
{
    Properties
    {
        _MaxIterations ("Max Iterations", Int) = 100
        _Zoom ("Zoom", Float) = 1.0
        _PanOffset ("Pan Offset", Vector) = (0, 0, 0, 0)
        _JuliaConstant ("Julia Constant", Vector) = (0, 0, 0, 0)
        _OutsideConstant ("Outside Constant", Vector) = (0, 0, 0, 1)
		_OutsideTrapGlowFactor("Outside Trap Glow Factor", Vector) = (0, 0, 0, 1)
		_OutsideDistanceFactor("Outside Distance Factor", Vector) = (0, 0, 0, 1)
		_OutsideDZEffectFactor("Outside DZ Effect Factor", Vector) = (0, 0, 0, 1)
		_OutsideVeinBoostFactor("Outside Vein Boost Factor", Vector) = (0, 0, 0, 1)
		_OutsideTFactor("Outside Tt Factor", Vector) = (0, 0, 0, 1)
		_InsideConstant("Inside Constant", Vector) = (0, 0, 0, 1)
		_InsideTrapValueFactor("Inside Trap Value Factor", Vector) = (0, 0, 0, 1)
		_InsideDZEffectFactor("Inside DZ Effect Factor", Vector) = (0, 0, 0, 1)
		_InsideDistanceFactor("Inside Distance Factor", Vector) = (0, 0, 0, 1)
		_InsideVeinBoostFactor("Inside Vein Boost Factor", Vector) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
			"Queue"="Transparent-100"
			"ForceNoShadowCasting"="True"
			"IgnoreProjector"="True"
			"PreviewType"="Plane"
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			ZWrite Off
			Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            int _MaxIterations;
            float _Zoom;
            float2 _PanOffset;
            float2 _JuliaConstant;
			float4 _OutsideConstant;
			float4 _OutsideTrapGlowFactor;
			float4 _OutsideDistanceFactor;
			float4 _OutsideDZEffectFactor;
			float4 _OutsideVeinBoostFactor;
			float4 _OutsideTFactor;
			float4 _InsideConstant;
			float4 _InsideTrapValueFactor;
			float4 _InsideDZEffectFactor;
			float4 _InsideDistanceFactor;
			float4 _InsideVeinBoostFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 c = (i.uv * 4.0 - 2.0) * float2(1.125, 1.0) * _Zoom + _PanOffset;
                float2 z = c;
                float minDistSq = 9999.0;
                float2 dz = float2(1.0, 1.0);
                int iterations = 0;

                while (iterations < _MaxIterations)
                {
                    float xSquared = z.x * z.x;
                    float ySquared = z.y * z.y;
                    float twoXY = 2.0 * z.x * z.y;
                    dz = float2(2.0 * (z.x * dz.x - z.y * dz.y), 2.0 * (z.x * dz.y + z.y * dz.x));
                    float distSq = xSquared + ySquared;
                    if (distSq < minDistSq) minDistSq = distSq;
                    z = float2(xSquared - ySquared, twoXY) + _JuliaConstant;
                    if (distSq > 4.0) break;
                    iterations++;
                }

                if (iterations == _MaxIterations)
                {
                    float veinBoost = 1.0 + (1.0 - length(_JuliaConstant));
                    float trapValue = saturate(1.0 / (minDistSq * 15.0 * veinBoost + 0.1));
                    float dzEffect = saturate(length(dz));
                    float distance = length(z);
                    return fixed4
					(
						_InsideConstant.x + trapValue * _InsideTrapValueFactor.x + dzEffect * _InsideDZEffectFactor.x + distance * _InsideDistanceFactor.x + veinBoost * _InsideVeinBoostFactor.x,
						_InsideConstant.y + trapValue * _InsideTrapValueFactor.y + dzEffect * _InsideDZEffectFactor.y + distance * _InsideDistanceFactor.y + veinBoost * _InsideVeinBoostFactor.y,
						_InsideConstant.z + trapValue * _InsideTrapValueFactor.z + dzEffect * _InsideDZEffectFactor.z + distance * _InsideDistanceFactor.z + veinBoost * _InsideVeinBoostFactor.z,
						1.0
                    );
                }
                else
                {
                    float t = (float)iterations / _MaxIterations;
                    float trapGlow = 1.0 / (minDistSq * 10.0 + 0.1);
                    float distance = length(z);
                    float dzEffect = saturate(length(dz));
                    float veinBoost = 1.0 + (1.0 - length(_JuliaConstant));
                    return fixed4
					(
                        _OutsideConstant.x + trapGlow * _OutsideTrapGlowFactor.x + distance * _OutsideDistanceFactor.x + dzEffect * _OutsideDZEffectFactor.x + veinBoost * _OutsideVeinBoostFactor.x + t * _OutsideTFactor.x,
						_OutsideConstant.y + trapGlow * _OutsideTrapGlowFactor.y + distance * _OutsideDistanceFactor.y + dzEffect * _OutsideDZEffectFactor.y + veinBoost * _OutsideVeinBoostFactor.y + t * _OutsideTFactor.y,
						_OutsideConstant.z + trapGlow * _OutsideTrapGlowFactor.z + distance * _OutsideDistanceFactor.z + dzEffect * _OutsideDZEffectFactor.z + veinBoost * _OutsideVeinBoostFactor.z + t * _OutsideTFactor.z,
						1.0
                    );
                }
            }
            ENDCG
        }
    }
}