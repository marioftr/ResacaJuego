using System.Collections;
using UnityEngine;

public class PuertaCerrada : ObjetoInteractuable
{
    private string[] _Frase =
    {
        "Parece que está cerrada.",
        "No consigo abrirla.",
        "¿Habrá alguna forma de abrirla?",
        "¡Ábrete, sésamo!"
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
