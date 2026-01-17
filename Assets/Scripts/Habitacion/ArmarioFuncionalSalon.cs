using System;
using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class ArmarioFuncionalSalon : ObjetoInteractuable
{
    [SerializeField] private Transform _Transform;
    [SerializeField] private Vector3 _PosicionPuertaAbierta = new Vector3(0.38f, 0f,4.350f);
    [SerializeField] private Vector3 _PosicionPuertaCerrada = new Vector3(0.38f, 0f, 4.05f);

    private string[] _Frase = {""};

    private void Start()
    {
        if (GestorBase.Instancia.ArmarioFuncionalSalon)
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
        _ObjetoOn = true;
        GestorBase.Instancia.ArmarioFuncionalSalon = true;
    }
    private void CerrarPuerta()
    {
        _Transform.position = _PosicionPuertaCerrada;
        _ObjetoOn = false;
        GestorBase.Instancia.ArmarioFuncionalSalon = false;
    }
}
