using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ResultsManager : MonoBehaviour
{
    [System.Serializable]
    public class RankingEntry
    {
        public string playerName;
        public int score;
    }

    public TMP_Text[] nameTexts;
    public TMP_Text[] scoreTexts;

    private List<RankingEntry> ranking = new List<RankingEntry>();

    void Start()
    {
        GenerateRanking();
        ShowRanking();
    }

    void GenerateRanking()
    {
        ranking.Add(new RankingEntry { playerName = "CPU_Ana:", score = 400 });
        ranking.Add(new RankingEntry { playerName = "CPU_Luis:", score = 300 });
        ranking.Add(new RankingEntry { playerName = "CPU_Sara:", score = 200 });
        ranking.Add(new RankingEntry { playerName = "CPU_Juan:", score = 100 });

        ranking.Add(new RankingEntry { playerName = "TÚ:", score = GameData.score });

        ranking = ranking.OrderByDescending(x => x.score).ToList();
    }

    void ShowRanking()
    {

        for (int i = 0; i < ranking.Count; i++)
        {
            nameTexts[i].text = "# " + (i + 1) + " " + ranking[i].playerName;
            scoreTexts[i].text = ranking[i].score + " Pts";

            if (ranking[i].playerName == "TÚ:")
            {
                nameTexts[i].color = Color.yellow;
                scoreTexts[i].color = Color.yellow;
            }
        }



    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}