using UnityEngine;

public class TeamIdentity : MonoBehaviour
{
    [SerializeField] private TeamData teamData;

    public TeamData TeamData
    {
        get => teamData;
        set => teamData = value;
    }
    
}
