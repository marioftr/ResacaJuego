using System.Collections;
using UnityEngine;

public class Guitarra : MinijuegoInteractuable
{
    private string[] _Frase =
    {
        "Es una guitarra.",
        "Es una guitarra.",
        "Es una guitarra.",
        "No sé tocar la guitarra.",
        "Aprendí a tocar la flauta dulce en el cole.",
        "¿Por qué hay una guitarra en el salón?",
        "La música hace que me duela la cabeza.",
        "¿Y si me toco algo?",
        "Solo me sé 'Cumpleaños feliz' con la flauta.",
        "Solo me sé 'Cumpleaños feliz' con la flauta.",
        "Solo me sé 'Cumpleaños feliz' con la flauta."
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
        _ObjetoOn = false;
    }
}
