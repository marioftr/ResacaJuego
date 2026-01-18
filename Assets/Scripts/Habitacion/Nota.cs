using System.Collections;
using UnityEngine;

public class Nota : ObjetoInteractuable
{
    [SerializeField] private string _Frase;
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
        _TextoFrase.text = _Frase;
        yield return new WaitForSeconds(5f);
        _TextoFrase.text = "";
    }
}
