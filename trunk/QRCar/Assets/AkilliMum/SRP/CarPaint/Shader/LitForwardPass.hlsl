//URP Car Paint

#ifndef UNIVERSAL_FORWARD_LIT_PASS_INCLUDED
#define UNIVERSAL_FORWARD_LIT_PASS_INCLUDED
  
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#if UNITY_VERSION >= 202020
    // GLES2 has limited amount of interpolators
    #if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
    #define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
    #endif

    #if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
    #define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
    #endif
#endif

// keep this file in sync with LitGBufferPass.hlsl

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
    float2 texcoord1    : TEXCOORD2;
    float2 texcoord2    : TEXCOORD3;
    float2 texcoord3    : TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
#if UNITY_VERSION >= 202020

        float2 uv                       : TEXCOORD0;
        DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        float3 positionWS               : TEXCOORD2;
    #endif

        float3 normalWS                 : TEXCOORD3;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
        float4 tangentWS                : TEXCOORD4;    // xyz: tangent, w: sign
    #endif
        float3 viewDirWS                : TEXCOORD5;

        half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light 

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD7;
    #endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        float3 viewDirTS                : TEXCOORD8;
    #endif

        //also must be defined on unityverison>202020!!!!!!!!!!
        float3 objectPos                : TEXCOORD9;
        float3 objectPosWSRotated       : TEXCOORD10; //our rotated WS object position to sample cube

        //for other UV's
        float2 uv1                       : TEXCOORD11;
        float2 uv2                       : TEXCOORD12;
        float2 uv3                       : TEXCOORD13;

        float4 positionCS               : SV_POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO

#else

    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

#if UNITY_VERSION >= 201936
    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        float3 positionWS               : TEXCOORD2;
    #endif
#else
    #ifdef _ADDITIONAL_LIGHTS
        float3 positionWS               : TEXCOORD2;
    #endif
#endif

#ifdef _NORMALMAP
    float4 normalWS                 : TEXCOORD3;    // xyz: normal, w: viewDir.x
    float4 tangentWS                : TEXCOORD4;    // xyz: tangent, w: viewDir.y
    float4 bitangentWS              : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
#else
    float3 normalWS                 : TEXCOORD3;
#endif

    float3 viewDirWS                : TEXCOORD6;

    half4 fogFactorAndVertexLight   : TEXCOORD7; // x: fogFactor, yzw: vertex light

#if UNITY_VERSION >= 201936
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord          : TEXCOORD8;
    #endif
#else
    #ifdef _MAIN_LIGHT_SHADOWS
        float4 shadowCoord          : TEXCOORD8;
    #endif
#endif

    //also must be defined on unityverison>202020!!!!!!!!!!
    float3 objectPos                : TEXCOORD9;
    float3 objectPosWSRotated       : TEXCOORD10; //our rotated WS object position to sample cube
    
    //for other UV's
    float2 uv1                       : TEXCOORD11;
    float2 uv2                       : TEXCOORD12;
    float2 uv3                       : TEXCOORD13;

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO

#endif

};

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;

#if UNITY_VERSION >= 201936
    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        inputData.positionWS = input.positionWS;
    #endif
#else
    #ifdef _ADDITIONAL_LIGHTS
        inputData.positionWS = input.positionWS;
    #endif
#endif

#if UNITY_VERSION >= 202020
    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    #if defined(_NORMALMAP) || defined(_DETAIL)
        float sgn = input.tangentWS.w;      // should be either +1 or -1
        float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
        inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
    #else
        inputData.normalWS = input.normalWS;
    #endif

        inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
        inputData.viewDirectionWS = viewDirWS;
#else
    #ifdef _NORMALMAP
        half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
        inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
    #else
        half3 viewDirWS = input.viewDirWS;
        inputData.normalWS = input.normalWS;
    #endif

        inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
        viewDirWS = SafeNormalize(viewDirWS);

        inputData.viewDirectionWS = viewDirWS;
#endif



#if UNITY_VERSION >= 201936
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif
#else
    #if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
        inputData.shadowCoord = input.shadowCoord;
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif
#endif

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
#if UNITY_VERSION >= 202020
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
#endif
}

void Unity_RotateAboutAxis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
{
    Rotation = radians(Rotation);
    float s = sin(Rotation);
    float c = cos(Rotation);
    float one_minus_c = 1.0 - c;

    Axis = normalize(Axis);
    float3x3 rot_mat =
    { one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
        one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
        one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
    };
    Out = mul(rot_mat, In);
}

//rotates the point according to our rotation input and returns result
float3 CreateRotation(float3 toRotate) {
    float3 output = toRotate;

    if (_EnviRotation.x != 0)
        Unity_RotateAboutAxis_Degrees_float(output, float3 (1, 0, 0), -_EnviRotation.x, output);
    if (_EnviRotation.y != 0)
        Unity_RotateAboutAxis_Degrees_float(output, float3 (0, 1, 0), -_EnviRotation.y, output);
    if (_EnviRotation.z != 0)
        Unity_RotateAboutAxis_Degrees_float(output, float3 (0, 0, 1), -_EnviRotation.z, output);

    return output;
}

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Physically Based) shader
Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //float4 posWorld = mul(unity_ObjectToWorld, input.positionOS.xyz);
    // #if UNITY_REQUIRE_FRAG_WORLDPOS
    //     #if UNITY_PACK_WORLDPOS_WITH_TANGENT
    //         o.tangentToWorldAndPackedData[0].w = posWorld.x;
    //         o.tangentToWorldAndPackedData[1].w = posWorld.y;
    //         o.tangentToWorldAndPackedData[2].w = posWorld.z;
    //     #else
    //         o.posWorld = posWorld.xyz;
    //     #endif
    // #endif
    // o.pos = UnityObjectToClipPos(v.vertex);
    // o.tex = TexCoords(v);
    //output.eyeVec = NormalizeNormalPerVertex(posWorld.xyz - _WorldSpaceCameraPos);
 
    ////rotate the item on objects space (we will use only for ref probe sampling) according to room rotation    
    //VertexPositionInputs vertexInputRotated = GetVertexPositionInputs(CreateRotation(input.positionOS.xyz));
    ////VertexNormalInputs normalInputRotated = GetVertexNormalInputs(CreateRotation(input.normalOS), input.tangentOS);
    ////use fake WS for ref probe sampling
    //output.objectPosWSRotated = vertexInputRotated.positionWS;
    /*half3 viewDirWSRotated = GetCameraPositionWS() - vertexInputRotated.positionWS;
#ifdef _NORMALMAP
    output.normalWSRotated = half4(normalInputRotated.normalWS, viewDirWSRotated.x);
#else
    output.normalWSRotated = NormalizeNormalPerVertex(normalInputRotated.normalWS);
#endif
    output.viewDirWSRotated = viewDirWSRotated;*/


    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.objectPosWSRotated = vertexInput.positionWS;

#ifdef _LCRS_PROBE_ROTATION
    float3 ObjectPosition = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).xyz;
    ObjectPosition += _EnviPosition;    
    output.objectPosWSRotated -= ObjectPosition;    
    output.objectPosWSRotated = CreateRotation(output.objectPosWSRotated);
    output.objectPosWSRotated += ObjectPosition;
#endif

#if UNITY_VERSION >= 202020
    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
#else
    half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
#endif
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.uv1 = TRANSFORM_TEX(
        _DecalUV == 0 ? input.texcoord * _DecalTileOffset.xy + _DecalTileOffset.zw :
        (_DecalUV == 1 ? input.texcoord1 * _DecalTileOffset.xy + _DecalTileOffset.zw :
            (_DecalUV == 2 ? input.texcoord2 * _DecalTileOffset.xy + _DecalTileOffset.zw : 
                input.texcoord3 * _DecalTileOffset.xy + _DecalTileOffset.zw)), _DecalMap);
    output.uv2 = TRANSFORM_TEX(
        _LiveryUV == 0 ? input.texcoord * _LiveryTileOffset.xy + _LiveryTileOffset.zw :
        (_LiveryUV == 1 ? input.texcoord1 * _LiveryTileOffset.xy + _LiveryTileOffset.zw :
            (_LiveryUV == 2 ? input.texcoord2 * _LiveryTileOffset.xy + _LiveryTileOffset.zw : 
                input.texcoord3 * _LiveryTileOffset.xy + _LiveryTileOffset.zw)), _LiveryMap);
    output.uv3 = TRANSFORM_TEX(
        _DirtUV == 0 ? input.texcoord * _DirtTileOffset.xy + _DirtTileOffset.zw :
        (_DirtUV == 1 ? input.texcoord1 * _DirtTileOffset.xy + _DirtTileOffset.zw :
            (_DirtUV == 2 ? input.texcoord2 * _DirtTileOffset.xy + _DirtTileOffset.zw : 
                input.texcoord3 * _DirtTileOffset.xy + _DirtTileOffset.zw)), _DirtMap);

#if UNITY_VERSION >= 202020

    // already normalized from normal transform to WS.
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = viewDirWS;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
        output.tangentWS = tangentWS;
    #endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
        output.viewDirTS = viewDirTS;
    #endif

        OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
        OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

        output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        output.positionWS = vertexInput.positionWS;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

        output.positionCS = vertexInput.positionCS;

        output.objectPos = input.positionOS; //object space

        return output;

#else

    #ifdef _NORMALMAP
        output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
        output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
        output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
    #else
        output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    #endif

    output.viewDirWS = viewDirWS;
    
    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    #if UNITY_VERSION >= 201936
        #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
            output.positionWS = vertexInput.positionWS;
        #endif

        #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = GetShadowCoord(vertexInput);
        #endif
    #else
        #ifdef _ADDITIONAL_LIGHTS
            output.positionWS = vertexInput.positionWS;
        #endif

        #if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
            output.shadowCoord = GetShadowCoord(vertexInput);
        #endif
    #endif

    output.positionCS = vertexInput.positionCS;

    output.objectPos = input.positionOS; //object space
    
    return output;

#endif

}

float mod(float a, float b)
{
    return a - floor(a / b) * b;
}
float2 mod(float2 a, float2 b)
{
    return a - floor(a / b) * b;
}
float3 mod(float3 a, float3 b)
{
    return a - floor(a / b) * b;
}
float4 mod(float4 a, float4 b)
{
    return a - floor(a / b) * b;
} 

//#if defined(SHADER_API_OPENGL)  !defined(SHADER_TARGET_GLSL)
//#define UNITY_BUGGY_TEX2DPROJ4
//#define UNITY_PROJ_COORD(a) a.xyw
//#else
//#define UNITY_PROJ_COORD(a) a
//#endif

//  #define WorldNormalVector(data,normal) 
//     half3(dot(data.normalWS.x,normal), dot(data.normalWS.y,normal), dot(data.normalWS.z,normal))

float3 LocalCorrect(float3 origVec, float3 bboxMin, float3 bboxMax, float3 objectPosWSRotated, float3 cubemapPos)
{
    // Find the ray intersection with box plane
    float3 invOrigVec = float3(1.0, 1.0, 1.0) / origVec;
    
    float3 intersecAtMaxPlane = (bboxMax - objectPosWSRotated) * invOrigVec;

    float3 intersecAtMinPlane = (bboxMin - objectPosWSRotated) * invOrigVec;

    // Get the largest intersection values (we are not intersted in negative values)
    float3 largestIntersec = max(intersecAtMaxPlane, intersecAtMinPlane);

    // Get the closest of all solutions
    float Distance = min(min(largestIntersec.x, largestIntersec.y), largestIntersec.z);

    // Get the intersection position
    float3 IntersectPositionWS = objectPosWSRotated + origVec * Distance;

    // Get corrected vector
    float3 localCorrectedVec = IntersectPositionWS - cubemapPos;

    return localCorrectedVec;
}

half3 GlossyEnvironmentReflectionEx(half3 reflectVector, half perceptualRoughness, half occlusion,
    float3 localCorrReflDirWS, float mixMultiplier)
{
#if !defined(_ENVIRONMENTREFLECTIONS_OFF)

    half mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
    //half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip);
    half4 encodedIrradiance;
//#ifdef _REALTIMEREFLECTION
if(_EnableRealTimeReflection > 0.5)
{
    encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(_EnviCubeMapMain, sampler_EnviCubeMapMain, localCorrReflDirWS, mip);
    #ifdef _REALTIMEREFLECTION_MIX
        //change full black areas with other probes
        if(encodedIrradiance.r < 0.01 && encodedIrradiance.g < 0.01 && encodedIrradiance.b < 0.01){ 
            encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(_EnviCubeMapToMix1, sampler_EnviCubeMapToMix1, localCorrReflDirWS, mip) * mixMultiplier;
        }
    #endif
}
//#else
else
{
    encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, localCorrReflDirWS, mip);
}
//#endif


    //half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, rrr, mip);

//#if !defined(UNITY_USE_NATIVE_HDR)
//todo:! it was 
//#if defined(UNITY_USE_NATIVE_HDR) || defined(UNITY_DOTS_INSTANCING_ENABLED)
//on 202020 code! is that correct? There is no ! on HDR !!!!!
#if !defined(UNITY_USE_NATIVE_HDR) || defined(UNITY_DOTS_INSTANCING_ENABLED)

    half3 irradiance;
    //#ifdef _REALTIMEREFLECTION
    if (_EnableRealTimeReflection > 0.5)
    {
        irradiance = DecodeHDREnvironment(encodedIrradiance, _EnviCubeMapMain_HDR);
        #ifdef _REALTIMEREFLECTION_MIX
            //change full black areas with other probes
            if(irradiance.r < 0.01 && irradiance.g < 0.01 && irradiance.b < 0.01){
                irradiance = DecodeHDREnvironment(encodedIrradiance, _EnviCubeMapToMix1_HDR);
            }
        #endif
    }
    //#else
    else
    {
        irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
    }
    //#endif
    //half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);

    //half3 irradiance = DecodeHDREnvironment(encodedIrradiance, _EnviCubeMapBaked_HDR);

#else
    half3 irradiance = encodedIrradiance.rgb;
#endif

    return irradiance * occlusion;
#endif // GLOSSY_REFLECTIONS

    return _GlossyEnvironmentColor.rgb * occlusion;
}

    //old code! not using! mix this with above code!
    half3 GlossyEnvironmentReflectionExtended(half3 reflectVector, float3 localCorrReflDirWS, float mixMultiplier)
    {
        half4 color = SAMPLE_TEXTURECUBE_LOD(_EnviCubeMapMain, sampler_EnviCubeMapMain, localCorrReflDirWS, 1);
#ifdef _REALTIMEREFLECTION_MIX
        //change full black areas with other probes
        if(color.r < 0.01 && color.g < 0.01 && color.b < 0.01){ 
            color = SAMPLE_TEXTURECUBE_LOD(_EnviCubeMapToMix1, sampler_EnviCubeMapToMix1, localCorrReflDirWS, 1) * mixMultiplier;
        }
#endif
        return color.rgb;
    }

#if UNITY_VERSION >= 202020

    half3 GlobalIlluminationEx(BRDFData brdfData, BRDFData brdfDataClearCoat, float clearCoatMask,
        half3 bakedGI, half occlusion,
        half3 normalWS, half3 viewDirectionWS,
        float3 objectPosWSRotated, float3 bBoxMin, float3 bBoxMax, float3 enviCubeMapPos, float mixMultiplier)
    {
        half3 reflectVector = reflect(-viewDirectionWS, normalWS);

        #ifdef _LCRS_PROBE_ROTATION
            reflectVector = CreateRotation(reflectVector);
        #endif

        float3 localCorrReflDirWS = reflectVector;
        //find local correction if real time!
        //#ifdef _REALTIMEREFLECTION
        if (_EnableRealTimeReflection > 0.5)
        {
            localCorrReflDirWS = LocalCorrect(reflectVector, bBoxMin, bBoxMax, objectPosWSRotated, enviCubeMapPos);
        }
        //#endif

        half NoV = saturate(dot(normalWS, viewDirectionWS));
        half fresnelTerm = Pow4(1.0 - NoV);

        half3 indirectDiffuse = bakedGI * occlusion;
        //half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion);
        half3 indirectSpecular = GlossyEnvironmentReflectionEx(reflectVector, brdfData.perceptualRoughness, occlusion,
            localCorrReflDirWS, mixMultiplier);

        half3 color = EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);

        #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
            //half3 coatIndirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfDataClearCoat.perceptualRoughness, occlusion);
            half3 coatIndirectSpecular = GlossyEnvironmentReflectionEx(reflectVector, brdfDataClearCoat.perceptualRoughness, occlusion,
            localCorrReflDirWS, mixMultiplier);
            // TODO: "grazing term" causes problems on full roughness
            half3 coatColor = EnvironmentBRDFClearCoat(brdfDataClearCoat, clearCoatMask, coatIndirectSpecular, fresnelTerm);

            // Blend with base layer using khronos glTF recommended way using NoV
            // Smooth surface & "ambiguous" lighting
            // NOTE: fresnelTerm (above) is pow4 instead of pow5, but should be ok as blend weight.
            half coatFresnel = kDielectricSpec.x + kDielectricSpec.a * fresnelTerm;
            return color * (1.0 - coatFresnel * clearCoatMask) + coatColor;
        #else
            return color;
        #endif
    }

#else

    half3 GlobalIlluminationEx(BRDFData brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS,
        float3 objectPosWSRotated, float3 bBoxMin, float3 bBoxMax, float3 enviCubeMapPos, float mixMultiplier)
    {    
        half3 reflectVector = reflect(-viewDirectionWS, normalWS);

        #ifdef _LCRS_PROBE_ROTATION
            reflectVector = CreateRotation(reflectVector);
        #endif

        float3 localCorrReflDirWS = reflectVector;
        //find local correction if real time!
        //#ifdef _REALTIMEREFLECTION
        if (_EnableRealTimeReflection > 0.5)
        {
            localCorrReflDirWS = LocalCorrect(reflectVector, bBoxMin, bBoxMax, objectPosWSRotated, enviCubeMapPos);
        }
        //#endif

        //float3 localCorrReflDirWS = LocalCorrect(reflectVector, bBoxMin, bBoxMax, objectPosWSRotated, enviCubeMapPos);

        half fresnelTerm = Pow4(1.0 - saturate(dot(normalWS, viewDirectionWS)));

        half3 indirectDiffuse = bakedGI * occlusion;
        half3 indirectSpecular = GlossyEnvironmentReflectionEx(reflectVector, brdfData.perceptualRoughness, occlusion,
            localCorrReflDirWS, mixMultiplier);

        return EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);
    }

#endif



#if UNITY_VERSION >= 202020

    ///////////////////////////////////////////////////////////////////////////////
    //                      Fragment Functions                                   //
    //       Used by ShaderGraph and others builtin renderers                    //
    ///////////////////////////////////////////////////////////////////////////////
    half4 UniversalFragmentPBREx(InputData inputData, SurfaceData surfaceData,
        float3 objectPosWSRotated, float3 bBoxMin, float3 bBoxMax, float3 enviCubeMapPos, float mixMultiplier)
    {
    #ifdef _SPECULARHIGHLIGHTS_OFF
        bool specularHighlightsOff = true;
    #else
        bool specularHighlightsOff = false;
    #endif

        BRDFData brdfData;

        // NOTE: can modify alpha
        InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

        BRDFData brdfDataClearCoat = (BRDFData)0;
    #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
        // base brdfData is modified here, rely on the compiler to eliminate dead computation by InitializeBRDFData()
        InitializeBRDFDataClearCoat(surfaceData.clearCoatMask, surfaceData.clearCoatSmoothness, brdfData, brdfDataClearCoat);
    #endif

        // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
    #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
        half4 shadowMask = inputData.shadowMask;
    #elif !defined (LIGHTMAP_ON)
        half4 shadowMask = unity_ProbesOcclusion;
    #else
        half4 shadowMask = half4(1, 1, 1, 1);
    #endif

        Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

        #if defined(_SCREEN_SPACE_OCCLUSION)
            AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
            mainLight.color *= aoFactor.directAmbientOcclusion;
            surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
        #endif

        MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
        half3 color = GlobalIlluminationEx(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
                                        inputData.bakedGI, surfaceData.occlusion,
                                        inputData.normalWS, inputData.viewDirectionWS,
                                        objectPosWSRotated, bBoxMin, bBoxMax, enviCubeMapPos, mixMultiplier);
        color += LightingPhysicallyBased(brdfData, brdfDataClearCoat,
                                        mainLight,
                                        inputData.normalWS, inputData.viewDirectionWS,
                                        surfaceData.clearCoatMask, specularHighlightsOff);

    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
            #if defined(_SCREEN_SPACE_OCCLUSION)
                light.color *= aoFactor.directAmbientOcclusion;
            #endif
            color += LightingPhysicallyBased(brdfData, brdfDataClearCoat,
                                            light,
                                            inputData.normalWS, inputData.viewDirectionWS,
                                            surfaceData.clearCoatMask, specularHighlightsOff);
        }
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        color += inputData.vertexLighting * brdfData.diffuse;
    #endif

        color += surfaceData.emission;

        return half4(color, surfaceData.alpha);
    }

#else

    ///////////////////////////////////////////////////////////////////////////////
    //                      Fragment Functions                                   //
    //       Used by ShaderGraph and others builtin renderers                    //
    ///////////////////////////////////////////////////////////////////////////////
    half4 UniversalFragmentPBREx(InputData inputData, half3 albedo, half metallic, half3 specular,
        half smoothness, half occlusion, half3 emission, half alpha,
        float3 objectPosWSRotated, float3 bBoxMin, float3 bBoxMax, float3 enviCubeMapPos, float mixMultiplier)
    {
        BRDFData brdfData;
        InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);

        Light mainLight = GetMainLight(inputData.shadowCoord);
        MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

        half3 color = GlobalIlluminationEx(brdfData, inputData.bakedGI, occlusion, inputData.normalWS, inputData.viewDirectionWS,
            objectPosWSRotated, bBoxMin, bBoxMax, enviCubeMapPos, mixMultiplier);
        
        color += LightingPhysicallyBased(brdfData, mainLight, inputData.normalWS, inputData.viewDirectionWS);

    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
            color += LightingPhysicallyBased(brdfData, light, inputData.normalWS, inputData.viewDirectionWS);
        }
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        color += inputData.vertexLighting * brdfData.diffuse;
    #endif

        color += emission;
        return half4(color, alpha);
    }
#endif

    // half4 UniversalFragmentPBRExtended(InputData inputData, half alpha,
    //     float3 objectPos, float3 bBoxMin, float3 bBoxMax, float3 enviCubeMapPos, float mixMultiplier)
    // {
    //     half3 color = GlobalIlluminationExtended(inputData.normalWS, inputData.viewDirectionWS, 
    //         objectPos, bBoxMin, bBoxMax, enviCubeMapPos, mixMultiplier);
    
    //     return half4(color, alpha);
    // }


// Used in Standard (Physically Based) shader
half4 LitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#if UNITY_VERSION >= 202020
    #if defined(_PARALLAXMAP)
    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        half3 viewDirTS = input.viewDirTS;
    #else
        half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
    #endif
        ApplyPerPixelDisplacement(viewDirTS, input.uv);
    #endif
#endif

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.uv, surfaceData);

//half3 viewDir;
//#ifdef _NORMALMAP
//    viewDir = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
//#else 
//    viewDir = input.viewDirWS;
//#endif

    half3 outputNormal = surfaceData.normalTS;
//#ifdef _FLAKENORMAL
if(_FlakesUsage > 0.5)
{
    // Apply scaled flake normal map
    half3 flakeNormal = SampleNormal(input.uv * _FlakesBumpMapScale, 
        TEXTURE2D_ARGS(_FlakesBumpMap, sampler_FlakesBumpMap), _BumpScale);

    // Apply flake map strength
    half3 scaledFlakeNormal = flakeNormal;
    scaledFlakeNormal.xy *= _FlakesBumpStrength;
    scaledFlakeNormal.z = 0; // Z set to 0 for better blending with other normal map.

    // Blend regular normal map with flakes normal map
    outputNormal = normalize(outputNormal + scaledFlakeNormal);
}
//#endif
//#ifdef _DIRTMAP
if(_DirtUsage > 0.5)
{
    float4 dirty = SAMPLE_TEXTURE2D(_DirtMap, sampler_DirtMap, input.uv3);
    half dirtyAlpha = saturate(dirty.a * _DirtMapCutoff);
    half3 dirtyNormal = SampleNormal(input.uv3,
        TEXTURE2D_ARGS(_DirtBumpMap, sampler_DirtBumpMap), _BumpScale);
    //new normal will be directly dirt's normal, because it is another layer on top of everything
    outputNormal = normalize(lerp(outputNormal, dirtyNormal, dirtyAlpha));
}
//#endif
	surfaceData.normalTS = outputNormal; //it may changed above

    // Apply Fresnel
    float fresnel =  1.0 - max(dot(normalize(input.viewDirWS), input.normalWS), 0.0);
    fresnel = pow(fresnel, _FresnelPower);
    /*surfaceData.albedo = lerp(surfaceData.albedo, _FresnelColor.xyz, saturate(fresnel)); */

    //_FresnelColor.xyz = lerp(_FresnelColor2.xyz, _FresnelColor.xyz, saturate(fresnel));
    surfaceData.albedo = lerp(lerp(surfaceData.albedo, _FresnelColor2.xyz, saturate(fresnel)),
                              lerp(_FresnelColor2.xyz, _FresnelColor.xyz, saturate(fresnel)), saturate(fresnel));
    /*float fresnelSq = fresnel * fresnel;
    surfaceData.albedo = lerp(surfaceData.albedo, fresnel * _FresnelColor.xyz +
        fresnelSq * _FresnelColor2.xyz, saturate(fresnel));*/

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

#if UNITY_VERSION >= 202020
    half4 color = UniversalFragmentPBREx(inputData, surfaceData,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#else
    half4 color = UniversalFragmentPBREx(inputData, surfaceData.albedo, surfaceData.metallic,
        surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#endif


//mix below code above
//half4 color = UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, 
//        surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);

//!!!!! We are changing surface data with livery here be carefull if you will use it later!!!
//#ifdef _LIVERYMAP
if(_LiveryUsage > 0.5)
{ 
    float4 liveryTex = SAMPLE_TEXTURE2D(_LiveryMap, sampler_LiveryMap, input.uv2);
    surfaceData.albedo = liveryTex.xyz * _LiveryColor;
    surfaceData.metallic = _LiveryMetalic;
    surfaceData.smoothness = _LiverySmoothness;

#if UNITY_VERSION >= 202020
    half4 livery = UniversalFragmentPBREx(inputData, surfaceData,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#else
    half4 livery = UniversalFragmentPBREx(inputData, surfaceData.albedo, surfaceData.metallic,
        surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#endif
    color = lerp(color, livery, liveryTex.a);
}
//#endif

//!!!!! We are changing surface data with decal here be carefull if you will use it later!!!
//#ifdef _DECALMAP
if(_DecalUsage > 0.5)
{
    float4 decalTex = SAMPLE_TEXTURE2D(_DecalMap, sampler_DecalMap, input.uv1);
    surfaceData.albedo = decalTex.xyz * _DecalColor;
    surfaceData.metallic = _DecalMetalic;
    surfaceData.smoothness = _DecalSmoothness;

    #if UNITY_VERSION >= 202020
        half4 decal = UniversalFragmentPBREx(inputData, surfaceData,
            input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
    #else
        half4 decal = UniversalFragmentPBREx(inputData, surfaceData.albedo, surfaceData.metallic,
            surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha,
            input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
    #endif
    color = lerp(color, decal, decalTex.a);
}
//#endif

//#ifdef _REALTIMEREFLECTION
//    half4 colorRef = UniversalFragmentPBRExtended(inputData, surfaceData.alpha,
//        input.objectPosWS, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);

//    #ifdef _DECALMAP
//        color = lerp(color, colorRef, _EnviCubeIntensity*(1-decalAlbedo.a));
//    #else
//        color = lerp(color, colorRef, _EnviCubeIntensity);
//    #endif
//#endif


//because dirt is not attached to color it self (it is like another layer on object)
//we will change this final color (after metalic color etc.)
//#ifdef _DIRTMAP
if(_DirtUsage > 0.5)
{
    float4 dirt = SAMPLE_TEXTURE2D(_DirtMap, sampler_DirtMap, input.uv3);
    //normal will be changed before initialization, look into the befıre "InitializeInputData" function !!!!
    /*half3 dirtNormal = SampleNormal(input.uv3,
        TEXTURE2D_ARGS(_DirtBumpMap, sampler_DirtBumpMap), _BumpScale);
    */
    half dirtAlpha = saturate(dirt.a * _DirtMapCutoff);
    
    surfaceData.albedo = dirt.xyz * _DirtColor;
    surfaceData.metallic = _DirtMetalic;
    surfaceData.smoothness = _DirtSmoothness;
    
#if UNITY_VERSION >= 202020
    half4 dirtColor = UniversalFragmentPBREx(inputData, surfaceData,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#else
    half4 dirtColor = UniversalFragmentPBREx(inputData, surfaceData.albedo, surfaceData.metallic,
        surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha,
        input.objectPosWSRotated, _BBoxMin.xyz, _BBoxMax.xyz, _EnviCubeMapPos.xyz, _MixMultiplier);
#endif

    color.rgb = lerp(color.rgb, dirtColor.xyz, dirtAlpha);
}
//#endif

    color.rgb = MixFog(color.rgb, inputData.fogCoord);

#if UNITY_VERSION >= 202020
    color.a = OutputAlpha(color.a, _Surface);
#endif

    return color;
}

#endif
