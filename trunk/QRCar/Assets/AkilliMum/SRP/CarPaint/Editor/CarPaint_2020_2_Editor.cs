
#if UNITY_2020_2_OR_NEWER

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Rendering.Universal;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal.ShaderGUI;
//using UnityEditor.Rendering.Universal.ShaderGUI;

namespace AkilliMum.SRP.CarPaint
{
    internal class CarPaint_2020_2_Editor : BaseShaderGUI
    {

        private LitGUI.LitProperties litProperties;
        private LitDetailGUI.LitProperties litDetailProperties;
        private SavedBool m_DetailInputsFoldout;

        private bool MenuRotation = true;
        private bool MenuReflection = true;
        private bool MenuFresnel = true;
        private bool MenuFlakes = true;
        //private bool MenuSnow = true;
        //private bool MenuBottom = true;
        private bool MenuDecal = true;
        private bool MenuLivery = true;
        private bool MenuDirt = true;

        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {
            Material targetMat = materialEditorIn.target as Material;



            //todo:
            //MaterialProperty _EnableRotation = ShaderGUI.FindProperty("_EnableRotation", properties);

            //MenuRotation = EditorGUILayout.BeginFoldoutHeaderGroup(MenuRotation, new GUIContent { text = "Rotation" });

            //bool enableRotation = false;
            //if (MenuRotation)
            //{
            //    enableRotation = _EnableRotation.floatValue > 0.5f;
            //    enableRotation = EditorGUILayout.Toggle("Enable Rotation", enableRotation);
            //    _EnableRotation.floatValue = enableRotation ? 1.0f : 0.0f;

            //    if (enableRotation)
            //    {
            //        MaterialProperty rotation = ShaderGUI.FindProperty("_EnviRotation", properties);
            //        materialEditorIn.ShaderProperty(rotation, "Rotation");

            //        MaterialProperty position = ShaderGUI.FindProperty("_EnviPosition", properties);
            //        materialEditorIn.ShaderProperty(position, "Position Correction");

            //    }
            //}

            //EditorGUILayout.EndFoldoutHeaderGroup();

            //if (enableRotation)
            //    targetMat.EnableKeyword("_LCRS_PROBE_ROTATION");
            //else
            //    targetMat.DisableKeyword("_LCRS_PROBE_ROTATION");
            targetMat.DisableKeyword("_LCRS_PROBE_ROTATION");



            MaterialProperty _EnableRealTimeReflection = ShaderGUI.FindProperty("_EnableRealTimeReflection", properties);

            MenuReflection = EditorGUILayout.BeginFoldoutHeaderGroup(MenuReflection, new GUIContent { text = "Reflection" });

            bool realTimeRef = false;
            if (MenuReflection)
            {
                realTimeRef = _EnableRealTimeReflection.floatValue != 0.0f;
                realTimeRef = EditorGUILayout.Toggle("RealTime Reflection", realTimeRef);
                _EnableRealTimeReflection.floatValue = realTimeRef ? 1.0f : 0.0f;

                //if (realTimeRef)
                //{
                //    MaterialProperty _EnviCubeIntensity = ShaderGUI.FindProperty("_EnviCubeIntensity", properties);
                //    materialEditorIn.ShaderProperty(_EnviCubeIntensity, "Intensity");

                //    MaterialProperty _EnviCubeSmoothness = ShaderGUI.FindProperty("_EnviCubeSmoothness", properties);
                //    materialEditorIn.ShaderProperty(_EnviCubeSmoothness, "Smoothness");
                //}
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            //if (realTimeRef)
            //    targetMat.EnableKeyword("_REALTIMEREFLECTION");
            //else
            //    targetMat.DisableKeyword("_REALTIMEREFLECTION");



            MaterialProperty _LiveryMap = ShaderGUI.FindProperty("_LiveryMap", properties);

            MenuLivery = EditorGUILayout.BeginFoldoutHeaderGroup(MenuLivery, new GUIContent { text = "Livery" });
            if (MenuLivery)
            {
                materialEditorIn.TexturePropertySingleLine(
                    new GUIContent { text = "Map", tooltip = "Transparent top paint for liveries" }, _LiveryMap);

                if (_LiveryMap.textureValue != null)
                {
                    MaterialProperty _LiveryUsage = ShaderGUI.FindProperty("_LiveryUsage", properties);
                    _LiveryUsage.floatValue = 1;

                    MaterialProperty _LiveryUV = ShaderGUI.FindProperty("_LiveryUV", properties);
                    materialEditorIn.ShaderProperty(_LiveryUV, "UV");

                    MaterialProperty _LiveryTileOffset = ShaderGUI.FindProperty("_LiveryTileOffset", properties);
                    materialEditorIn.ShaderProperty(_LiveryTileOffset, "Tile and Offset");

                    MaterialProperty _LiveryColor = ShaderGUI.FindProperty("_LiveryColor", properties);
                    materialEditorIn.ColorProperty(_LiveryColor, "Color");

                    MaterialProperty _LiveryMetalic = ShaderGUI.FindProperty("_LiveryMetalic", properties);
                    materialEditorIn.ShaderProperty(_LiveryMetalic, "Metallic");

                    MaterialProperty _LiverySmoothness = ShaderGUI.FindProperty("_LiverySmoothness", properties);
                    materialEditorIn.ShaderProperty(_LiverySmoothness, "Smoothness");
                }
                else
                {
                    MaterialProperty _LiveryUsage = ShaderGUI.FindProperty("_LiveryUsage", properties);
                    _LiveryUsage.floatValue = 0;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_LiveryMap.textureValue)
            //    targetMat.EnableKeyword("_LIVERYMAP");
            //else
            //    targetMat.DisableKeyword("_LIVERYMAP");



            MaterialProperty _DecalMap = ShaderGUI.FindProperty("_DecalMap", properties);

            MenuDecal = EditorGUILayout.BeginFoldoutHeaderGroup(MenuDecal, new GUIContent { text = "Decal" });
            if (MenuDecal)
            {
                materialEditorIn.TexturePropertySingleLine(
                    new GUIContent { text = "Map", tooltip = "Transparent top paint for decals" }, _DecalMap);

                if (_DecalMap.textureValue != null)
                {
                    MaterialProperty _DecalUsage = ShaderGUI.FindProperty("_DecalUsage", properties);
                    _DecalUsage.floatValue = 1;

                    MaterialProperty _DecalUV = ShaderGUI.FindProperty("_DecalUV", properties);
                    materialEditorIn.ShaderProperty(_DecalUV, "UV");

                    MaterialProperty _DecalTileOffset = ShaderGUI.FindProperty("_DecalTileOffset", properties);
                    materialEditorIn.ShaderProperty(_DecalTileOffset, "Tile and Offset");

                    MaterialProperty _DecalColor = ShaderGUI.FindProperty("_DecalColor", properties);
                    materialEditorIn.ColorProperty(_DecalColor, "Color");

                    MaterialProperty _DecalMetalic = ShaderGUI.FindProperty("_DecalMetalic", properties);
                    materialEditorIn.ShaderProperty(_DecalMetalic, "Metallic");

                    MaterialProperty _DecalSmoothness = ShaderGUI.FindProperty("_DecalSmoothness", properties);
                    materialEditorIn.ShaderProperty(_DecalSmoothness, "Smoothness");
                }
                else
                {
                    MaterialProperty _DecalUsage = ShaderGUI.FindProperty("_DecalUsage", properties);
                    _DecalUsage.floatValue = 0;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_DecalMap.textureValue)
            //    targetMat.EnableKeyword("_DECALMAP");
            //else
            //    targetMat.DisableKeyword("_DECALMAP");



            MaterialProperty _DirtMap = ShaderGUI.FindProperty("_DirtMap", properties);

            MenuDirt = EditorGUILayout.BeginFoldoutHeaderGroup(MenuDirt, new GUIContent { text = "Dirt" });
            if (MenuDirt)
            {
                materialEditorIn.TexturePropertySingleLine(
                    new GUIContent { text = "Map", tooltip = "Transparent top paint for dirt" }, _DirtMap);

                if (_DirtMap.textureValue != null)
                {
                    MaterialProperty _DirtUsage = ShaderGUI.FindProperty("_DirtUsage", properties);
                    _DirtUsage.floatValue = 1;

                    MaterialProperty _DirtUV = ShaderGUI.FindProperty("_DirtUV", properties);
                    materialEditorIn.ShaderProperty(_DirtUV, "UV");

                    MaterialProperty _DirtTileOffset = ShaderGUI.FindProperty("_DirtTileOffset", properties);
                    materialEditorIn.ShaderProperty(_DirtTileOffset, "Tile and Offset");

                    MaterialProperty _DirtColor = ShaderGUI.FindProperty("_DirtColor", properties);
                    materialEditorIn.ColorProperty(_DirtColor, "Color");

                    MaterialProperty _DirtBumpMap = ShaderGUI.FindProperty("_DirtBumpMap", properties);
                    materialEditorIn.TexturePropertySingleLine(
                        new GUIContent { text = "Normal", tooltip = "Normal Map of Dirt" }, _DirtBumpMap);

                    MaterialProperty _DirtMapCutoff = ShaderGUI.FindProperty("_DirtMapCutoff", properties);
                    materialEditorIn.ShaderProperty(_DirtMapCutoff, "Cutoff");

                    MaterialProperty _DirtMetalic = ShaderGUI.FindProperty("_DirtMetalic", properties);
                    materialEditorIn.ShaderProperty(_DirtMetalic, "Metallic");

                    MaterialProperty _DirtSmoothness = ShaderGUI.FindProperty("_DirtSmoothness", properties);
                    materialEditorIn.ShaderProperty(_DirtSmoothness, "Smoothness");
                }
                else
                {
                    MaterialProperty _DirtUsage = ShaderGUI.FindProperty("_DirtUsage", properties);
                    _DirtUsage.floatValue = 0;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_DirtMap.textureValue)
            //    targetMat.EnableKeyword("_DIRTMAP");
            //else
            //    targetMat.DisableKeyword("_DIRTMAP");



            //!!!!!!!!!! Bottom is deprecated, i will use dirt!!!!!!!!!!!!!
            //MaterialProperty _BottomMap = ShaderGUI.FindProperty("_BottomMap", properties);

            //MenuBottom = EditorGUILayout.BeginFoldoutHeaderGroup(MenuBottom, new GUIContent { text = "Bottom" });
            //if (MenuBottom)
            //{
            //    materialEditorIn.TexturePropertySingleLine(
            //        new GUIContent { text = "Map", tooltip = "Bottom for Dirt 'like' Textures" }, _BottomMap);

            //    if (_BottomMap.textureValue != null)
            //    {
            //        MaterialProperty _BottomBumpMap = ShaderGUI.FindProperty("_BottomBumpMap", properties);
            //        materialEditorIn.TexturePropertySingleLine(
            //            new GUIContent { text = "Normal", tooltip = "Normal Map of Bottom" }, _BottomBumpMap);

            //        MaterialProperty _BottomMapDirection = ShaderGUI.FindProperty("_BottomMapDirection", properties);
            //        materialEditorIn.VectorProperty(_BottomMapDirection, "Direction");

            //        MaterialProperty _BottomMapHeight = ShaderGUI.FindProperty("_BottomMapHeight", properties);
            //        materialEditorIn.ShaderProperty(_BottomMapHeight, "Height");

            //        MaterialProperty _BottomMapPosition = ShaderGUI.FindProperty("_BottomMapPosition", properties);
            //        materialEditorIn.ShaderProperty(_BottomMapPosition, "Position");

            //        MaterialProperty _BottomMapCutoff = ShaderGUI.FindProperty("_BottomMapCutoff", properties);
            //        materialEditorIn.ShaderProperty(_BottomMapCutoff, "Cutoff");

            //        MaterialProperty _BottomMapScale = ShaderGUI.FindProperty("_BottomMapScale", properties);
            //        materialEditorIn.ShaderProperty(_BottomMapScale, "Scale");
            //    }
            //}
            //EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_BottomMap.textureValue)
            //    targetMat.EnableKeyword("_BOTTOMMAP");
            //else
            //    targetMat.DisableKeyword("_BOTTOMMAP");



            //MaterialProperty _SnowMap = ShaderGUI.FindProperty("_SnowMap", properties);

            //MenuSnow = EditorGUILayout.BeginFoldoutHeaderGroup(MenuSnow, new GUIContent { text = "Snow" });
            //if (MenuSnow)
            //{
            //    materialEditor.TexturePropertySingleLine(
            //        new GUIContent { text = "Map", tooltip = "Snow Map Texture" }, _SnowMap);

            //    MaterialProperty _SnowBumpMap = ShaderGUI.FindProperty("_SnowBumpMap", properties);
            //    materialEditor.TexturePropertySingleLine(
            //        new GUIContent { text = "Snow Bump", tooltip = "Normal Map of Snow" }, _SnowBumpMap);

            //    MaterialProperty _SnowAlphaMap = ShaderGUI.FindProperty("_SnowAlphaMap", properties);
            //    materialEditor.TexturePropertySingleLine(
            //        new GUIContent { text = "Snow Alpha", tooltip = "Alpha Map of Snow" }, _SnowAlphaMap);

            //    MaterialProperty _SnowColor = ShaderGUI.FindProperty("_SnowColor", properties);
            //    materialEditor.ColorProperty(_SnowColor, "Color");

            //    MaterialProperty _SnowDirection = ShaderGUI.FindProperty("_SnowDirection", properties);
            //    materialEditor.VectorProperty(_SnowDirection, "Direction");

            //    MaterialProperty _SnowLevel = ShaderGUI.FindProperty("_SnowLevel", properties);
            //    materialEditor.ShaderProperty(_SnowLevel, "Level");
            //}
            //EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_SnowMap.textureValue)
            //    targetMat.EnableKeyword("_SNOWMAP");
            //else
            //    targetMat.DisableKeyword("_SNOWMAP");



            MaterialProperty _FlakesBumpMap = ShaderGUI.FindProperty("_FlakesBumpMap", properties);

            MenuFlakes = EditorGUILayout.BeginFoldoutHeaderGroup(MenuFlakes, new GUIContent { text = "Flakes" });
            if (MenuFlakes)
            {
                materialEditorIn.TexturePropertySingleLine(
                    new GUIContent { text = "Map", tooltip = "Normal Map of Flakes" }, _FlakesBumpMap);

                if (_FlakesBumpMap.textureValue != null)
                {
                    MaterialProperty _FlakesUsage = ShaderGUI.FindProperty("_FlakesUsage", properties);
                    _FlakesUsage.floatValue = 1;

                    MaterialProperty _FlakesBumpMapScale = ShaderGUI.FindProperty("_FlakesBumpMapScale", properties);
                    materialEditorIn.ShaderProperty(_FlakesBumpMapScale, "Scale");

                    MaterialProperty _FlakesBumpStrength = ShaderGUI.FindProperty("_FlakesBumpStrength", properties);
                    materialEditorIn.ShaderProperty(_FlakesBumpStrength, "Strength");
                }
                else
                {
                    MaterialProperty _FlakesUsage = ShaderGUI.FindProperty("_FlakesUsage", properties);
                    _FlakesUsage.floatValue = 0;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //if (_FlakesBumpMap.textureValue)
            //    targetMat.EnableKeyword("_FLAKENORMAL");
            //else
            //    targetMat.DisableKeyword("_FLAKENORMAL");



            MenuFresnel = EditorGUILayout.BeginFoldoutHeaderGroup(MenuFresnel, new GUIContent { text = "Fresnel" });
            if (MenuFresnel)
            {
                MaterialProperty _FresnelColor = ShaderGUI.FindProperty("_FresnelColor", properties);
                materialEditorIn.ColorProperty(_FresnelColor, "Color 1");
                MaterialProperty _FresnelColor2 = ShaderGUI.FindProperty("_FresnelColor2", properties);
                materialEditorIn.ColorProperty(_FresnelColor2, "Color 2");
                //materialEditorIn.ShaderProperty(_FresnelColor, "Color");

                MaterialProperty _FresnelPower = ShaderGUI.FindProperty("_FresnelPower", properties);
                materialEditorIn.ShaderProperty(_FresnelPower, "Power");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //call base!
            base.OnGUI(materialEditorIn, properties);

        }

#if UNITY_2021_2_OR_NEWER
        //?
#else
        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);
            m_DetailInputsFoldout = new SavedBool($"{headerStateKey}.DetailInputsFoldout", true);
        }
#endif

#if UNITY_2021_2_OR_NEWER
        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(LitDetailGUI.Styles.detailInputs, Expandable.Details, _ => LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor));
        }
#else
        public override void DrawAdditionalFoldouts(Material material)
        {
            m_DetailInputsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_DetailInputsFoldout.value, LitDetailGUI.Styles.detailInputs);
            if (m_DetailInputsFoldout.value)
            {
                LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
#endif

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            litDetailProperties = new LitDetailGUI.LitProperties(properties);
        }

#if UNITY_2021_2_OR_NEWER
        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }
#else
        // material changed check
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }
        public void ValidateMaterial(Material material)
        {
            //just wrote it to disable compiler error over 2021.2 :)
        }
#endif

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            if (litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, Enum.GetNames(typeof(LitGUI.WorkflowMode)));
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                {
                    MaterialChanged((Material)obj);
                    ValidateMaterial((Material)obj);
                }
            }
            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if (EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                    ValidateMaterial(material);
                }
            }

            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
#if UNITY_2021_2_OR_NEWER
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
#endif
            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            MaterialChanged(material);
            ValidateMaterial(material);
        }
    }

    internal class LitDetailGUI
    {
        public static class Styles
        {
            public static readonly GUIContent detailInputs = new GUIContent("Detail Inputs",
                "These settings let you add details to the surface.");

            public static readonly GUIContent detailMaskText = new GUIContent("Mask",
                "Select a mask for the Detail maps. The mask uses the alpha channel of the selected texture. The __Tiling__ and __Offset__ settings have no effect on the mask.");

            public static readonly GUIContent detailAlbedoMapText = new GUIContent("Base Map",
                "Select the texture containing the surface details.");

            public static readonly GUIContent detailNormalMapText = new GUIContent("Normal Map",
                "Select the texture containing the normal vector data.");

            public static readonly GUIContent detailAlbedoMapScaleInfo = new GUIContent("Setting the scaling factor to a value other than 1 results in a less performant shader variant.");
        }

        public struct LitProperties
        {
            public MaterialProperty detailMask;
            public MaterialProperty detailAlbedoMapScale;
            public MaterialProperty detailAlbedoMap;
            public MaterialProperty detailNormalMapScale;
            public MaterialProperty detailNormalMap;

            public LitProperties(MaterialProperty[] properties)
            {
                detailMask = BaseShaderGUI.FindProperty("_DetailMask", properties, false);
                detailAlbedoMapScale = BaseShaderGUI.FindProperty("_DetailAlbedoMapScale", properties, false);
                detailAlbedoMap = BaseShaderGUI.FindProperty("_DetailAlbedoMap", properties, false);
                detailNormalMapScale = BaseShaderGUI.FindProperty("_DetailNormalMapScale", properties, false);
                detailNormalMap = BaseShaderGUI.FindProperty("_DetailNormalMap", properties, false);
            }
        }

        public static void DoDetailArea(LitProperties properties, MaterialEditor materialEditor)
        {
            materialEditor.TexturePropertySingleLine(Styles.detailMaskText, properties.detailMask);
            materialEditor.TexturePropertySingleLine(Styles.detailAlbedoMapText, properties.detailAlbedoMap,
                properties.detailAlbedoMap.textureValue != null ? properties.detailAlbedoMapScale : null);
            if (properties.detailAlbedoMapScale.floatValue != 1.0f)
            {
                EditorGUILayout.HelpBox(Styles.detailAlbedoMapScaleInfo.text, MessageType.Info, true);
            }
            materialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, properties.detailNormalMap,
                properties.detailNormalMap.textureValue != null ? properties.detailNormalMapScale : null);
            materialEditor.TextureScaleOffsetProperty(properties.detailAlbedoMap);
        }

        public static void SetMaterialKeywords(Material material)
        {
            if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailNormalMap") && material.HasProperty("_DetailAlbedoMapScale"))
            {
                bool isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
                bool hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
                CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
                CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
            }
        }
    }

    internal class SavedBool
    {
        private bool m_Value;
        private string m_Name;

        public bool value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                if (this.m_Value == value)
                    return;
                this.m_Value = value;
                EditorPrefs.SetBool(this.m_Name, value);
            }
        }

        public SavedBool(string name, bool value)
        {
            this.m_Name = name;
            this.m_Value = EditorPrefs.GetBool(name, value);
        }

        public static implicit operator bool(SavedBool s)
        {
            return s.value;
        }
    }
}

#endif