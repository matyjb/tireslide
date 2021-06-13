using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    None,
    Playing,
    Paused,
    Finished,
    Starting
}

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    public Rigidbody player;

    public CanvasGroup pauseCanvas;

    private GameState _gameState;

    private Coroutine startingCoroutine;

    public Animator startAnim;

    private AudioSource countdownAudio;

    public static float initialGameTimeLeft = 60;

    public float gameTimeLeftSeconds;

    public TextMeshProUGUI timeLeftText;

    private float timerInitialFontSize;

    private CameraController camController;

    private Camera camera;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            if (_gameState != value)
            {
                // gamestate changed!
                switch (value)
                {
                    case GameState.Starting:
                        camController.enabled = true;
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // freeze player
                        player.constraints = RigidbodyConstraints.FreezeAll;
                        // start 3..2..1..Go
                        if (startingCoroutine != null)
                        {
                            StopCoroutine(startingCoroutine);
                        }
                        startingCoroutine = StartCoroutine(ThreeTwoOneGo());
                        gameTimeLeftSeconds = initialGameTimeLeft;
                        player.gameObject.GetComponent<CarControllerNew>().ControlsEnabled = true;
                        break;
                    case GameState.Playing:
                        camController.enabled = true;
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // unfreeze player
                        player.constraints = RigidbodyConstraints.None;
                        player.gameObject.GetComponent<CarControllerNew>().ControlsEnabled = true;
                        break;
                    case GameState.Paused:
                        camController.enabled = true;
                        Time.timeScale = 0;
                        AudioListener.pause = true;
                        player.gameObject.GetComponent<CarControllerNew>().ControlsEnabled = false;
                        break;
                    case GameState.Finished:
                        camController.enabled = false;
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        player.gameObject.GetComponent<CarControllerNew>().ControlsEnabled = false;
                        // unfreeze player
                        player.constraints = RigidbodyConstraints.None;
                        break;
                }
                _gameState = value;
            }
        }
    }

    public void Pause_performed(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            switch (GameState)
            {
                case GameState.Playing:
                    GameState = GameState.Paused;
                    break;
                case GameState.Paused:
                    GameState = GameState.Playing;
                    break;
                default:
                    break;
            }
        }
    }

    public void Reset_performed(InputAction.CallbackContext obj)
    {
        if (obj.started && GameState == GameState.Playing || GameState == GameState.Finished)
        {
            foreach (IResetable item in FindObjectsOfType<MonoBehaviour>().OfType<IResetable>())
            {
                item.ResetToInitial();
            }
            GameState = GameState.Starting;
        }
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        camController = GetComponent<CameraController>();
        countdownAudio = GetComponent<AudioSource>();
        GameState = GameState.Starting;
        gameTimeLeftSeconds = initialGameTimeLeft;
        timerInitialFontSize = timeLeftText.fontSize;
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        pauseCanvas.alpha = Mathf.Lerp(pauseCanvas.alpha, GameState == GameState.Paused ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        if (GameState == GameState.Playing)
        {
            gameTimeLeftSeconds -= Time.deltaTime;
        }
        if (gameTimeLeftSeconds <= 0)
        {
            // penalty points
            PointsManager.instance.multiplier = 0; // no points gained ever
            PointsManager.instance.AddUnscaledPoints(-5 * Time.deltaTime);

            timeLeftText.text = "FINISH NOW!";
            timeLeftText.fontSize = timerInitialFontSize - Mathf.Sin(gameTimeLeftSeconds * 5);
        }
        else
        {
            timeLeftText.fontSize = timerInitialFontSize;
            timeLeftText.text = string.Format("{0,2}:{1,2}.{2,3}", ((int)(gameTimeLeftSeconds / 60f)).ToString().PadLeft(4, ' '), ((int)(gameTimeLeftSeconds % 60f)).ToString().PadLeft(2, '0'), ((int)((gameTimeLeftSeconds - (int)gameTimeLeftSeconds) * 1000)).ToString().PadLeft(3, '0'));
        }
        if (GameState == GameState.Finished)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 63, Time.deltaTime / 0.5f);
        }
        else
        {
            camera.fieldOfView = 60;
        }

    }

    private IEnumerator ThreeTwoOneGo()
    {
        startAnim.SetTrigger("Start");
        float time = 0.5f;
        countdownAudio.pitch = 1f;
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.pitch = 1.1f;
        countdownAudio.PlayOneShot(countdownAudio.clip);
        GameState = GameState.Playing;
        startingCoroutine = null;
    }
}
