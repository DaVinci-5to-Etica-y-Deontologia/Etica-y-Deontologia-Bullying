Shader "Custom Shader/UserIcon"
{
    Properties
    {
        [MainTexture] [NoScaleOffset] _MainTex("_MainTex", 2D) = "white" {}
        [MainColor]_Color("_Color", Color) = (1, 1, 1, 1)
        _r("r", Color) = (0.4056604, 0, 0, 0)
        _g("g", Color) = (1, 0.9034402, 0, 0)
        _b("b", Color) = (1, 1, 1, 0)

        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        // DisableBatching: <None>
        "ShaderGraphShader" = "true"
        "ShaderGraphTargetId" = "UniversalSpriteUnlitSubTarget"
        }
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
    Pass
    {
        Name "Sprite Unlit"
        Tags
        {
            "LightMode" = "Universal2D"
        }

        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest [unity_GUIZTestMode]
        ZWrite Off

        ColorMask[_ColorMask]

        

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        #define ALPHA_CLIP_THRESHOLD 1


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _r;
        float4 _g;
        float4 _b;
        float _Stencil;
        float _StencilOp;
        float _StencilReadMask;
        float _StencilComp;
        float _StencilWriteMask;
        float _ColorMask;
        float4 _Color;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.tex, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.samplerstate, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.r;
                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.g;
                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.b;
                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.a;
                float4 _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4 = _r;
                float4 _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4;
                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float.xxxx), _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4, _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4);
                float4 _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4 = _g;
                float4 _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4;
                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float.xxxx), _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4, _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4);
                float4 _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4 = _b;
                float4 _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4;
                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float.xxxx), _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4, _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4);
                float4 _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4 = float4((_Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4).x, (_Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4).x, (_Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4).x, _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float);
                float4 _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4;
                Unity_Lerp_float4(float4(0, 0, 0, 0), _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4, (_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float.xxxx), _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4);
                float4 _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4 = _Color;
                float4 _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4;
                Unity_Multiply_float4_float4(_Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4, _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4, _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4);
                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_R_1_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[0];
                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_G_2_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[1];
                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_B_3_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[2];
                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[3];
                surface.BaseColor = (_Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4.xyz);
                surface.Alpha = _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float;
                surface.AlphaClipThreshold = 0.5;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif








                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
            Pass
            {
                Name "SceneSelectionPass"
                Tags
                {
                    "LightMode" = "SceneSelectionPass"
                }

                // Render State
                Cull Off

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag

                // Keywords
                // PassKeywords: <None>
                // GraphKeywords: <None>

                // Defines

                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_TEXCOORD0
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENESELECTIONPASS 1

                #define _ALPHATEST_ON 1


                // custom interpolator pre-include
                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                // Includes
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                // custom interpolators pre packing
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings(Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings(PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float4 _r;
                float4 _g;
                float4 _b;
                float4 _Color;
                CBUFFER_END


                    // Object and Global properties
                    SAMPLER(SamplerState_Linear_Repeat);
                    TEXTURE2D(_MainTex);
                    SAMPLER(sampler_MainTex);

                    // Graph Includes
                    // GraphIncludes: <None>

                    // -- Property used by ScenePickingPass
                    #ifdef SCENEPICKINGPASS
                    float4 _SelectionID;
                    #endif

                    // -- Properties used by SceneSelectionPass
                    #ifdef SCENESELECTIONPASS
                    int _ObjectId;
                    int _PassValue;
                    #endif

                    // Graph Functions

                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                    {
                        Out = A * B;
                    }

                    void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                    {
                        Out = lerp(A, B, T);
                    }

                    // Custom interpolators pre vertex
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                    // Graph Vertex
                    struct VertexDescription
                    {
                        float3 Position;
                        float3 Normal;
                        float3 Tangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        description.Position = IN.ObjectSpacePosition;
                        description.Normal = IN.ObjectSpaceNormal;
                        description.Tangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Custom interpolators, pre surface
                    #ifdef FEATURES_GRAPH_VERTEX
                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                    {
                    return output;
                    }
                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                    #endif

                    // Graph Pixel
                    struct SurfaceDescription
                    {
                        float Alpha;
                        float AlphaClipThreshold;
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        UnityTexture2D _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                        float4 _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.tex, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.samplerstate, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.r;
                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.g;
                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.b;
                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.a;
                        float4 _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4 = _r;
                        float4 _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4;
                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float.xxxx), _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4, _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4);
                        float4 _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4 = _g;
                        float4 _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4;
                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float.xxxx), _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4, _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4);
                        float4 _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4 = _b;
                        float4 _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4;
                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float.xxxx), _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4, _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4);
                        float4 _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4 = float4((_Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4).x, (_Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4).x, (_Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4).x, _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float);
                        float4 _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4;
                        Unity_Lerp_float4(float4(0, 0, 0, 0), _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4, (_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float.xxxx), _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4);
                        float4 _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4 = _Color;
                        float4 _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4;
                        Unity_Multiply_float4_float4(_Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4, _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4, _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4);
                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_R_1_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[0];
                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_G_2_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[1];
                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_B_3_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[2];
                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[3];
                        surface.Alpha = _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float;
                        surface.AlphaClipThreshold = 0.5;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs
                    #ifdef HAVE_VFX_MODIFICATION
                    #define VFX_SRP_ATTRIBUTES Attributes
                    #define VFX_SRP_VARYINGS Varyings
                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                    #endif
                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                        output.ObjectSpacePosition = input.positionOS;

                        return output;
                    }
                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                    #ifdef HAVE_VFX_MODIFICATION
                    #if VFX_USE_GRAPH_VALUES
                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                    #endif
                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                    #endif








                        #if UNITY_UV_STARTS_AT_TOP
                        #else
                        #endif


                        output.uv0 = input.texCoord0;
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                            return output;
                    }

                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                    // --------------------------------------------------
                    // Visual Effect Vertex Invocations
                    #ifdef HAVE_VFX_MODIFICATION
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                    #endif

                    ENDHLSL
                    }
                    Pass
                    {
                        Name "ScenePickingPass"
                        Tags
                        {
                            "LightMode" = "Picking"
                        }

                        // Render State
                        Cull Back

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        HLSLPROGRAM

                        // Pragmas
                        #pragma target 2.0
                        #pragma exclude_renderers d3d11_9x
                        #pragma vertex vert
                        #pragma fragment frag

                        // Keywords
                        // PassKeywords: <None>
                        // GraphKeywords: <None>

                        // Defines

                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define ATTRIBUTES_NEED_TEXCOORD0
                        #define VARYINGS_NEED_TEXCOORD0
                        #define FEATURES_GRAPH_VERTEX
                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                        #define SHADERPASS SHADERPASS_DEPTHONLY
                        #define SCENEPICKINGPASS 1

                        #define _ALPHATEST_ON 1


                        // custom interpolator pre-include
                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                        // Includes
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                        // --------------------------------------------------
                        // Structs and Packing

                        // custom interpolators pre packing
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                        struct Attributes
                        {
                             float3 positionOS : POSITION;
                             float3 normalOS : NORMAL;
                             float4 tangentOS : TANGENT;
                             float4 uv0 : TEXCOORD0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : INSTANCEID_SEMANTIC;
                            #endif
                        };
                        struct Varyings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };
                        struct SurfaceDescriptionInputs
                        {
                             float4 uv0;
                        };
                        struct VertexDescriptionInputs
                        {
                             float3 ObjectSpaceNormal;
                             float3 ObjectSpaceTangent;
                             float3 ObjectSpacePosition;
                        };
                        struct PackedVaryings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0 : INTERP0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };

                        PackedVaryings PackVaryings(Varyings input)
                        {
                            PackedVaryings output;
                            ZERO_INITIALIZE(PackedVaryings, output);
                            output.positionCS = input.positionCS;
                            output.texCoord0.xyzw = input.texCoord0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }

                        Varyings UnpackVaryings(PackedVaryings input)
                        {
                            Varyings output;
                            output.positionCS = input.positionCS;
                            output.texCoord0 = input.texCoord0.xyzw;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }


                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float4 _MainTex_TexelSize;
                        float4 _r;
                        float4 _g;
                        float4 _b;
                        float _Stencil;
                        float _StencilOp;
                        float _StencilReadMask;
                        float _StencilComp;
                        float _StencilWriteMask;
                        float _ColorMask;
                        float4 _Color;
                        CBUFFER_END


                            // Object and Global properties
                            SAMPLER(SamplerState_Linear_Repeat);
                            TEXTURE2D(_MainTex);
                            SAMPLER(sampler_MainTex);

                            // Graph Includes
                            // GraphIncludes: <None>

                            // -- Property used by ScenePickingPass
                            #ifdef SCENEPICKINGPASS
                            float4 _SelectionID;
                            #endif

                            // -- Properties used by SceneSelectionPass
                            #ifdef SCENESELECTIONPASS
                            int _ObjectId;
                            int _PassValue;
                            #endif

                            // Graph Functions

                            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                            {
                                Out = A * B;
                            }

                            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                            {
                                Out = lerp(A, B, T);
                            }

                            // Custom interpolators pre vertex
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                            // Graph Vertex
                            struct VertexDescription
                            {
                                float3 Position;
                                float3 Normal;
                                float3 Tangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                description.Position = IN.ObjectSpacePosition;
                                description.Normal = IN.ObjectSpaceNormal;
                                description.Tangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Custom interpolators, pre surface
                            #ifdef FEATURES_GRAPH_VERTEX
                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                            {
                            return output;
                            }
                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                            #endif

                            // Graph Pixel
                            struct SurfaceDescription
                            {
                                float Alpha;
                                float AlphaClipThreshold;
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                UnityTexture2D _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                                float4 _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.tex, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.samplerstate, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.r;
                                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.g;
                                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.b;
                                float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.a;
                                float4 _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4 = _r;
                                float4 _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4;
                                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float.xxxx), _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4, _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4);
                                float4 _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4 = _g;
                                float4 _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4;
                                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float.xxxx), _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4, _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4);
                                float4 _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4 = _b;
                                float4 _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4;
                                Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float.xxxx), _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4, _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4);
                                float4 _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4 = float4((_Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4).x, (_Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4).x, (_Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4).x, _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float);
                                float4 _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4;
                                Unity_Lerp_float4(float4(0, 0, 0, 0), _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4, (_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float.xxxx), _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4);
                                float4 _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4 = _Color;
                                float4 _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4;
                                Unity_Multiply_float4_float4(_Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4, _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4, _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4);
                                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_R_1_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[0];
                                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_G_2_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[1];
                                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_B_3_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[2];
                                float _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[3];
                                surface.Alpha = _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float;
                                surface.AlphaClipThreshold = 0.5;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs
                            #ifdef HAVE_VFX_MODIFICATION
                            #define VFX_SRP_ATTRIBUTES Attributes
                            #define VFX_SRP_VARYINGS Varyings
                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                            #endif
                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                output.ObjectSpacePosition = input.positionOS;

                                return output;
                            }
                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                            #ifdef HAVE_VFX_MODIFICATION
                            #if VFX_USE_GRAPH_VALUES
                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                            #endif
                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                            #endif








                                #if UNITY_UV_STARTS_AT_TOP
                                #else
                                #endif


                                output.uv0 = input.texCoord0;
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                    return output;
                            }

                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                            // --------------------------------------------------
                            // Visual Effect Vertex Invocations
                            #ifdef HAVE_VFX_MODIFICATION
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                            #endif

                            ENDHLSL
                            }
                            Pass
                            {
                                Name "Sprite Unlit"
                                Tags
                                {
                                    "LightMode" = "UniversalForward"
                                }

                                // Render State
                                Cull Off
                                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                                ZTest [unity_GUIZTestMode]
                                ZWrite Off

                                // Debug
                                // <None>

                                // --------------------------------------------------
                                // Pass

                                HLSLPROGRAM

                                // Pragmas
                                #pragma target 2.0
                                #pragma exclude_renderers d3d11_9x
                                #pragma vertex vert
                                #pragma fragment frag

                                // Keywords
                                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                                // GraphKeywords: <None>

                                // Defines

                                #define ATTRIBUTES_NEED_NORMAL
                                #define ATTRIBUTES_NEED_TANGENT
                                #define ATTRIBUTES_NEED_TEXCOORD0
                                #define ATTRIBUTES_NEED_COLOR
                                #define VARYINGS_NEED_POSITION_WS
                                #define VARYINGS_NEED_TEXCOORD0
                                #define VARYINGS_NEED_COLOR
                                #define FEATURES_GRAPH_VERTEX
                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                #define SHADERPASS SHADERPASS_SPRITEFORWARD


                                // custom interpolator pre-include
                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                // Includes
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                // --------------------------------------------------
                                // Structs and Packing

                                // custom interpolators pre packing
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                struct Attributes
                                {
                                     float3 positionOS : POSITION;
                                     float3 normalOS : NORMAL;
                                     float4 tangentOS : TANGENT;
                                     float4 uv0 : TEXCOORD0;
                                     float4 color : COLOR;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : INSTANCEID_SEMANTIC;
                                    #endif
                                };
                                struct Varyings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float3 positionWS;
                                     float4 texCoord0;
                                     float4 color;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };
                                struct SurfaceDescriptionInputs
                                {
                                     float4 uv0;
                                };
                                struct VertexDescriptionInputs
                                {
                                     float3 ObjectSpaceNormal;
                                     float3 ObjectSpaceTangent;
                                     float3 ObjectSpacePosition;
                                };
                                struct PackedVaryings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float4 texCoord0 : INTERP0;
                                     float4 color : INTERP1;
                                     float3 positionWS : INTERP2;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };

                                PackedVaryings PackVaryings(Varyings input)
                                {
                                    PackedVaryings output;
                                    ZERO_INITIALIZE(PackedVaryings, output);
                                    output.positionCS = input.positionCS;
                                    output.texCoord0.xyzw = input.texCoord0;
                                    output.color.xyzw = input.color;
                                    output.positionWS.xyz = input.positionWS;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }

                                Varyings UnpackVaryings(PackedVaryings input)
                                {
                                    Varyings output;
                                    output.positionCS = input.positionCS;
                                    output.texCoord0 = input.texCoord0.xyzw;
                                    output.color = input.color.xyzw;
                                    output.positionWS = input.positionWS.xyz;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }


                                // --------------------------------------------------
                                // Graph

                                // Graph Properties
                                CBUFFER_START(UnityPerMaterial)
                                float4 _MainTex_TexelSize;
                                float4 _r;
                                float4 _g;
                                float4 _b;
                                float _Stencil;
                                float _StencilOp;
                                float _StencilReadMask;
                                float _StencilComp;
                                float _StencilWriteMask;
                                float _ColorMask;
                                float4 _Color;
                                CBUFFER_END


                                    // Object and Global properties
                                    SAMPLER(SamplerState_Linear_Repeat);
                                    TEXTURE2D(_MainTex);
                                    SAMPLER(sampler_MainTex);

                                    // Graph Includes
                                    // GraphIncludes: <None>

                                    // -- Property used by ScenePickingPass
                                    #ifdef SCENEPICKINGPASS
                                    float4 _SelectionID;
                                    #endif

                                    // -- Properties used by SceneSelectionPass
                                    #ifdef SCENESELECTIONPASS
                                    int _ObjectId;
                                    int _PassValue;
                                    #endif

                                    // Graph Functions

                                    void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
                                    {
                                        Out = lerp(A, B, T);
                                    }

                                    // Custom interpolators pre vertex
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                    // Graph Vertex
                                    struct VertexDescription
                                    {
                                        float3 Position;
                                        float3 Normal;
                                        float3 Tangent;
                                    };

                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                    {
                                        VertexDescription description = (VertexDescription)0;
                                        description.Position = IN.ObjectSpacePosition;
                                        description.Normal = IN.ObjectSpaceNormal;
                                        description.Tangent = IN.ObjectSpaceTangent;
                                        return description;
                                    }

                                    // Custom interpolators, pre surface
                                    #ifdef FEATURES_GRAPH_VERTEX
                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                    {
                                    return output;
                                    }
                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                    #endif

                                    // Graph Pixel
                                    struct SurfaceDescription
                                    {
                                        float3 BaseColor;
                                        float Alpha;
                                        float AlphaClipThreshold;
                                    };

                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                    {
                                        SurfaceDescription surface = (SurfaceDescription)0;
                                        UnityTexture2D _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                                        float4 _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.tex, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.samplerstate, _Property_8c98c4dd1b5341ab8ed2216de7e1663f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.r;
                                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.g;
                                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.b;
                                        float _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float = _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_RGBA_0_Vector4.a;
                                        float4 _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4 = _r;
                                        float4 _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4;
                                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_R_4_Float.xxxx), _Property_81da68802d9a46fab0041e85947174b2_Out_0_Vector4, _Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4);
                                        float4 _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4 = _g;
                                        float4 _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4;
                                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_G_5_Float.xxxx), _Property_48ceefc2abbd461d95a698ba76c879bf_Out_0_Vector4, _Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4);
                                        float4 _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4 = _b;
                                        float4 _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4;
                                        Unity_Multiply_float4_float4((_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_B_6_Float.xxxx), _Property_0685a733e93c46b4a79f36f58be94704_Out_0_Vector4, _Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4);
                                        float4 _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4 = float4((_Multiply_344cb45df7044705847165ba32d30ee3_Out_2_Vector4).x, (_Multiply_ceaa050054b1450f84dafd9587e38abe_Out_2_Vector4).x, (_Multiply_98a208a921ea4685950f39a2b383a052_Out_2_Vector4).x, _SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float);
                                        float4 _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4;
                                        Unity_Lerp_float4(float4(0, 0, 0, 0), _Vector4_81175d24a8f54fcc83074b4ef5b8b813_Out_0_Vector4, (_SampleTexture2D_6ad0440a9c2944439019725bfdc4c0d2_A_7_Float.xxxx), _Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4);
                                        float4 _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4 = _Color;
                                        float4 _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4;
                                        Unity_Multiply_float4_float4(_Lerp_65d31fa3d0d7419ca04eeb6fae0bd0bd_Out_3_Vector4, _Property_2ad552252d3c47a8bcd93af2c0626de9_Out_0_Vector4, _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4);
                                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_R_1_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[0];
                                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_G_2_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[1];
                                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_B_3_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[2];
                                        float _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float = _Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4[3];
                                        surface.BaseColor = (_Multiply_4d2ae081708c4dc5912ddcc2c1305c18_Out_2_Vector4.xyz);
                                        surface.Alpha = _Split_fa64fe74a0544883ac9f11a4c5824cbf_A_4_Float;
                                        surface.AlphaClipThreshold = 0.5;
                                        return surface;
                                    }

                                    // --------------------------------------------------
                                    // Build Graph Inputs
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #define VFX_SRP_ATTRIBUTES Attributes
                                    #define VFX_SRP_VARYINGS Varyings
                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                    #endif
                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                    {
                                        VertexDescriptionInputs output;
                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                        output.ObjectSpaceNormal = input.normalOS;
                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                        output.ObjectSpacePosition = input.positionOS;

                                        return output;
                                    }
                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                    {
                                        SurfaceDescriptionInputs output;
                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    #ifdef HAVE_VFX_MODIFICATION
                                    #if VFX_USE_GRAPH_VALUES
                                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                    #endif
                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                    #endif








                                        #if UNITY_UV_STARTS_AT_TOP
                                        #else
                                        #endif


                                        output.uv0 = input.texCoord0;
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                    #else
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                    #endif
                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                            return output;
                                    }

                                    // --------------------------------------------------
                                    // Main

                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

                                    // --------------------------------------------------
                                    // Visual Effect Vertex Invocations
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                    #endif

                                    ENDHLSL
                                    }
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                        FallBack "Hidden/Shader Graph/FallbackError"
}