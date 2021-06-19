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
    public TextMeshProUGUI seedPauseText;
    public CanvasGroup gameCanvas;
    public CanvasGroup finishCanvas;

    public GameState _gameState;

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

    private int currentSeed = 0;

    public TextMeshProUGUI scoreFinishText;
    public TextMeshProUGUI seedFinishText;

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
                        PlaceCarOnStart();

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

                        scoreFinishText.text = string.Format("Score: {0}", Mathf.RoundToInt(PointsManager.instance.Points).ToString());
                        seedFinishText.text = string.Format("Seed: {0}", currentSeed.ToString());

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
        if(obj.started)
            RestartGame();
    }

    public IEnumerator ChangeStateAfter(GameState nextState)
    {
        yield return new WaitForSeconds(0.1f);
        GameState = nextState;
    }

    public void GenerateNewMapRandom()
    {
        GameState = GameState.Starting;
        currentSeed = Random.Range(0, int.MaxValue);
        generator.Generate(currentSeed);
        PlaceCarOnStart();
        seedPauseText.text = currentSeed.ToString();
        //StartCoroutine(ChangeStateAfter(GameState.Starting));
    }

    public void GenerateNewMapFromSeed()
    {
        GameState = GameState.Starting;
        if(!int.TryParse(seedText.text, out currentSeed))
        {
            currentSeed = seedText.text.GetHashCode();
        }
        generator.Generate(currentSeed);
        PlaceCarOnStart();
        seedPauseText.text = currentSeed.ToString();
        //StartCoroutine(ChangeStateAfter(GameState.Starting));
    }

    public void PlaceCarOnStart()
    {
        GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
        if (respawn != null)
        {
            player.transform.position = respawn.transform.position;
            player.transform.rotation = respawn.transform.rotation;
        }
        else
        {
            Debug.LogWarning("Respawn position not found");
        }
    }

    public void GoToMenu()
    {
        GameState = GameState.Menu;
    }

    public void RestartGame()
    {
        if (GameState == GameState.Playing || GameState == GameState.Finished)
        {
            GameState = GameState.Starting;
            PlaceCarOnStart();
        }
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
        pauseCanvas.interactable = pauseCanvas.blocksRaycasts = GameState == GameState.Paused;
        menuCanvas.alpha = Mathf.Lerp(menuCanvas.alpha, GameState == GameState.Menu ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        menuCanvas.interactable = menuCanvas.blocksRaycasts = GameState == GameState.Menu;
        gameCanvas.alpha = Mathf.Lerp(gameCanvas.alpha, GameState == GameState.Playing || GameState == GameState.Starting ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        gameCanvas.interactable = gameCanvas.blocksRaycasts = GameState == GameState.Playing || GameState == GameState.Starting;
        finishCanvas.alpha = Mathf.Lerp(finishCanvas.alpha, GameState == GameState.Finished ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
        finishCanvas.interactable = finishCanvas.blocksRaycasts = GameState == GameState.Finished;
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
