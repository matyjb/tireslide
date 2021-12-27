using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SendFormData : MonoBehaviour
{
    public Slider[] formSliders;
    public MapGenerator mapGenerator;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdVgQ6l63xsQIWUWLt0GAa9hYLRhIaOXvNCoQsV2PX4lpAI5w/formResponse";

    IEnumerator Post(int difficulty, int lengthReview, int boring, int lenght, int seed, int countCubeStacks, int countScoreGates, int countComboGates)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.114721724", difficulty.ToString());
        form.AddField("entry.1885154946", lengthReview.ToString());
        form.AddField("entry.2051931939", boring.ToString());
        form.AddField("entry.1676969810", seed.ToString());
        form.AddField("entry.1118262487", lenght.ToString());
        form.AddField("entry.387613298", countCubeStacks.ToString());
        form.AddField("entry.145226367", countScoreGates.ToString());
        form.AddField("entry.1845304051", countComboGates.ToString());
        byte[] ramData = form.data;
        WWW WWW = new WWW(BASE_URL, ramData);
        yield return WWW;
    }

    public void Send()
    {
        Debug.Log("Sedning stuff");
        int n = mapGenerator.lastGeneratedMapLenght;
        int seed = mapGenerator.lastGeneratedMapSeed;
        int countCubeStacks = mapGenerator.CountCubesStacks();
        int countScoreGates = mapGenerator.CountScoreGates();
        int countComboGates = mapGenerator.CountComboGates();
        // do the stuff
        StartCoroutine(Post((int)formSliders[0].value, (int)formSliders[1].value, (int)formSliders[2].value, n,seed, countCubeStacks, countScoreGates, countComboGates));

        GameManager.instance.GameState = GameState.Finished;
    }
}
