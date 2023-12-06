using UnityEngine;
using Euler;
using System.Collections.Generic;

[System.Serializable]
public class UsernameGenerator
{
#if UNITY_EDITOR

    [SerializeField]
    Parse[] autoParses;

    public void Execute()
    {
        totalList.Clear();
        totalList.Capacity = 0;

        for (int i = 0; i < autoParses.Length; i++)
        {
            autoParses[i].Execute();

            if (maxCount < autoParses[i].DataOriginalOrder[0].Length)
                maxCount = autoParses[i].DataOriginalOrder[0].Length;

            totalList.AddRange(autoParses[i].DataOriginalOrder);
            autoParses[i].Clear();
        }
    }

#endif

    [SerializeField]
    List<PDO<string, string>> totalList = new List<PDO<string, string>>();

    int maxCount;

    public string GenerateUsername()
    {
        string newUserName = "";

        for (int i = 1; i < maxCount; i++)
        {
            int rng = Random.Range(0, totalList.Count);

            if(i < totalList[rng].Length)
                newUserName += totalList[rng][i].ToString();
        }

        return newUserName;
    }
}
