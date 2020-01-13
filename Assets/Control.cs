using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    int tenths;
    int seconds;
    int minutes;
    float counter;
    bool timerEnabled;

    // Cached references
    public Text tenthsText;
    public Text secondsText;
    public Text minutesText;
    public Button playPauseButton;
    public Button resetButton;
    public Image image;
    public Slider loadImageProgressSlider;
    public GameObject waitLoadSpinner;

    //
    UnityWebRequest httpClient;
    private bool loadImageProgressEnabled = false;

    public void LoadImageBlocking()
    {
        using (httpClient = new UnityWebRequest("https://spdvistorage.blob.core.windows.net/clickycrates-blobs/default/4-2-pokemon-transparent.png"))
        {
            Debug.Log("Getting image...");
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            httpClient.SendWebRequest(); // blocking call?
            while (httpClient.downloadProgress < 1.0f)
            {
                Thread.Sleep(10);
                Debug.Log(httpClient.downloadProgress * 100 + "% (Bytes downloaded: " + httpClient.downloadedBytes/1024 + " KB");
            }
            //Thread.Sleep(1000);
            Debug.Log("hpptClient.isDone = " + httpClient.isDone);
            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                Debug.Log(httpClient.error);
            }
            else
            {
                byte[] textureBinary = httpClient.downloadHandler.data;
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(textureBinary);
                image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }

    public void LoadImageCoroutine()
    {
        StartCoroutine(LoadImageNonBlocking());
        Debug.Log("After coroutine called");
    }

    public IEnumerator LoadImageNonBlocking()
    {
        using (httpClient = new UnityWebRequest("https://spdvistorage.blob.core.windows.net/clickycrates-blobs/default/4-2-pokemon-transparent.png"))
        {
            Debug.Log("Getting image...");
            
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            
            loadImageProgressEnabled = true;
            waitLoadSpinner.SetActive(true);
            yield return new WaitForSeconds(3); // Unneeded wait just to see spinner spin during 3 seconds

            yield return httpClient.SendWebRequest(); // blocking call?
            Debug.Log("hpptClient.isDone = " + httpClient.isDone);

            loadImageProgressEnabled = false;
            waitLoadSpinner.SetActive(false);
            loadImageProgressSlider.value = 1.0f;

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                Debug.Log(httpClient.error);
            }
            else
            {
                byte[] textureBinary = httpClient.downloadHandler.data;
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(textureBinary);
                image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerEnabled)
        {
            //Debug.Log(counter);
            counter += Time.deltaTime;
            if (counter > 0.1f)
            {
                counter = 0f;
                tenths++;
                tenthsText.text = tenths.ToString();
                if (Mathf.FloorToInt(tenths) > 9)
                {
                    tenths = 0;
                    seconds++;
                    secondsText.text = seconds.ToString("00");
                    if (seconds > 59)
                    {
                        seconds = 0;
                        minutes++;
                        minutesText.text = minutes.ToString("00");
                    }
                }
            }
        }

        if (loadImageProgressEnabled)
        {
            if (httpClient.downloadProgress < 1.0f)
            {
                //Thread.Sleep(10);
                Debug.Log(httpClient.downloadProgress * 100 + "% (Bytes downloaded: " + httpClient.downloadedBytes / 1024 + " KB");
                loadImageProgressSlider.value = httpClient.downloadProgress;
            }
            //Thread.Sleep(1000);
        }

    }

    public void PlayPause()
    {
        timerEnabled = !timerEnabled;
        if (timerEnabled)
        {
            playPauseButton.gameObject.GetComponentInChildren<Text>().text = "Pause";
        }
        else
        {
            playPauseButton.gameObject.GetComponentInChildren<Text>().text = "Play";
        }
    }

    public void Reset()
    {
        tenths = 0;
        seconds = 0;
        minutes = 0;
        counter = 0f;
        timerEnabled = false;
        tenthsText.text = tenths.ToString("0");
        secondsText.text = seconds.ToString("00");
        minutesText.text = minutes.ToString("00");
    }



}
