using UnityEngine;
using System.Collections;

public class PuertaAbierta : ObjetoInteractuable
{
    private Transform _Transform;
    [SerializeField] private Vector3 _RotacionAbierta = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _RotacionCerrada = new Vector3(0, -90, 0);

    private string[] _Frase = {""};

    private void Awake()
    {
        _Transform = transform.parent.parent;
    }
    protected override void ObjetoOn()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOn);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _Transform.eulerAngles = _RotacionAbierta;
        _ObjetoOn = true;
    }
    protected override void ObjetoOff()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOff);
        StopAllCoroutines();
        StartCoroutine(Texto());
        _Transform.eulerAngles = _RotacionCerrada;
        _ObjetoOn = false;
    }
    protected override IEnumerator Texto()
    {
        _TextoFrase.text = "";
        yield return null;
    }
}
