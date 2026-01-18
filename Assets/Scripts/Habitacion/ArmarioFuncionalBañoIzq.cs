using System;
using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class ArmarioFuncionalBañoIzq : ObjetoInteractuable
{
    [SerializeField] private Transform _Transform;
    [SerializeField] private Vector3 _RotacionPuertaAbierta;
    [SerializeField] private Vector3 _RotacionPuertaCerrada;
    private string[] _Frase = {""};

    private void Awake()
    {
        _Transform = transform;
    }
    private void Start()
    {
        if (GestorBase.Instancia.ArmarioFuncionalBañoIzq)
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
        _Transform.localEulerAngles = _RotacionPuertaAbierta;
        _ObjetoOn = true;
        GestorBase.Instancia.ArmarioFuncionalBañoIzq = true;
    }
    private void CerrarPuerta()
    {
        _Transform.localEulerAngles = _RotacionPuertaCerrada;
        _ObjetoOn = false;
        GestorBase.Instancia.ArmarioFuncionalBañoIzq = false;
    }
}
