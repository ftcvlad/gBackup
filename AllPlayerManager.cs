using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlayerManager : MonoBehaviour {

    static List<Player> allActivePlayers = new List<Player>();

    public static void addPlayer(Player p) {
        allActivePlayers.Add(p);

        p.setTeamId(allActivePlayers.Count);//temporary!
    }
}
