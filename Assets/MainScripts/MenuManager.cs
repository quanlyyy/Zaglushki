using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    private static MenuManager instance;
    private AsyncOperation loadingSceneOperation;

    public bool bloom, AmbientOcclusion, ColorGrading, Vignette, DepthOfField = false;
    public Toggle bloomBTN, AmbientOcclusionBTN, ColorGradingBTN, VignetteBTN, DepthOfFieldBTN;

    public Image LoadSceneImg;
    public Image loadImageAmount;

    public PostProcessProfile profile;

    private Bloom b;
    private AmbientOcclusion a;
    private ColorGrading c;
    private Vignette v;
    private DepthOfField d;

    private bool loadedProgressChanged = false;

    public GameObject settingsBtn;

    public bool stateAnimSettings = true;
    public GameObject BGLoad;
    public LoadAnim LoadAnimObj;
    public Text loadTExt;

    [SerializeField] float mouseX;
    [SerializeField] float mouseY;

    Vector3 distance;
    public Vector3 centerPos;

    public GameObject BGMooved;

    public void Start()
    {
        instance = this;
        LoadPrefs();
    }

    public static void SwitchToScene(string sceneName)
    {
        if (instance == null) return;

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        instance.loadedProgressChanged = true;
    }

    void Update()
    {
        if (!Sceneindex())
        {
            if (bloomBTN != null) bloom = bloomBTN.isOn;
            if (AmbientOcclusionBTN != null) AmbientOcclusion = AmbientOcclusionBTN.isOn;
            if (ColorGradingBTN != null) ColorGrading = ColorGradingBTN.isOn;
            if (VignetteBTN != null) Vignette = VignetteBTN.isOn;
            if (DepthOfFieldBTN != null) DepthOfField = DepthOfFieldBTN.isOn;
        }

        if (loadedProgressChanged && loadingSceneOperation != null)
        {
            loadImageAmount.fillAmount = loadingSceneOperation.progress;
            loadTExt.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + " %";
        }

        if (!Sceneindex())
        {
            distance = centerPos - Input.mousePosition;
            mouseX = distance.x;
            mouseY = distance.y;

            BGMooved.transform.position = new Vector3(-mouseX / 1000, -mouseY / 1000, 0f);
        }
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("bloom", bloom ? 1 : 0);
        PlayerPrefs.SetInt("AmbientOcclusion", AmbientOcclusion ? 1 : 0);
        PlayerPrefs.SetInt("ColorGrading", ColorGrading ? 1 : 0);
        PlayerPrefs.SetInt("Vignette", Vignette ? 1 : 0);
        PlayerPrefs.SetInt("DepthOfField", DepthOfField ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SavePrefs();
        SwitchToScene("SampleScene");
    }

    public void StartEducation()
    {
        SavePrefs();
        SwitchToScene("EducationScene");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
    public void CreateUserBtn()
    {
        SceneManager.LoadSceneAsync("InstructorCreateUserScene");
    }

    public void ResultsBtn()
    {
        SceneManager.LoadSceneAsync("InstructorResultsSystems");
    }

    public void UsersBtn()
    {
        SceneManager.LoadSceneAsync("InstructorUserScene");
    }

    public void ProfileBtn()
    {
        SceneManager.LoadSceneAsync("ProfileScene");
    }
    public void ExitAccountBtn()
    {
        SceneManager.LoadSceneAsync("LoginScene");
    }

    public void AnalyticsBtn()
    {
        SceneManager.LoadSceneAsync("AnalyticsScene");
    }

    public void LoadPrefs()
    {
        if (Sceneindex())
        {
            profile.TryGetSettings(out b);
            profile.TryGetSettings(out a);
            profile.TryGetSettings(out c);
            profile.TryGetSettings(out v);
            profile.TryGetSettings(out d);

            b.active = PlayerPrefs.GetInt("bloom") == 1;
            a.active = PlayerPrefs.GetInt("AmbientOcclusion") == 1;
            c.active = PlayerPrefs.GetInt("ColorGrading") == 1;
            v.active = PlayerPrefs.GetInt("Vignette") == 1;
            d.active = PlayerPrefs.GetInt("DepthOfField") == 1;
        }
    }

    public bool Sceneindex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "SampleScene" || sceneName == "EducationScene";
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void SettingsPanel()
    {
        if (stateAnimSettings)
        {
            settingsBtn.GetComponent<Animator>().Play("SettingsClose");
            stateAnimSettings = !stateAnimSettings;
        }
        else
        {
            settingsBtn.GetComponent<Animator>().Play("SettingsOpen");
            stateAnimSettings = !stateAnimSettings;
        }
    }
}