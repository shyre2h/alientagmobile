Shader "Custom/Rim"
{
    Properties
    {
        _RimGradient ("Rim Color Gradient", 2D) = "white" {}
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    }

    SubShader
    {
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input
        {
            float3 viewDir;
        };

        sampler2D _RimGradient;
        float _RimPower;

        void surf(Input IN, inout SurfaceOutput o)
        {
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

            // Sample the gradient texture for rim colors
            half4 rimColors = tex2D(_RimGradient, float2(rim, 0));

            // Calculate the final rim color by blending the sampled colors
            half3 finalRimColor = rimColors.rgb;

            // Apply rim power
            finalRimColor = pow(finalRimColor, _RimPower);

            o.Emission = finalRimColor;
        }
        ENDCG
    }
    Fallback "Diffuse"
}