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

    private Vector2 gazePointLeft, gazePointRight, gazePointCenter;

    void Start()
    {
        camera = GetComponent<Camera>();

        gazePointLeft = new Vector2(0.6f, 0.5f);
        gazePointRight = new Vector2(0.4f, 0.5f);
        gazePointCenter = new Vector2(0.5f, 0.5f);
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
            
            if (Application.isPlaying && ExperimentManager.Condition.WindowPosition == ConditionData.WindowPositionEnum.Dynamic)
            {
                gazePointLeft = LimitedGazePoint(gazePointLeft,GazeInputManager.GazePointLeft);
                gazePointRight = LimitedGazePoint(gazePointRight,GazeInputManager.GazePointRight);
                gazePointCenter = LimitedGazePoint(gazePointCenter,GazeInputManager.GazePointCenter);
            }
            else
            {
                gazePointLeft = new Vector2(0.6f, 0.5f);
                gazePointRight = new Vector2(0.4f, 0.5f);
                gazePointCenter = new Vector2(0.5f, 0.5f);

                /*bool fakeGazeCorner = true;
                if (fakeGazeCorner)
                {
                    gazePointLeft = new Vector2(0.8f, 0.3f);
                    gazePointRight = new Vector2(0.6f, 0.3f);
                    gazePointCenter = new Vector2(0.7f, 0.3f);
                }*/
            }
            switch (camera.stereoTargetEye)
            {
                case StereoTargetEyeMask.Left:
                    material.SetVector("_BlurMaskCenter", gazePointLeft);
                    break;
                case StereoTargetEyeMask.Right:
                    material.SetVector("_BlurMaskCenter", gazePointRight);
                    break;
                default:
                    material.SetVector("_BlurMaskCenter", gazePointCenter);
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

    Vector2 LimitedGazePoint(Vector2 origin, Vector2 input)
    {
        float windowRadius = 0.4f;
        //float movableRadius = 1.0f - windowRadius * 2;

        float tolerance = 0.3f;
        float a = (2.0f * windowRadius - 1.0f) / (2.0f * tolerance - 1.0f);
        float b = (1.0f - a) / 2.0f;

        Vector2 mapped = new Vector2(a * input.x + b, a * input.y + b);
        
        if (input.x > (1.0f - tolerance) || input.x < tolerance || input.y > (1.0f - tolerance) || input.y < tolerance) return origin;
        //else return input;
        else return mapped;
    }
}

