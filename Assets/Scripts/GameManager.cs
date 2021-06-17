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
    Starting,
    Menu,
}

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    public Rigidbody player;

    public CanvasGroup pauseCanvas;
    public CanvasGroup menuCanvas;
    public TextMeshProUGUI seedText;
    public CanvasGroup gameCanvas;

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

    public MapGenerator generator;

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
                    case GameState.Menu:
                        camController.enabled = true;
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // freeze player
                        player.constraints = RigidbodyConstraints.FreezeAll;
                        player.gameObject.GetComponent<CarControllerNew>().ControlsEnabled = false;

                        break;
                    case GameState.Starting:
                        foreach (IResetable item in FindObjectsOfType<MonoBehaviour>().OfType<IResetable>())
                        {
                            item.ResetToInitial();
                        }
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

                        GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
                        if (respawn != null)
                        {
                            player.transform.position = respawn.transform.position;
                            player.transform.rotation = respawn.transform.rotation;
                        }
                        else
                        {
                            Debug.LogError("Respawn position not found");
                        }

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
            GameState = GameState.Starting;
        }
    }

    public IEnumerator ChangeStateAfter(GameState nextState)
    {
        yield return new WaitForSeconds(0.1f);
        GameState = nextState;
    }

    public void GenerateNewMapRandom()
    {
        generator.Generate(Random.Range(int.MinValue, int.MaxValue));
        StartCoroutine(ChangeStateAfter(GameState.Starting));
    }

    public void GenerateNewMapFromSeed()
    {
        generator.Generate(seedText.text.GetHashCode());
        StartCoroutine(ChangeStateAfter(GameState.Starting));
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        camController = GetComponent<CameraController>();
        countdownAudio = GetComponent<AudioSource>();
        GameState = GameState.Menu;
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
        pauseCanvas.interactable = GameState == GameState.Paused;
        menuCanvas.alpha = Mathf.Lerp(menuCanvas.alpha, GameState == GameState.Menu ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        menuCanvas.interactable = GameState == GameState.Menu;
        //gameCanvas.alpha = Mathf.Lerp(gameCanvas.alpha, GameState != GameState.Finished && GameState != GameState.Menu ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        gameCanvas.alpha = Mathf.Lerp(gameCanvas.alpha, GameState != GameState.Menu ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        gameCanvas.interactable = GameState == GameState.Playing;
        if (GameState == GameState.Playing)
        {
            gameTimeLeftSeconds -= Time.deltaTime;
        }
        if (gameTimeLeftSeconds <= 0)
        {
            // penalty points
            PointsManager.instance.multiplier = 0; // no points gained ever
            if(GameState != GameState.Finished)
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
