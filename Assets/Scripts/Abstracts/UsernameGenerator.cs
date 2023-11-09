
using UnityEngine;
using Euler;



public class UsernameGenerator
{

    
    Parse _autoParse;

    public string GenerateNewUsername()
    {
        _autoParse?.Execute();
        string newUserName = "";

        newUserName += _autoParse.DataOriginalOrder[Random.Range(0, 21)][2].ToString();
        newUserName += _autoParse.DataOriginalOrder[Random.Range(0, 21)][3].ToString();
        newUserName += _autoParse.DataOriginalOrder[Random.Range(0, 21)][4].ToString();
        newUserName += _autoParse.DataOriginalOrder[Random.Range(0, 21)][5].ToString();

        _autoParse.DataOriginalOrder.Clear();

        return newUserName;
    }

   public UsernameGenerator(Parse usernameParse)
    {
        _autoParse = usernameParse;
    }
}
