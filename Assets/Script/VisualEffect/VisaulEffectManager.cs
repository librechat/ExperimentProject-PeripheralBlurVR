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

    [SerializeField]
    private InputManager.HmdType HMDType = InputManager.HmdType.Vive;

    [Header("Vive Object Reference")]

    [SerializeField]
    Transform ViveForwardTarget;
    [SerializeField]
    BlurredCamera[] ViveBlurredCameras;

    [Header("Oculus Object Reference")]

    [SerializeField]
    BlurredCamera[] OculusBlurredCameras;
    [SerializeField]
    Transform OculusForwardTarget;

    [Header("Config")]    

    [SerializeField]
    WindowSizeConfig m_WindowSizeConfig;

    [Header("Parameters")]

    [SerializeField]
    public bool BlurEnabled = true;    
    [SerializeField]
    bool indicated = true;
    
    //[SerializeField, Range(0.0f, 1.0f)]
    float InnerRadius = 0.15f;
    //[SerializeField, Range(0.0f, 1.0f)]
    float OuterRadius = 0.35f;

    [SerializeField, Range(0, 6)]
    public int DownSampleNum = 0;
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

    public static Camera SceneCamera
    {
        get { return s_Instance.blurredCameras[0].camera.GetComponent<Camera>(); }
    }

    private BlurredCamera[] blurredCameras;
    private Transform forwardTarget;

    public static VisaulEffectManager s_Instance;

    #endregion

    #region Functions

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;

        blurredCameras = (HMDType == InputManager.HmdType.Oculus) ? OculusBlurredCameras : ViveBlurredCameras;
        forwardTarget = (HMDType == InputManager.HmdType.Oculus) ? OculusForwardTarget : ViveForwardTarget;
    }

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

        for (int i = 0; i < blurredCameras.Length; i++)
        {
            blurredCameras[i].camera.ForwardTarget = forwardTarget;
            blurredCameras[i].camera.CurShader = CurShader;
        }
    }

    public void Init()
    {
        // set parameters
        SetParameters(ExperimentManager.Condition);
    }

    void OnValidate()
    {
        stored_DownSampleNum = DownSampleNum;
        stored_BlurSpreadSize = BlurSpreadSize;
        stored_BlurIterations = BlurIterations;
    }

    void Update()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            CurShader = Shader.Find(ShaderName);
            blurredCameras = (HMDType == InputManager.HmdType.Oculus) ? OculusBlurredCameras : ViveBlurredCameras;
            forwardTarget = (HMDType == InputManager.HmdType.Oculus) ? OculusForwardTarget : ViveForwardTarget;
        }
#endif
        if (Application.isPlaying)
        {
            DownSampleNum = stored_DownSampleNum;
            BlurSpreadSize = stored_BlurSpreadSize;
            BlurIterations = stored_BlurIterations;
        }

        for (int i = 0; i < blurredCameras.Length; i++)
        {
            blurredCameras[i].camera.enabled = BlurEnabled;

            blurredCameras[i].material.SetFloat("_InnerRadius", InnerRadius);
            blurredCameras[i].material.SetFloat("_OuterRadius", OuterRadius);

            blurredCameras[i].camera.DownSampleNum = DownSampleNum;
            blurredCameras[i].camera.BlurSpreadSize = BlurSpreadSize;
            blurredCameras[i].camera.BlurIterations = BlurIterations;

            if (!indicated) blurredCameras[i].material.SetColor("_IndicatorColor", hiddenColor);
            else blurredCameras[i].material.SetColor("_IndicatorColor", indicatedColor);

            // get gaze position and change window position if in dynamic condition
        }
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

    public void SetParameters(ConditionData condition)
    {
        BlurEnabled = condition.BlurInPeripheral;
        InnerRadius = m_WindowSizeConfig.RadiusList[(int)condition.WindowSize].Inner;
        OuterRadius = m_WindowSizeConfig.RadiusList[(int)condition.WindowSize].Outer;

        // TODO: condition.WindowPosition (should be connected to pupil labs)
    }

    #endregion
}