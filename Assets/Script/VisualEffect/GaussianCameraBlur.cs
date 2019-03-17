using UnityEngine;
using System.Collections;


[ExecuteInEditMode]

public class GaussianCameraBlur : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    struct BlurredCamera
    {
        public CameraBlurPost camera;
        public Material material;
    };
    [SerializeField]
    public bool BlurEnabled = true;
    [SerializeField]
    BlurredCamera[] bCameras;
    [SerializeField]
    Transform ForwardTarget;
    [SerializeField]
    bool indicated = true;
    [SerializeField]
    Color indicatedColor = new Color(1, 0, 0, 0);
    private Color hiddenColor = new Color(0, 0, 0, 0);
    [SerializeField, Range(0.0f, 1.0f)]
    float InnerRadius = 0.15f;
    [SerializeField, Range(0.0f, 1.0f)]
    float OuterRadius = 0.35f;

    private string ShaderName = "CustomShader/GaussianBlur";

    public Shader CurShader;
    private Material CurMaterial;

    public static int ChangeValue;
    public static float ChangeValue2;
    public static int ChangeValue3;

    [Range(0, 6)]
    public int DownSampleNum = 2;
    [Range(0.0f, 20.0f)]
    public float BlurSpreadSize = 3.0f;
    [Range(0, 8)]
    public int BlurIterations = 3;

    #endregion

    #region MaterialGetAndSet
    Material material
    {
        get
        {
            if (CurMaterial == null)
            {
                CurMaterial = new Material(CurShader);
                CurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return CurMaterial;
        }
    }
    #endregion

    #region Functions

    void Start()
    {
        ChangeValue = DownSampleNum;
        ChangeValue2 = BlurSpreadSize;
        ChangeValue3 = BlurIterations;

        CurShader = Shader.Find(ShaderName);

        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < bCameras.Length; i++)
        {
            bCameras[i].camera.ForwardTarget = ForwardTarget;
            bCameras[i].camera.CurShader = CurShader;
        }
    }

    



    void OnValidate()
    {
        ChangeValue = DownSampleNum;
        ChangeValue2 = BlurSpreadSize;
        ChangeValue3 = BlurIterations;
    }


    void Update()
    {
        if (Application.isPlaying)
        {
            DownSampleNum = ChangeValue;
            BlurSpreadSize = ChangeValue2;
            BlurIterations = ChangeValue3;
        }
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            CurShader = Shader.Find(ShaderName);
        }
#endif

        for (int i = 0; i < bCameras.Length; i++)
        {
            bCameras[i].camera.enabled = BlurEnabled;
            bCameras[i].material.SetFloat("_InnerRadius", InnerRadius);
            bCameras[i].material.SetFloat("_OuterRadius", OuterRadius);
            bCameras[i].camera.DownSampleNum = DownSampleNum;
            bCameras[i].camera.BlurSpreadSize = BlurSpreadSize;
            bCameras[i].camera.BlurIterations = BlurIterations;

            if (!indicated) bCameras[i].material.SetColor("_IndicatorColor", hiddenColor);
            else bCameras[i].material.SetColor("_IndicatorColor", indicatedColor);
        }
        
        /*if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetButtonDown("Oculus_GearVR_X"))
        {
            indicated = !indicated;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetButtonDown("Oculus_GearVR_Y"))
        {
            BlurEnabled = !BlurEnabled;
        }*/

    }

    void OnDisable()
    {
        if (CurMaterial)
        {
            DestroyImmediate(CurMaterial);
        }

    }

    #endregion

}

