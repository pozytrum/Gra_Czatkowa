using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPref = "BackgroundPref";

    [Tooltip("Root GameObject of the menu used to toggle its activation")]
    public GameObject menuRoot;
    [Tooltip("Master volume when menu is open")]
    [Range(0.001f, 1f)]
    public float volumeWhenMenuOpen = 0.5f;
    [Tooltip("Toggle component for shadows")]
    public Toggle shadowsToggle;
    [Tooltip("Toggle component for framerate display")]
    public Toggle framerateToggle;
    [Tooltip("Toggle component for music control")]
    public Toggle musicToggle;
    [Tooltip("GameObject for the controls")]
    public GameObject controlImage;

    public AudioSource backgroundAudio;

    //PlayerInputHandler m_PlayerInputsHandler;
    FramerateCounter m_FramerateCounter;

    void Start()
    {
        //m_PlayerInputsHandler = FindObjectOfType<PlayerInputHandler>();
        //DebugUtility.HandleErrorIfNullFindObject<PlayerInputHandler, InGameMenuManager>(m_PlayerInputsHandler, this);

        if (PlayerPrefs.GetInt(FirstPlay) == 0)
        {
            PlayerPrefs.SetInt(BackgroundPref, 1);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }

        UpdateSound();

        m_FramerateCounter = FindObjectOfType<FramerateCounter>();
        //DebugUtility.HandleErrorIfNullFindObject<FramerateCounter, InGameMenuManager>(m_FramerateCounter, this);

        menuRoot.SetActive(false);

        shadowsToggle.isOn = QualitySettings.shadows != ShadowQuality.Disable;
        shadowsToggle.onValueChanged.AddListener(OnShadowsChanged);

        framerateToggle.isOn = m_FramerateCounter.uiText.gameObject.activeSelf;
        framerateToggle.onValueChanged.AddListener(OnFramerateCounterChanged);

        musicToggle.isOn = PlayerPrefs.GetInt(BackgroundPref) == 1 ? true : false;
        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
    }

    private void Update()
    {
        
        if (Input.GetButtonDown(GameConstants.k_ButtonNamePauseMenu)
            || (menuRoot.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel)))
        {
            if (controlImage.activeSelf)
            {
                controlImage.SetActive(false);
                return;
            }

            SetPauseMenuActivation(!menuRoot.activeSelf);

        }

        if (Input.GetAxisRaw(GameConstants.k_AxisNameVertical) != 0)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                shadowsToggle.Select();
            }
        }
    }

    public void ClosePauseMenu()
    {
        SetPauseMenuActivation(false);
    }


    public void TogglePauseMenu()
    {
        SetPauseMenuActivation(!menuRoot.activeSelf);
    }
    void SetPauseMenuActivation(bool active)
    {
        menuRoot.SetActive(active);

        if (menuRoot.activeSelf)
        {
       //     Cursor.lockState = CursorLockMode.None;
          //  Cursor.visible = true;
            Time.timeScale = 0f;
            AudioUtility.SetMasterVolume(volumeWhenMenuOpen);

            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
         //   Cursor.lockState = CursorLockMode.Locked;
         //   Cursor.visible = false;
            Time.timeScale = 1f;
            AudioUtility.SetMasterVolume(1);
        }

    }

    void OnShadowsChanged(bool newValue)
    {
        QualitySettings.shadows = newValue ? ShadowQuality.All : ShadowQuality.Disable;
    }

    void OnFramerateCounterChanged(bool newValue)
    {
        m_FramerateCounter.uiText.gameObject.SetActive(newValue);
    }

    public void OnShowControlButtonClicked(bool show)
    {
        controlImage.SetActive(show);
    }

    private void UpdateSound()
    {
        if (PlayerPrefs.GetInt(BackgroundPref) == 1)
        {
            backgroundAudio.volume = 1.0F;
        } else
        {
            backgroundAudio.volume = 0.000F;
        }
    }

    private void OnMusicToggleChanged(bool toggle)
    {
        if (toggle)
        {
            PlayerPrefs.SetInt(BackgroundPref, 1);
        } else
        {
            PlayerPrefs.SetInt(BackgroundPref, 0);
        }


        UpdateSound();
    }
}
