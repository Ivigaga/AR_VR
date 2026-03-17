Shader "Custom/ChromaKey" {
 
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    // He cambiado el nombre de la variable a _KeyColor para evitar conflictos
    _KeyColor ("Key Color (Color a eliminar)", Color) = (0,1,0,1) 
    _Sensitivity ("Threshold (Umbral)", Range(0, 1.0)) = 0.25
    _Smoothness ("Smoothing (Suavizado)", Range(0, 1.0)) = 0.1
}
 
SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    LOD 200
 
    CGPROGRAM
    #pragma surface surf Lambert alpha:fade
 
    sampler2D _MainTex;
    // Aquí también actualizamos el nombre de la variable
    float4 _KeyColor;
    float _Sensitivity;
    float _Smoothness;
 
    struct Input {
        float2 uv_MainTex;
    };
 
    void surf (Input IN, inout SurfaceOutput o) {
        half4 c = tex2D (_MainTex, IN.uv_MainTex);
        
        // Calculamos la distancia respecto a _KeyColor en vez de _Color
        float distanceToKey = distance(c.rgb, _KeyColor.rgb);
 
        float alphaVal = smoothstep(_Sensitivity, _Sensitivity + _Smoothness, distanceToKey);
 
        o.Emission = c.rgb;
        o.Alpha = c.a * alphaVal;
    }
    ENDCG
}
FallBack "Diffuse"
}