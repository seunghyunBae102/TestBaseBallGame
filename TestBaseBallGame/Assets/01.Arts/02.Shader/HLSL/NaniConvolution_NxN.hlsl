// NaniConvolution_NxN.hlsl — AutoN + 정규화 모드 + signed 디코드 토글

inline float2 Nani_ClampUV(float2 uv, float2 texel)
{
    return clamp(uv, texel * 0.5, 1.0 - texel * 0.5);
}

// normalizeMode: 0=None, 1=Sum, 2=SumAbs
void Nani_ConvolutionKernelTex_Core_AutoN(
    Texture2D SourceTex, SamplerState SourceSamp,
    float2 UV, float2 TexelSize,
    Texture2D KernelTex, SamplerState KernelSamp,
    float Scale, float Bias,
    int normalizeMode, // 0,1,2
    int useSignedDecode, // 0/1  (signed PNG이면 1)
    out float4 OutColor)
{
    float kW, kH;
    KernelTex.GetDimensions(kW, kH);
    float N = (kW + 0.5) >0.5 ? 1:0;
    if (N != 0)
        N += 1;
    int halfK = (int)(N / 2);

    float2 kTexel = 1.0 / float2(kW, kH);
    float3 acc = 0;
    float wsum = 0;
    float wsumAbs = 0;

    [loop]
    for (int j = -halfK; j <= halfK; j++)
    {
        [loop]
        for (int i = -halfK; i <= halfK; i++)
        {
            float2 suv = Nani_ClampUV(UV + TexelSize * float2(i, j), TexelSize);
            float2 kUV = (float2(i + halfK, j + halfK) + 0.5) * kTexel;

            float w = KernelTex.Sample(KernelSamp, kUV).r;
            if (useSignedDecode != 0)
                w = w * 2.0 - 1.0;

            float3 c = SourceTex.Sample(SourceSamp, suv).rgb;
            acc += c * w;
            wsum += w;
            wsumAbs += abs(w);
        }
    }

    float norm = 1.0;
    if (normalizeMode == 1)
        norm = 1.0 / max(abs(wsum), 1e-6); // 합으로 정규화
    else if (normalizeMode == 2)
        norm = 1.0 / max(wsumAbs, 1e-6); // 절대합 정규화 (signed 커널용)
    // normalizeMode==0: 정규화 안 함

    float3 rgb = acc * (norm * Scale) + Bias;
    
    rgb = rgb * -1 + 1;
    
    float a = SourceTex.Sample(SourceSamp, UV).a;
    OutColor = float4(rgb, a);
}

// Use Pragmas = ON
void Nani_ConvolutionKernelTex_float(
    Texture2D SourceTex, SamplerState SourceSamp,
    float2 UV, float2 TexelSize,
    Texture2D KernelTex, SamplerState KernelSamp,
    float Scale, float Bias,
    float NormalizeMode, // 0,1,2
    float UseSignedDecode, // 0/1
    out float4 OutColor)
{
    Nani_ConvolutionKernelTex_Core_AutoN(
        SourceTex, SourceSamp, UV, TexelSize,
        KernelTex, KernelSamp, Scale, Bias,
        (int) (NormalizeMode + 0.5),
        (int) (UseSignedDecode + 0.5),
        OutColor);
}

void Nani_ConvolutionKernelTex_half(
    Texture2D SourceTex, SamplerState SourceSamp,
    half2 UV, half2 TexelSize,
    Texture2D KernelTex, SamplerState KernelSamp,
    half Scale, half Bias,
    half NormalizeMode,
    half UseSignedDecode,
    out half4 OutColor)
{
    float4 tmp;
    Nani_ConvolutionKernelTex_Core_AutoN(
        SourceTex, SourceSamp, (float2) UV, (float2) TexelSize,
        KernelTex, KernelSamp, (float) Scale, (float) Bias,
        (int) (NormalizeMode + 0.5h),
        (int) (UseSignedDecode + 0.5h),
        tmp);
    OutColor = (half4) tmp;
}
