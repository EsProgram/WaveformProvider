#ifndef WAVE_UTIL
#define WAVE_UTIL

#define WAVE_TEX_DEFINE(name) \
	sampler2D name; \
	float4 name##_ST; \
	float4 name##_TexelSize;

#define WAVE_HEIGHT(waveTex, uv) WaveHeight(waveTex, uv)
#define WAVE_NORMAL(waveTex, uv) WaveNormal(waveTex, uv, waveTex##_TexelSize, 1, 1)
#define WAVE_NORMAL_ADJ(waveTex, uv, parallaxScale, normalScale) WaveNormal(waveTex, uv, waveTex##_TexelSize, parallaxScale, normalScale)

float WaveHeight(sampler2D waveTex, float2 uv)
{
#if SHADER_TARGET < 30
	return tex2D(waveTex, uv).r * 2 - 1;
#else
	return tex2Dlod(waveTex, float4(uv, 0, 0)).r * 2 - 1;
#endif
}

float3 WaveNormal(sampler2D waveTex, float2 uv, float2 texelSize, float parallaxScale, float normalScale)
{
	float2 shiftX = { texelSize.x,  0 };
	float2 shiftZ = { 0, texelSize.y };
	shiftX *= parallaxScale * normalScale;
	shiftZ *= parallaxScale * normalScale;
	float3 texX = WAVE_HEIGHT(waveTex, uv.xy + shiftX);
	float3 texx = WAVE_HEIGHT(waveTex, uv.xy - shiftX);
	float3 texZ = WAVE_HEIGHT(waveTex, uv.xy + shiftZ);
	float3 texz = WAVE_HEIGHT(waveTex, uv.xy - shiftZ);
	float3 du = { 1, 0, normalScale * (texX.x - texx.x) };
	float3 dv = { 0, 1, normalScale * (texZ.x - texz.x) };
	return normalize(cross(du, dv));
}

#endif //WAVE_UTIL