using System.Collections;
using UnityEngine;

[ExecuteInEditMode]

public class VisaulEffectManager : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    struct BlurredCamera
    {
        public string name;
        public InputManager.HmdType hmdType;
        public CameraBlurPost camera;
        public Material material;
    };

    private string ShaderName = "CustomShader/GaussianBlur";

    public Shader CurShader;
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
    private Material CurMaterial;

    [Header("Object Reference")]

    [SerializeField]
    BlurredCamera[] BlurredCameras;
    [SerializeField]
    Transform OculusForwardTarget;
    [SerializeField]
    Transform ViveForwardTarget;

    [SerializeField]
    WindowSizeConfig m_WindowSizeConfig;

    [Header("Parameters")]

    [SerializeField]
    public bool BlurEnabled = true;    
    [SerializeField]
    bool indicated = true;
    
    [SerializeField, Range(0.0f, 1.0f)]
    float InnerRadius = 0.15f;
    [SerializeField, Range(0.0f, 1.0f)]
    float OuterRadius = 0.35f;

    [SerializeField, Range(0, 6)]
    public int DownSampleNum = 2;
    [SerializeField, Range(0.0f, 20.0f)]
    public float BlurSpreadSize = 3.0f;
    [SerializeField, Range(0, 8)]
    public int BlurIterations = 3;

    public static int stored_DownSampleNum;
    public static float stored_BlurSpreadSize;
    public static int stored_BlurIterations;

    [SerializeField]
    Color indicatedColor = new Color(1, 0, 0, 0);
    private Color hiddenColor = new Color(0, 0, 0, 0);
    #endregion

    #region Functions

    void Start()
    {
        stored_DownSampleNum = DownSampleNum;
        stored_BlurSpreadSize = BlurSpreadSize;
        stored_BlurIterations = BlurIterations;

        CurShader = Shader.Find(ShaderName);

        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < BlurredCameras.Length; i++)
        {
            if (BlurredCameras[i].hmdType == InputManager.HmdType.Oculus) BlurredCameras[i].camera.ForwardTarget = OculusForwardTarget;
            if (BlurredCameras[i].hmdType == InputManager.HmdType.Vive) BlurredCameras[i].camera.ForwardTarget = ViveForwardTarget;
            BlurredCameras[i].camera.CurShader = CurShader;
        }

        // set parameters
        ConditionData condition = ExperimentManager.Condition;
        BlurEnabled = condition.BlurInPeripheral;
        InnerRadius = m_WindowSizeConfig.RadiusList[(int)condition.WindowSize].Inner;
        OuterRadius = m_WindowSizeConfig.RadiusList[(int)condition.WindowSize].Outer;
        
        // TODO: condition.WindowPosition (should be connected to pupil labs)
    }

    void OnValidate()
    {
        stored_DownSampleNum = DownSampleNum;
        stored_BlurSpreadSize = BlurSpreadSize;
        stored_BlurIterations = BlurIterations;
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            DownSampleNum = stored_DownSampleNum;
            BlurSpreadSize = stored_BlurSpreadSize;
            BlurIterations = stored_BlurIterations;
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            CurShader = Shader.Find(ShaderName);
        }
#endif

        for (int i = 0; i < BlurredCameras.Length; i++)
        {
            BlurredCameras[i].camera.enabled = BlurEnabled;

            BlurredCameras[i].material.SetFloat("_InnerRadius", InnerRadius);
            BlurredCameras[i].material.SetFloat("_OuterRadius", OuterRadius);

            BlurredCameras[i].camera.DownSampleNum = DownSampleNum;
            BlurredCameras[i].camera.BlurSpreadSize = BlurSpreadSize;
            BlurredCameras[i].camera.BlurIterations = BlurIterations;

            if (!indicated) BlurredCameras[i].material.SetColor("_IndicatorColor", hiddenColor);
            else BlurredCameras[i].material.SetColor("_IndicatorColor", indicatedColor);
        }

        // KeyInput();
    }

    void OnDisable()
    {
        if (CurMaterial)
        {
            DestroyImmediate(CurMaterial);
        }

    }

    void KeyInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetButtonDown("Oculus_GearVR_X"))
        {
            indicated = !indicated;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetButtonDown("Oculus_GearVR_Y"))
        {
            BlurEnabled = !BlurEnabled;
        }
    }

    #endregion
}