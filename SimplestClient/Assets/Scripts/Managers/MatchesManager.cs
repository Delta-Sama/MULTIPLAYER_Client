using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchesManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject MatchButtonPrefab;

    [Header("UI Elements")]
    [SerializeField] private GameObject MatchesContent;

    public static MatchesManager Instance;

    private List<MatchData> matches;

    void Awake()
    {
        Instance = this;

        matches = new List<MatchData>();
    }

    public void RefreshMatchList()
    {
        matches.Clear();

        NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.GetMatchesList + ",");
    }

    public void AddMatch(int matchId, string matchName)
    {
        MatchData data = new MatchData();
        data.matchId = matchId;
        data.matchName = matchName;

        GameObject matchButton = Instantiate(MatchButtonPrefab, MatchesContent.transform);
        matchButton.transform.Find("MatchName").GetComponent<Text>().text = matchName;

        data.matchButton = matchButton;

        matches.Add(data);
    }
}
