using UnityEngine;

[CreateAssetMenu(fileName = "New Team Data", menuName = "Combat/Team", order = 0)]
public class TeamData : ScriptableObject
{
    [SerializeField] private string teamName = "";
    [SerializeField] private Color teamColor;

    public string TeamName => teamName;

    public Color TeamColor => teamColor;
}
