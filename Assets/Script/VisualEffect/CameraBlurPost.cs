using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraBlurPost : MonoBehaviour {

    // attach this on the camera

    public Transform ForwardTarget;
    public Shader CurShader;
    public Material material;

    public int DownSampleNum = 2;
    public float BlurSpreadSize = 3.0f;
    public int BlurIterations = 3;

    private Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (CurShader != null)
        {
#if UNITY_EDITOR
            if (camera == null)
            {
                camera = GetComponent<Camera>();
            }
#endif
            // set gaze point as center
            Vector2 gazePointLeft, gazePointRight, gazePointCenter;
            if (Application.isPlaying && ExperimentManager.Condition.WindowPosition == ConditionData.WindowPositionEnum.Dynamic)
            {
                gazePointLeft = GazeInputManager.GazePointLeft;
                gazePointRight = GazeInputManager.GazePointRight;
                gazePointCenter = GazeInputManager.GazePointCenter;
            }
            else
            {
                gazePointLeft = new Vector2(0.5f, 0.6f);
                gazePointRight = new Vector2(0.5f, 0.4f);
                gazePointCenter = new Vector2(0.5f, 0.5f);
            }
            switch (camera.stereoTargetEye)
            {
                case StereoTargetEyeMask.Left:
                    material.SetVector("_viewportGazePosition", gazePointLeft);
                    break;
                case StereoTargetEyeMask.Right:
                    material.SetVector("_viewportGazePosition", gazePointRight);
                    break;
                default:
                    material.SetVector("_viewportGazePosition", gazePointCenter);
                    break;
            }
            
            
            
            float widthMod = 1.0f / (1.0f * (1 << DownSampleNum));

            material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod);
            sourceTexture.filterMode = FilterMode.Bilinear;

            int renderWidth = sourceTexture.width >> DownSampleNum;
            int renderHeight = sourceTexture.height >> DownSampleNum;

            RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);

            renderBuffer.filterMode = FilterMode.Bilinear;

            Graphics.Blit(sourceTexture, renderBuffer, material, 0);


            for (int i = 0; i < BlurIterations; i++)
            {

                float iterationOffs = (i * 1.0f);

                material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod + iterationOffs);


                RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);

                Graphics.Blit(renderBuffer, tempBuffer, material, 1);
                RenderTexture.ReleaseTemporary(renderBuffer);

                renderBuffer = tempBuffer;

                tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                Graphics.Blit(renderBuffer, tempBuffer, material, 2);


                RenderTexture.ReleaseTemporary(renderBuffer);
                renderBuffer = tempBuffer;
            }

            Graphics.Blit(renderBuffer, destTexture);
            RenderTexture.ReleaseTemporary(renderBuffer);

        }

        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
    
}

