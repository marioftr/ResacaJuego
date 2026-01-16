using System.Collections;
using UnityEngine;

public class LucesHab : ObjetoInteractuable
{
    [SerializeField] private Light _Bombilla;
    private string[] _Frase ={""};
    
    protected override void ObjetoOn()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOn);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _Bombilla.enabled = true;
        _ObjetoOn = true;
    }
    protected override void ObjetoOff()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOff);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _Bombilla.enabled = false;
        _ObjetoOn = false;
    }

    protected override IEnumerator Texto()
    {
        _TextoFrase.text = "";
        yield return null;
    }
}
