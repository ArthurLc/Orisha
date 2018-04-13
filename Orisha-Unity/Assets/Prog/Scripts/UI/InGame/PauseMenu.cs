/*Script temporaire de menu in game (pour paramétrer la cam)
 * 
 * 
 * 
 * Manquant:
 *  - Initialisation des variables en fonction du GameLoopManager (axes inversés, sensi camera)
 *  - Navigation dans la fenêtre d'option (joystick)
 *  
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using vd_Inputs;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    [Header("Sous menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionMenu;
    private float timeScaleWhenPaused = 0.0f;

    StandaloneInputModule inputModule;

	public Button firstButtonActive;
	public Slider firstOptionActive;

    private void OnEnable()
    {
        inputModule = FindObjectOfType<StandaloneInputModule>();
        InputManager.newInputMode += UpdateInputmode;
    }


    private void OnDisable()
    {
        InputManager.newInputMode -= UpdateInputmode;
    }

    bool UpdateInputmode(vd_Inputs.InputMode _newInputMode)
    {
        inputModule.horizontalAxis = InputManager.Horizontal;
        inputModule.verticalAxis = InputManager.Vertical;

        return true;
    }
		
    void Start ()
    {
        inputModule = FindObjectOfType<StandaloneInputModule>();
        UpdateInputmode(InputManager.GetInputMode);
        pauseMenu.SetActive(false);

		InputManager.newInputMode += OnInputModeChange;
    }
	
	void Update ()
    {
		if (Input.GetButtonDown (vd_Inputs.InputManager.Pause)) 
		{
			if (pauseMenu.activeInHierarchy == false)
				Pause ();
			else
				Continue ();
		} 
		else if (pauseMenu.activeInHierarchy == true && Input.GetButtonDown (vd_Inputs.InputManager.Dash)) 
		{
			if (optionMenu.activeSelf == true)
				ResetMenu ();
			else
				Continue ();
		}
	}

    private void Pause()
    {
        FindObjectOfType<Cinemachine.CinemachineBrain>().enabled = false;
        pauseMenu.SetActive(true);
        GameLoopManager.EnableMouse();
		OnInputModeChange (InputManager.GetInputMode);
        timeScaleWhenPaused = Time.timeScale;
        Time.timeScale = 0.0f;
		TimeManager.Instance.Slow_AllScene (0.0f);
    }
		
    public void Continue()
    {
        FindObjectOfType<Cinemachine.CinemachineBrain>().enabled = true;

		ResetMenu();
        pauseMenu.SetActive(false);
        GameLoopManager.DisableMouse();


        Time.timeScale = timeScaleWhenPaused;
		TimeManager.Instance.Slow_UnactiveAll();
        //Debug.Log(Time.timeScale);
    }

    public void Menu()
    {
        if (SceneManager.GetSceneByName("Menu").IsValid() == true)
            SceneManager.LoadScene("Menu");
        else
            Debug.LogWarning("PauseMenu: pas de scène Menu, retour menu impossible.");
    }

	/// <summary>
	/// return to the MainMenu canvas.
	/// </summary>
	public void ResetMenu()
	{
		mainMenu.SetActive(true);
		optionMenu.SetActive(false);

		if(EventSystem.current.currentSelectedGameObject != null)
			EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().Select();

		OnInputModeChange (InputManager.GetInputMode);
	}

    /// <summary>
    /// Ouverture du panel Options
    /// </summary>
    public void Options()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);

		OnInputModeChange (InputManager.GetInputMode);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Camera_UpdateSensi(Slider _slider)
    {
        GameLoopManager.camera_sensitivity = _slider.value * 2.0f;
        InputManager.newInputMode(InputManager.GetInputMode);
    }

    public void Camera_InvertAxisX(Toggle _toggle)
    {
        GameLoopManager.camera_isXaxisInversed = _toggle.isOn;
        InputManager.newInputMode(InputManager.GetInputMode);
    }

    public void Camera_InvertAxisY(Toggle _toggle)
    {
        GameLoopManager.camera_isYaxisInversed = _toggle.isOn;
        InputManager.newInputMode(InputManager.GetInputMode);
    }

	bool OnInputModeChange(InputMode _newMode)
	{
		if (_newMode == InputMode.keyboard && pauseMenu.activeInHierarchy == true)
		{
			EventSystem.current.GetComponent<EventSystem> ().SetSelectedGameObject (null);
			GameLoopManager.EnableMouse();
		}
		
		else if(pauseMenu.activeInHierarchy == true)
		{
			GameLoopManager.DisableMouse();
				
			if (mainMenu.activeSelf == true)
				firstButtonActive.Select ();
			else
				firstOptionActive.Select ();
		}

		return true;
	}
}
