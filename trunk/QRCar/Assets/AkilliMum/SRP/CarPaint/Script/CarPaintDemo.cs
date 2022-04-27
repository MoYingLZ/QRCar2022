#define DEBUG_RENDER

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AkilliMum.SRP.CarPaint
{
    public class CarPaintDemo : MonoBehaviour
    {
#if DEBUG_RENDER
        private float _deltaTime = 0.0f;
#endif

        public Material[] Materials;

        public Texture2D Livery;
        public Toggle LiveryToggle;

        public Texture2D Decal;
        public Toggle DecalToggle;

        public Texture2D Dirt;
        public Toggle DirtToggle;
        public Slider DirtCutOffSlider;

        void OnEnable()
        {
            Application.targetFrameRate = 1000; //todo: danger!!!!!!!!!!!!!!!!
        }

        private void Update()
        {
#if DEBUG_RENDER
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
#endif
        }

        void OnGUI()
        {
#if DEBUG_RENDER
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 50);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
#endif
        }

        public enum DemoOperation
        {
            LiveryOn = 1,
            LiveryOff = 2,
            DecalOn = 10,
            DecalOff = 11,
            DirtOn = 21,
            DirtOff = 22,
            DirtCutOff = 23
        }

        public void ToggleLivery()
        {
            if (LiveryToggle.isOn)
                Change(DemoOperation.LiveryOn);
            else
                Change(DemoOperation.LiveryOff);
        }

        public void ToggleDecal()
        {
            if (DecalToggle.isOn)
                Change(DemoOperation.DecalOn);
            else
                Change(DemoOperation.DecalOff);
        }

        public void ToggleDirt()
        {
            if (DirtToggle.isOn)
                Change(DemoOperation.DirtOn);
            else
                Change(DemoOperation.DirtOff);
        }

        public void ToggleDirtCutOffSlider()
        {
            Change(DemoOperation.DirtCutOff, DirtCutOffSlider.value);
        }

        void Change(DemoOperation demoOperation, float value = 0)
        {
            //Debug.Log("Demo Operation: " + demoOperation);

            foreach (var mat in Materials)
            {
                if (demoOperation == DemoOperation.LiveryOn)
                {
                    mat.SetTexture("_LiveryMap", Livery);
                    mat.SetFloat("_LiveryUsage", 1);
                }
                else if (demoOperation == DemoOperation.LiveryOff)
                {
                    mat.SetTexture("_LiveryMap", null);
                    mat.SetFloat("_LiveryUsage", 0);
                }
                else if (demoOperation == DemoOperation.DecalOn)
                {
                    mat.SetTexture("_DecalMap", Decal);
                    mat.SetFloat("_DecalUsage", 1);
                }
                else if (demoOperation == DemoOperation.DecalOff)
                {
                    mat.SetTexture("_DecalMap", null);
                    mat.SetFloat("_DecalUsage", 0);
                }
                else if (demoOperation == DemoOperation.DirtOn)
                {
                    mat.SetTexture("_DirtMap", Dirt);
                    mat.SetFloat("_DirtUsage", 1);
                }
                else if (demoOperation == DemoOperation.DirtOff)
                {
                    mat.SetTexture("_DirtMap", null);
                    mat.SetFloat("_DirtUsage", 0);

                }
                else if (demoOperation == DemoOperation.DirtCutOff)
                {
                    mat.SetFloat("_DirtMapCutoff", value);
                }
            }

        }
    }
}