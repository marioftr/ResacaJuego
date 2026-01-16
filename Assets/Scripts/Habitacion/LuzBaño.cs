using System;
using System.Collections;
using UnityEngine;

public class LuzBaño : ObjetoInteractuable
{
    [SerializeField] private Light _Bombilla;
    [SerializeField] private GameObject _Lampara;
    private Transform _Transform;
    private string[] _Frase ={""};
    
    private void Awake()
    {
        _Transform = transform;
    }
    private void Start()
    {
        if (GestorBase.Instancia.LuzBañoOn)
        {
            EncenderLuz();
        }
        else
        {
            ApagarLuz();
        }
    }

    protected override void ObjetoOn()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOn);
        StopAllCoroutines();
        StartCoroutine(Texto());
        EncenderLuz();
    }
    protected override void ObjetoOff()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOff);
        StopAllCoroutines();
        StartCoroutine(Texto());
        ApagarLuz();
    }

    protected override IEnumerator Texto()
    {
        _TextoFrase.text = "";
        yield return null;
    }
    
    private void EncenderLuz()
    {
        _Bombilla.enabled = true;
        _Lampara.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        _ObjetoOn = true;
        _Transform.position = new Vector3(-3.815f, 1f, 2.213f);
        _Transform.eulerAngles = new Vector3(-10f, -90f, 180f);
        GestorBase.Instancia.LuzBañoOn = true;
    }
    private void ApagarLuz()
    {
        _Bombilla.enabled = false;
        _Lampara.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        _ObjetoOn = false;
        _Transform.position = new Vector3(-3.815f, 1.08f, 2.2f);
        _Transform.eulerAngles = new Vector3(10f, -90f, 0);
        GestorBase.Instancia.LuzBañoOn = false;
    }
}
