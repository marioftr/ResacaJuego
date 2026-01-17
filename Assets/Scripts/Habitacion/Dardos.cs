using System.Collections;
using UnityEngine;

public class Dardos : MinijuegoInteractuable
{
    private string[] _Frase =
    {
        "Es una diana.",
        "Es una diana.",
        "Es una diana.",
        "Robé esta diana el mismo día que el cono.",
        "No soy especialmente bueno a los dardos.",
        "Ahora recuerdo... Ayer jugamos a los dardos.",
        "Anoche me bebí hasta los dardos.",
        "Me apetece jugar una partida."
    };
    
    protected override void ObjetoOn()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOn);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _ObjetoOn = true;
    }
    protected override void ObjetoOff()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOff);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _ObjetoOn = false;
    }

    protected override IEnumerator Texto()
    {
        int indice = Random.Range(0, _Frase.Length);
        while (indice == _IndiceTextoAnterior && _Frase.Length > 1)
        {
            indice = Random.Range(0, _Frase.Length);
        }
        _IndiceTextoAnterior = indice;
        _TextoFrase.text = _Frase[indice];
        yield return new WaitForSeconds(3f);
        _TextoFrase.text = "";
    }
}
