using System;
using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class PuertaFuncionalBaño : ObjetoInteractuable
{
    private Transform _Transform;
    [SerializeField] private Vector3 _RotacionPuertaAbierta = new Vector3(0, 180, 0);
    [SerializeField] private Vector3 _RotacionPuertaCerrada = new Vector3(0, 90, 0);

    private string[] _Frase = {""};

    private void Awake()
    {
        _Transform = transform.parent.parent;
    }

    private void Start()
    {
        if (GestorBase.Instancia.PuertaAbiertaBaño)
        {
            AbrirPuerta();
        }
        else
        {
            CerrarPuerta();
        }
    }

    protected override void ObjetoOn()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOn);
        StopAllCoroutines();
        StartCoroutine(Texto());
        AbrirPuerta();
    }
    protected override void ObjetoOff()
    {
        GestorEfectosSonido.ReproducirEfecto(_EfectoOff);
        StopAllCoroutines();
        StartCoroutine(Texto());
        CerrarPuerta();
    }
    protected override IEnumerator Texto()
    {
        _TextoFrase.text = "";
        yield return null;
    }
    
    private void AbrirPuerta()
    {
        _Transform.eulerAngles = _RotacionPuertaAbierta;
        _ObjetoOn = true;
        GestorBase.Instancia.PuertaAbiertaBaño = true;
    }
    private void CerrarPuerta()
    {
        _Transform.eulerAngles = _RotacionPuertaCerrada;
        _ObjetoOn = false;
        GestorBase.Instancia.PuertaAbiertaBaño = false;
    }
}
