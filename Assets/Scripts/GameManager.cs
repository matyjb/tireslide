using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    public CanvasGroup pauseCanvas;

    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            Time.timeScale = _isPaused ? 0 : 1;
            AudioListener.pause = _isPaused;
        }
    }

    public void Pause_performed(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            IsPaused = !IsPaused;
        }
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        pauseCanvas.alpha = Mathf.Lerp(pauseCanvas.alpha, IsPaused ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
    }
}
