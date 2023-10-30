using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PopUpResult : PopUp
{
    [SerializeField]
    TMPro.TextMeshProUGUI damageNumber;

    [SerializeField]
    TMPro.TextMeshProUGUI viwersNumber;

    [SerializeField]
    float timeToShow=1;

    private void Awake()
    {
        eventManager.events.SearchOrCreate<EventParam<(int, int)>>("finishday").delegato += PopUp;
    }

    void PopUp((int damage, int viewers) resultado)
    {
        onActive.Invoke();
        callsManager.DestroyAll();
        StartCoroutine(ViewerNumber(resultado));
    }

    IEnumerator ViewerNumber((int damage, int viewers) resultado)
    {
        damageNumber.text = string.Empty;

        viwersNumber.text = "0";

        for (int i = 1; i <= resultado.viewers; i++)
        {
            yield return new WaitForSeconds(timeToShow / resultado.viewers);

            viwersNumber.text = i.ToString();
        }

        damageNumber.text = resultado.damage.ToString();

        if (resultado.damage < 0)
            damageNumber.text = damageNumber.text.RichText("color", "red");
        else
            damageNumber.text = damageNumber.text.RichText("color", "green");


        
        callsManager.Create("Exit", Application.Quit);

        
        callsManager.Create("Reset", ()=> SceneManager.LoadScene(SceneManager.GetActiveScene().name));

        
    }
}
