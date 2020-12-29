using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gazuma minigame game manager
/// </summary>
public class GazumaGameManager : MonoBehaviour
{

    /// <summary>
    /// Loading screen to be displayed when something is loading by the manager
    /// </summary>
    [SerializeField]
    private Canvas LoadingScreen;

    /// <summary>
    /// Base prefab for the target object
    /// </summary>
    [SerializeField]
    private GameObject ballPrefab;

    /// <summary>
    /// Targets' spawner
    /// </summary>
    [SerializeField]
    private GameObject spawner;

    /// <summary>
    /// Central color indicator's object
    /// </summary>
    [SerializeField]
    private GameObject indicator;

    /// <summary>
    /// Voice recognizer interface
    /// </summary>
    private VoiceControllerInterface voiceControllerInterface;

    /// <summary>
    /// Time before spawning another target
    /// </summary>
    private const float SPAWN_DELAY = 2.0f;
    /// <summary>
    /// Time since spawning last target
    /// </summary>
    private double spawnTimer = 0.0;
    /// <summary>
    /// Radius of the target's circle
    /// </summary>
    private const float TARGETS_PATH_RADIUS = 0.5f;
    /// <summary>
    /// References to all the spawned targets
    /// </summary>
    private LinkedList<GameObject> spawnedTargets = new LinkedList<GameObject>();

    /// <summary>
    /// Max number of spawned targets
    /// </summary>
    private const int MAX_TARGETS = 20;
    /// <summary>
    /// Current number of spawned targets
    /// </summary>
    private int numberOfTargets = 0;

    /// <summary>
    /// Targets' movement speed
    /// </summary>
    private const float BALL_SPEED = 0.15f;
    /// <summary>
    /// Time since game's start
    /// </summary>
    private float movementTimer = 0.0f;

    /// <summary>
    /// Possible targets' colors
    /// </summary>
    private Color[] colors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.white
    };

    /// <summary>
    /// Current indicator's color
    /// </summary>
    private Color indicatorColor;

    private void Start()
    {
        LoadingScreen.enabled = false;  // no loading visible at the start

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();  // initialize voice recognizer interface

        spawner.transform.localPosition = new Vector3(  // move the spawner to the proper position
            TARGETS_PATH_RADIUS,
            0.0f,
            1.0f
        );

        SetIndicatorColor(colors[Random.Range(0, colors.Length)]);  // give it first color

        voiceControllerInterface.StartListening();  // start listening for commands
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (numberOfTargets < MAX_TARGETS && spawnTimer > SPAWN_DELAY)  // is it time to spawn another target?
        {
            spawnTimer = 0.0;
            SpawnNewBall();
        }

        MoveBalls();  // update targets' positions
    }

    /// <summary>
    /// Parse voice recognizer results
    /// </summary>
    /// <param name="results"> Results string </param>
    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        int pickedNumber = 0;
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):  // closing the minigame
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("MainMenu");
                break;
            case string b when b.Contains("raz") || b.Contains("jeden") || b.Contains("1"):  // picking the target
                pickedNumber = 1;
                break;
            case string c when c.Contains("dwa") || c.Contains("2"):
                pickedNumber = 2;
                break;
            case string d when d.Contains("trzy") || d.Contains("3") || d.Contains("czy"):
                pickedNumber = 3;
                break;
            case string e when e.Contains("cztery") || e.Contains("4"):
                pickedNumber = 4;
                break;
            case string f when f.Contains("pięć") || f.Contains("5"):
                pickedNumber = 5;
                break;
            case string g when g.Contains("sześć") || g.Contains("6"):
                pickedNumber = 6;
                break;
            case string h when h.Contains("siedem") || h.Contains("7"):
                pickedNumber = 7;
                break;
            case string i when i.Contains("skip") || i.Contains("dalej") || i.Contains("kolor"):  // changing the indicator's color
                Color newColor;
                do
                {
                    newColor = colors[Random.Range(0, colors.Length)];
                } while (newColor.Equals(indicatorColor));
                indicatorColor = newColor;
                SetIndicatorColor(indicatorColor);
                break;
            default:
                break;
        }

        if (pickedNumber != 0)  // removing the scored target(s)
        {
            LinkedListNode<GameObject> ptr = spawnedTargets.First;
            while (ptr != null)
            {
                GazumaBall currentBall = ptr.Value.GetComponent<GazumaBall>();
                if (MatchesNumberAndColor(currentBall, pickedNumber))
                {
                    if (ExistsAndMatchesColor(ptr.Next))  // destroy matching neighbours
                    {
                        movementTimer -= SPAWN_DELAY;
                        Destroy(ptr.Next.Value);
                        spawnedTargets.Remove(ptr.Next);
                    }
                    if (ExistsAndMatchesColor(ptr.Previous))
                    {
                        movementTimer -= SPAWN_DELAY;
                        Destroy(ptr.Previous.Value);
                        spawnedTargets.Remove(ptr.Previous);
                    }
                    movementTimer -= SPAWN_DELAY;  // destroy scored target
                    Destroy(ptr.Value);
                    spawnedTargets.Remove(ptr);
                    break;
                }
                ptr = ptr.Next;
            }
        }

        voiceControllerInterface.StartListening();  // listen for next voice commands
    }

    /// <summary>
    /// Returns true if at the node there is a target with color matching with the indicator
    /// </summary>
    /// <param name="node">LinkedListNode at which there might be a target</param>
    /// <returns></returns>
    private bool ExistsAndMatchesColor(LinkedListNode<GameObject> node)
    {
        return node != null &&
            node.Value.GetComponent<GazumaBall>().GetColor().Equals(indicatorColor);
    }

    /// <summary>
    /// Returns true if currentBall matches indicator's color and pickedNumber
    /// </summary>
    /// <param name="currentBall">Current target being checked</param>
    /// <param name="pickedNumber">Currently picked number</param>
    /// <returns></returns>
    private bool MatchesNumberAndColor(GazumaBall currentBall, int pickedNumber)
    {
        return currentBall.GetColor().Equals(indicatorColor) &&
            currentBall.GetNumber().Equals(pickedNumber);
    }

    /// <summary>
    /// Update all targets' positions
    /// </summary>
    private void MoveBalls()
    {
        movementTimer += Time.deltaTime;
        int i = 1;
        float t = movementTimer * BALL_SPEED;
        foreach (var ball in spawnedTargets)
        {
            float delay = BALL_SPEED * SPAWN_DELAY * i;
            ball.transform.localPosition = new Vector3(
                TARGETS_PATH_RADIUS * Mathf.Cos(t - delay),
                TARGETS_PATH_RADIUS * Mathf.Sin(t - delay),
                1.0f
            );
            i++;
        }
    }

    /// <summary>
    /// Spawn a new target and add it to the list
    /// </summary>
    private void SpawnNewBall()
    {
        GameObject newBall = Instantiate(ballPrefab, transform);
        spawnedTargets.AddLast(newBall);
        numberOfTargets++;
    }

    /// <summary>
    /// Setting a new color for the indicator
    /// </summary>
    /// <param name="color">New color to set</param>
    private void SetIndicatorColor(Color color)
    {
        indicatorColor = color;
        indicator.GetComponent<GazumaIndicator>().SetColor(indicatorColor);
    }

}
