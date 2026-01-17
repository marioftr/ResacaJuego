using System;
using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class CabineteFuncionalSalon : ObjetoInteractuable
{
    [SerializeField] private Transform _Transform;
    [SerializeField] private Vector3 _PosicionPuertaAbierta = new Vector3(-1.14f, 1.8f,4.7f);
    [SerializeField] private Vector3 _PosicionPuertaCerrada = new Vector3(-0.66f, 1.8f, 4.7f);

    private string[] _Frase = {""};

    private void Start()
    {
        if (GestorBase.Instancia.CabineteFuncionalSalon)
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
        GestorBase.Instancia.CabineteFuncionalSalon = true;
    }
    private void CerrarPuerta()
    {
        _Transform.position = _PosicionPuertaCerrada;
        _ObjetoOn = false;
        GestorBase.Instancia.CabineteFuncionalSalon = false;
    }
}
