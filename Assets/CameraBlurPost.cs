using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraBlurPost : MonoBehaviour {

    public Transform ForwardTarget;
    public Shader CurShader;
    public Material material;

    public int DownSampleNum = 2;
    public float BlurSpreadSize = 3.0f;
    public int BlurIterations = 3;

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (CurShader != null)
        {

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

