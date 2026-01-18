using System;
using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class DuchaFuncionalIzq : ObjetoInteractuable
{
    [SerializeField] private Transform _Transform;
    [SerializeField] private Vector3 _PosicionPuertaAbierta;
    [SerializeField] private Vector3 _PosicionPuertaCerrada;
    private string[] _Frase = {""};

    private void Start()
    {
        if (GestorBase.Instancia.DuchaFuncionalIzq)
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
        _Transform.position = _PosicionPuertaAbierta;
        _Transform.position = _PosicionPuertaAbierta;
        _ObjetoOn = true;
        GestorBase.Instancia.DuchaFuncionalIzq = true;
    }
    private void CerrarPuerta()
    {
        _Transform.position = _PosicionPuertaCerrada;
        _Transform.position = _PosicionPuertaCerrada;
        _ObjetoOn = false;
        GestorBase.Instancia.DuchaFuncionalIzq = false;
    }
}
