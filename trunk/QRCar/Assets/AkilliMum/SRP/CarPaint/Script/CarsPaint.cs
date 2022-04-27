using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

// ReSharper disable CheckNamespace

namespace AkilliMum.SRP.CarPaint
{
    [ExecuteAlways]
    public class CarsPaint : MonoBehaviour
    {
        [Tooltip("Please use this to enable/disable the script. DO NOT USE script's enable/disable check box!")]
        public bool IsEnabled = true;

        [Tooltip("Debug the reflection!")]
        public bool IsDebug = false;

        //[Tooltip("Set this camera to your main cam to not draw the probe several times!")]
        //public Camera SceneCamera;

        [Tooltip("How to reflect the reflections? It is used to create a surrounding bounding box around the car to calculate the reflection space. Please try and select the best solution for your game!")]
        public BoundingBox BoundingBox = BoundingBox.Default;

        //[Range(0, 1)]
        //[Tooltip("Intensity of the reflection.")]
        //public float Intensity = 0.5f;

        [Range(1, 20)]
        [Tooltip("Runs the script for every Xth frame; you may gain the fps, but you will lose the reality (realtime) of reflection!")]
        public int RunForEveryXthFrame = 1;

        [Tooltip("Uncheck this if you suffer reflection glitches. Because camera may occlude some objects according to unity settings!")]
        public bool UseOcclusionCulling = true;

        [Tooltip("Enables the HDR, so post effects will be visible (like bloom) on the reflection.")]
        public bool HDR = false;
        private bool _oldHDR = false;

        //[Range(0, 8)]
        //[Tooltip("Smoothness of cubemap (mip level 0 to 8).")]
        //public float Smoothness = 0.0f;
        //private float _oldSmoothness = 0.0f;

        [Range(0, 10)]
        [Tooltip("Starts to drawing at this level of LOD. So if you do not creating perfect mirrors, you can use lower lods for surface and gain fps.")]
        public int CameraLODLevel = 0;

        [Tooltip("Disables the point and spot lights for the reflection. You may gain the fps, but you will lose the reality of reflection.")]
        public bool DisablePixelLights = false;
        private IList<SceneLights> _additionalLights;

        [Range(0, 1000)]
        [Tooltip("Shadow distance for reflection. 0 -> No shadows")]
        public float ShadowDistance = 0;

        [Tooltip("Select the reflection size! Most of the time 128 should do the job. Bigger values will impact the performance very much!")]
        public TextureSizeType TextureSize = TextureSizeType.x128;

        [Tooltip("Only these layers will be rendered by the reflection. So you can select what to be reflected with the reflection by putting them on certain layers (Do not forget to remove your car's layer too :))")]
        public LayerMask ReflectLayers = -1;

        [Tooltip("Clipping plane near variable for the render camera.")]
        public float ClippingPlaneNear = 0.3f;

        [Tooltip("Clipping plane far variable for the render camera.")]
        public float ClippingPlaneFar = 1000f;

        [Tooltip("Use this option if you only reflect specific layers (for example only near or moving objects) and want to get other reflections from static probes!")]
        public bool MixOtherProbes = false;
        private bool _oldMixOtherProbes = false;

        [Range(1, 5)]
        [Tooltip("Other reflection probes colors may become not as expected. Use this to increase the value (color) of them.")]
        public float _MixMultiplier = 1f;

        private Bounds _bounds;
        private List<Renderer> _renderers; //to hold renderers of the whole car object
        private Int64 _frameCount = 0; //to not draw for every X frame
        private Int64 _lastRenderedFrame = 0; //beforo camera execute may be called many times, prevent extra calls
        private bool _usePreviousTexture = false; //use texture from previous render for RunForEveryXthFrame option

        //to not draw the scene again for each cam on scene
        private Camera _sceneCamera = null;
        private ReflectionProbe _probe = null;
        private GameObject _probeContainer = null;
        private RenderTexture _cube = null;
        private IList<ReflectionProbe> _sceneProbes;

        //void OnEnable() //no need, fixed update will do it
        //{
        //    InitializeProperties();

        //}

        public void InitializeProperties()
        {
            _renderers = this.GetComponentsInChildren<Renderer>().ToList();

            _sceneProbes = FindObjectsOfType<ReflectionProbe>().ToList();

            CreateMirrorObjects(); //it is renderers dependent!

            RenderPipelineManager.beginCameraRendering -= ExecuteBeforeCameraRender;
            RenderPipelineManager.beginCameraRendering += ExecuteBeforeCameraRender;
        }

        private void FixedUpdate()
        {
            if (_sceneCamera == null)
            {
                FindCam();
                InitializeProperties();
            }
        }

        private void FindCam()
        {
            _sceneCamera = FindObjectOfType<Camera>();
        }

        //Bounds GetBoundsAndSetPositions()
        //{
        //    if (_renderers == null)
        //        return new Bounds();

        //    var b = new Bounds(this.transform.position, Vector3.zero);

        //    var previousRotation = this.transform.rotation;
        //    //reset
        //    this.transform.rotation = Quaternion.identity;
        //    foreach (Renderer r in _renderers)
        //    {
        //        b.Encapsulate(r.bounds);
        //    }
        //    //set back
        //    this.transform.rotation = previousRotation;

        //    _probe.gameObject.transform.position = b.center;
        //    _probe.size = b.size;
        //    _probe.gameObject.transform.rotation = this.transform.rotation;

        //    return b;
        //}


        void OnDrawGizmos()
        {
            if(IsDebug && _probe != null && _probe.gameObject!= null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(_probe.gameObject.transform.position, _probe.size);
            }
        }

        public void ExecuteBeforeCameraRender(
            ScriptableRenderContext context,
            Camera cameraSrp)
        {
            //_camera = cameraSrp;

            //_context = context;

            RenderReflective(context, cameraSrp);
        }

        private int _previousMaxAdditionalLightsCount;
        private void RenderReflective(ScriptableRenderContext context,
            Camera cameraSrp)
        {
            if(_sceneCamera == null)
            {
                Debug.LogError("Cam not found, will return with out render!");
                return;
            }

            //so this is not our cam to render, just return to not draw extra calls
            if (_sceneCamera != cameraSrp)
                return;

            _frameCount = Time.frameCount;

            if (!IsEnabled)
                return;

            _usePreviousTexture = false;
            if (_frameCount % RunForEveryXthFrame != 0)
            {
                _usePreviousTexture = true;
            }
            //we do not need to draw the reflection because of user selection :)
            if (_usePreviousTexture)
            {
                return;
            }

            if (cameraSrp.cameraType == CameraType.Preview || cameraSrp.cameraType == CameraType.Reflection)
                return;

            //we have already draw this frame!
            if (_lastRenderedFrame == Time.frameCount)
                return;

            _lastRenderedFrame = Time.frameCount; //!!

            //Debug.Log("Will render frame: " + Time.frameCount);

            //do not draw fog
            var previousFog = RenderSettings.fog;
            RenderSettings.fog = false;

            // Optionally disable pixel lights for reflection/refraction
            if (DisablePixelLights)
            {
                //save previous
                _previousMaxAdditionalLightsCount =
                    ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).maxAdditionalLightsCount;
                //disable
                ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).maxAdditionalLightsCount = 0;
            }

            var previousLODLevel = QualitySettings.maximumLODLevel;
            QualitySettings.maximumLODLevel = CameraLODLevel;

            //setup
            CreateMirrorObjects();
            UpdateProbe();
            SetProbeValues();

            //UpdateCameraModes();

            //var bounds = GetBoundsAndSetPositions();

            //_probe.RenderProbe();
            _probe.RenderProbe(_cube);
            //var probeResult = _probe.RenderProbe(_cube);
            //Debug.Log("Probe render result: " + probeResult);
            //Debug.Log("Is finished: " + _probe.IsFinishedRendering(probeResult));
            //Debug.Log("Frame: " + Time.frameCount);

            ApplyMaterials();

            //set fog
            RenderSettings.fog = previousFog;

            // Restore pixel light count
            if (DisablePixelLights)
            {
                //revert
                ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).maxAdditionalLightsCount =
                    _previousMaxAdditionalLightsCount;
            }

            QualitySettings.maximumLODLevel = previousLODLevel;
        }

        void SetProbeValues()
        {
            _probe.cullingMask = ReflectLayers.value;
            _probe.hdr = HDR;
            _probe.nearClipPlane = ClippingPlaneNear;
            _probe.farClipPlane = ClippingPlaneFar;
            _probe.resolution = (int)TextureSize; //must be power of 2!
            _probe.shadowDistance = ShadowDistance;
            
            //public bool UseOcclusionCulling = true; //todo: do we need to change main cam occlusion?
            if (MixOtherProbes)
            {
                _probe.clearFlags = ReflectionProbeClearFlags.SolidColor;
                _probe.backgroundColor = Color.black;
            }
            else
            {
                _probe.clearFlags = ReflectionProbeClearFlags.Skybox;
            }
        }

        void ApplyMaterials()
        {

            ReflectionProbe found = null;
            if(MixOtherProbes)
            {
                var max = float.MaxValue;
                foreach (var probe in _sceneProbes)
                {
                    if(probe.GetInstanceID() != _probe.GetInstanceID())
                    {
                        var distance = Vector3.Distance(this.transform.position, probe.transform.position);
                        if (distance < max)
                        {
                            max = distance;
                            found = probe;
                        }
                    }
                    
                }

            }

            //set positions
            Vector3 bboxLenght = _bounds.size; //calculated on start up and does not change
            Vector3 centerBBox = _probe.gameObject.transform.position; //moves with car!
            // Min and max BBox points in world coordinates.
            Vector3 BMin = centerBBox - bboxLenght / 2;
            Vector3 BMax = centerBBox + bboxLenght / 2;
            // Pass the values to the material.
            if (_renderers != null)
            {
                foreach (var ren in _renderers)
                {
                    if (ren != null)
                    {
                        foreach (var mat in ren.materials)
                        {
                            if (mat != null &&
                                (
                                    mat.shader.name == "AkilliMum/URP/CarPaint/2019" ||
                                    mat.shader.name == "AkilliMum/URP/CarPaint/2020_2"
                                ))
                            {
                                mat.SetVector("_BBoxMin", BMin);
                                mat.SetVector("_BBoxMax", BMax);
                                mat.SetVector("_EnviCubeMapPos", centerBBox);
                                //mat.SetTexture("_EnviCubeMapMain", _probe.texture);
                                mat.SetTexture("_EnviCubeMapMain", _cube);
                                mat.SetTexture("_EnviCubeMapToMix1", found == null ? null : found.texture);
                                mat.SetFloat("_MixMultiplier", _MixMultiplier);
                                //mat.SetFloat("_EnviCubeSmoothness", Smoothness); //moved to main shader
                                //mat.SetFloat("_EnviCubeIntensity", Intensity); //moved to main shader
                                if (MixOtherProbes)
                                    mat.EnableKeyword("_REALTIMEREFLECTION_MIX");
                                else
                                    mat.DisableKeyword("_REALTIMEREFLECTION_MIX");
                            }
                        }
                    }
                }
            }
        }

        // Cleanup all the objects we possibly have created
        protected virtual void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= ExecuteBeforeCameraRender;

            if (_probe)
            {
                DestroyImmediate(_probe);
                _probe = null;
            }

            if (_cube)
            {
                DestroyImmediate(_cube);
                _cube = null;
            }
        }

        // On-demand create any objects we need
        private void CreateMirrorObjects()
        {
            if(_cube == null ||
                !_cube ||
                _cube.width != (int)TextureSize ||
                _oldHDR != HDR ||
                //_oldSmoothness != Smoothness ||
                _oldMixOtherProbes != MixOtherProbes)
            {
                Debug.Log("will create cube texture! Something has changed...");

                if (_cube != null)
                    DestroyImmediate(_cube);
                if (HDR)
                {
                    if(SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                        _cube = new RenderTexture((int)TextureSize, (int)TextureSize, 24, RenderTextureFormat.ARGBFloat);
                    else
                        _cube = new RenderTexture((int)TextureSize, (int)TextureSize, 24, RenderTextureFormat.DefaultHDR);
                }
                else
                {
                    if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
                        _cube = new RenderTexture((int)TextureSize, (int)TextureSize, 24, RenderTextureFormat.ARGB32);
                    else
                        _cube = new RenderTexture((int)TextureSize, (int)TextureSize, 24, RenderTextureFormat.Default);
                }
                _cube.name = "__MirrorReflection1" + this.GetInstanceID();
                _cube.dimension = TextureDimension.Cube;
                if (MixOtherProbes)
                {
                    _cube.filterMode = FilterMode.Point;
                }
                else
                {
                    _cube.filterMode = FilterMode.Trilinear;
                }
                //else
                //{
                //    if (((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).msaaSampleCount > 0)
                //        _cube.antiAliasing = ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).msaaSampleCount;
                //}

                _cube.isPowerOfTwo = true;
                _cube.hideFlags = HideFlags.DontSave;
                //if (Smoothness>0)
                //{
                _cube.useMipMap = true;
                _cube.autoGenerateMips = true;
                //}
                //else
                //{
                //    _cube.useMipMap = false;
                //    _cube.autoGenerateMips = false;
                //}

                _oldHDR = HDR;
                //_oldSmoothness = Smoothness;
                _oldMixOtherProbes = MixOtherProbes;
            }
        }

        void UpdateProbe()
        {
            GetBoundsAndSetPositions();

            if (_probe == null || !_probe)
            {
                //Debug.Log("will create probe! Something has changed...");


                //GameObject go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + this.GetInstanceID(),
                //    typeof(ReflectionProbe));

                //_probe = go.GetComponent<ReflectionProbe>();
                _probe = this.GetComponentInChildren<ReflectionProbe>();
                if (_probe == null)
                {
                    Debug.Log("added new probe to object!");
                    _probeContainer = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + this.GetInstanceID(),
                        typeof(ReflectionProbe));
                    _probe = _probeContainer.GetComponent<ReflectionProbe>();
                    _probeContainer.hideFlags = HideFlags.HideAndDontSave;
                    _probeContainer.transform.parent = this.transform;
                }
                _probe.importance = 0; //will draw manually in shader
                _probe.mode = ReflectionProbeMode.Realtime;
                _probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
                _probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
                //_probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;

                
            }

            //set dynamic values
            _probeContainer.transform.position = _bounds.center;
            _probe.size = _bounds.size;
        }

        void GetBoundsAndSetPositions()
        {
            if (_renderers == null)
                _bounds = new Bounds();

            _bounds = new Bounds(this.transform.position, Vector3.zero);

            foreach (Renderer r in _renderers)
            {
                _bounds.Encapsulate(r.bounds);
            }

            //do not change for default :)
            if (BoundingBox == BoundingBox.Average)
            {
                //get average for x,z space
                var average = (_bounds.size.x + _bounds.size.z) / 2;
                _bounds.size = new Vector3(average, _bounds.size.y, average);
            }
            else if(BoundingBox == BoundingBox.Max)
            {
                //get max for x,z space
                var max = _bounds.size.x > _bounds.size.z ? _bounds.size.x : _bounds.size.z;
                _bounds.size = new Vector3(max, _bounds.size.y, max);
            }
            else if (BoundingBox == BoundingBox.Min)
            {
                //get min for x,z space
                var min = _bounds.size.x > _bounds.size.z ? _bounds.size.z : _bounds.size.x;
                _bounds.size = new Vector3(min, _bounds.size.y, min);
            }
        }
    }

    public enum TextureSizeType
    {
        x16 = 16,
        x32 = 32,
        x64 = 64,
        x128 = 128,
        x256 = 256,
        x512 = 512,
        x1024 = 1024,
        x2048 = 2048
    }

    public enum BoundingBox
    {
        Default = 1,
        Average = 10,
        Min = 20,
        Max = 30
    }

    public class SceneLights
    {
        public Light Light;
        public bool Enabled;
    }

    
}
