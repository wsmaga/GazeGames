using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GazumaGameManager : MonoBehaviour
{

    [SerializeField]
    private Canvas LoadingScreen;

    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private GameObject spawner;

    [SerializeField]
    private GameObject indicator;

    private VoiceControllerInterface voiceControllerInterface;

    private const float SPAWN_DELAY = 2.0f;
    private double spawnTimer = 0.0;
    private const float SPAWN_RADIUS = 0.5f;
    private LinkedList<GameObject> spawnedBalls = new LinkedList<GameObject>();
    private GameObject newBall;
    private const int MAX_BALLS = 20;
    private int numberOfBalls = 0;

    private const float BALL_SPEED = 0.15f;
    private float movementTimer = 0.0f;

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

    private Color indicatorColor;

    private void Start()
    {
        LoadingScreen.enabled = false;

        voiceControllerInterface = GetComponentInChildren<VoiceControllerInterface>();

        spawner.transform.localPosition = new Vector3(
            SPAWN_RADIUS,
            0.0f,
            1.0f
        );

        SetIndicatorColor(colors[Random.Range(0, colors.Length)]);

        voiceControllerInterface.StartListening();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (numberOfBalls < MAX_BALLS && spawnTimer > SPAWN_DELAY)
        {
            spawnTimer = 0.0;
            SpawnNewBall();
        }

        MoveBalls();
    }

    public void OnVoiceRecognizerResults(string results)
    {
        string lowercase = results.ToLower();
        int pickedNumber = 0;
        switch (lowercase)
        {
            case string a when a.Contains("wyjście") || a.Contains("wyjdź") || a.Contains("koniec"):
                LoadingScreen.enabled = true;
                SceneManager.LoadScene("MainMenu");
                break;
            case string b when b.Contains("raz") || b.Contains("jeden") || b.Contains("1"):
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
            case string i when i.Contains("skip") || i.Contains("dalej"):
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

        if (pickedNumber != 0)
        {
            LinkedListNode<GameObject> ptr = spawnedBalls.First;
            while (ptr != null)
            {
                GazumaBall currentBall = ptr.Value.GetComponent<GazumaBall>();
                if (MatchesNumberAndColor(currentBall, pickedNumber))
                {
                    if (ExistsAndMatchesColor(ptr.Next))
                    {
                        movementTimer -= SPAWN_DELAY;
                        Destroy(ptr.Next.Value);
                        spawnedBalls.Remove(ptr.Next);
                    }
                    if (ExistsAndMatchesColor(ptr.Previous))
                    {
                        movementTimer -= SPAWN_DELAY;
                        Destroy(ptr.Previous.Value);
                        spawnedBalls.Remove(ptr.Previous);
                    }
                    movementTimer -= SPAWN_DELAY;
                    Destroy(ptr.Value);
                    spawnedBalls.Remove(ptr);
                    break;
                }
                ptr = ptr.Next;
            }
        }

        voiceControllerInterface.StartListening();
    }

    private bool ExistsAndMatchesColor(LinkedListNode<GameObject> node)
    {
        return node != null &&
            node.Value.GetComponent<GazumaBall>().GetColor().Equals(indicatorColor);
    }

    private bool MatchesNumberAndColor(GazumaBall currentBall, int pickedNumber)
    {
        return currentBall.GetColor().Equals(indicatorColor) &&
            currentBall.GetNumber().Equals(pickedNumber);
    }

    private void MoveBalls()
    {
        movementTimer += Time.deltaTime;
        int i = 1;
        float t = movementTimer * BALL_SPEED;
        foreach (var ball in spawnedBalls)
        {
            float delay = BALL_SPEED * SPAWN_DELAY * i;
            ball.transform.localPosition = new Vector3(
                SPAWN_RADIUS * Mathf.Cos(t - delay),
                SPAWN_RADIUS * Mathf.Sin(t - delay),
                1.0f
            );
            i++;
        }
    }

    private void SpawnNewBall()
    {
        newBall = Instantiate(ballPrefab, transform);
        spawnedBalls.AddLast(newBall);
        numberOfBalls++;
    }

    private void SetIndicatorColor(Color color)
    {
        indicatorColor = color;
        indicator.GetComponent<GazumaIndicator>().SetColor(indicatorColor);
    }

}
