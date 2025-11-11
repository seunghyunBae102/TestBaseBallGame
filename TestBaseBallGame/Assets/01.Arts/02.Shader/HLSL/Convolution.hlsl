// Shader Graph Custom Function 전용: include 불필요

inline float2 Nani_ClampUV(float2 uv, float2 texel)
{
    return clamp(uv, texel * 0.5, 1.0 - texel * 0.5);
}

// 3x3 Convolution (고정 커널)
// Inputs (순서 유지):
//   Texture2D SourceTex, SamplerState SourceSamp,
//   float2 UV, float2 TexelSize,
//   float3 Row0, float3 Row1, float3 Row2,
//   float Scale, float Bias
// Output:
//   out float4 OutColor
void Nani_Convolution3x3(
    Texture2D SourceTex, SamplerState SourceSamp,
    float2 UV, float2 TexelSize,
    float3 Row0, float3 Row1, float3 Row2,
    float Scale, float Bias,
    out float4 OutColor)
{
    float2 uv0 = Nani_ClampUV(UV + TexelSize * float2(-1, -1), TexelSize);
    float2 uv1 = Nani_ClampUV(UV + TexelSize * float2(0, -1), TexelSize);
    float2 uv2 = Nani_ClampUV(UV + TexelSize * float2(1, -1), TexelSize);
    float2 uv3 = Nani_ClampUV(UV + TexelSize * float2(-1, 0), TexelSize);
    float2 uv4 = Nani_ClampUV(UV, TexelSize);
    float2 uv5 = Nani_ClampUV(UV + TexelSize * float2(1, 0), TexelSize);
    float2 uv6 = Nani_ClampUV(UV + TexelSize * float2(-1, 1), TexelSize);
    float2 uv7 = Nani_ClampUV(UV + TexelSize * float2(0, 1), TexelSize);
    float2 uv8 = Nani_ClampUV(UV + TexelSize * float2(1, 1), TexelSize);

    float3 s0 = SourceTex.Sample(SourceSamp, uv0).rgb;
    float3 s1 = SourceTex.Sample(SourceSamp, uv1).rgb;
    float3 s2 = SourceTex.Sample(SourceSamp, uv2).rgb;
    float3 s3 = SourceTex.Sample(SourceSamp, uv3).rgb;
    float4 s4a = SourceTex.Sample(SourceSamp, uv4);
    float3 s4 = s4a.rgb;
    float3 s5 = SourceTex.Sample(SourceSamp, uv5).rgb;
    float3 s6 = SourceTex.Sample(SourceSamp, uv6).rgb;
    float3 s7 = SourceTex.Sample(SourceSamp, uv7).rgb;
    float3 s8 = SourceTex.Sample(SourceSamp, uv8).rgb;

    float3 sum =
        s0 * Row0.x + s1 * Row0.y + s2 * Row0.z +
        s3 * Row1.x + s4 * Row1.y + s5 * Row1.z +
        s6 * Row2.x + s7 * Row2.y + s8 * Row2.z;

    sum = sum * Scale + Bias;
    OutColor = float4(sum, s4a.a); // 알파는 중앙 샘플 유지
}

// 1D Separable Convolution (가우시안 등)
float Nani_GetWeight(int idx, float4 W0, float4 W1, float4 W2, float4 W3)
{
    if (idx < 4)
        return W0[idx];
    if (idx < 8)
        return W1[idx - 4];
    if (idx < 12)
        return W2[idx - 8];
    return W3[idx - 12];
}

void Nani_Convolution1D(
    Texture2D SourceTex, SamplerState SourceSamp,
    float2 UV, float2 TexelSize,
    float2 Dir, int Len,
    float4 W0, float4 W1, float4 W2, float4 W3,
    out float4 OutColor)
{
    float3 acc = 0;
    float wsum = 0;

    [loop]
    for (int k = 0; k < Len; k++)
    {
        float w = Nani_GetWeight(k, W0, W1, W2, W3);
        float2 o = (k - (Len - 1) * 0.5) * Dir * TexelSize;
        float2 suv = Nani_ClampUV(UV + o, TexelSize);
        float3 c = SourceTex.Sample(SourceSamp, suv).rgb;
        acc += c * w;
        wsum += w;
    }

    float3 rgb = acc / max(wsum, 1e-5);
    float a = SourceTex.Sample(SourceSamp, UV).a;
    OutColor = float4(rgb, a);
}
