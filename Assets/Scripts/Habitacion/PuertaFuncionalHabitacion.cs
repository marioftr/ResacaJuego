using System;
using UnityEngine;
using System.Collections;

public class PuertaFuncionalHabitacion : ObjetoInteractuable
{
    private Transform _Transform;
    [SerializeField] private Vector3 _RotacionPuertaAbierta = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _RotacionPuertaCerrada = new Vector3(0, -90, 0);
    [SerializeField] private GameObject[] _Dardos;

    private string[] _Frase = {""};

    private void Awake()
    {
        _Transform = transform.parent.parent;
    }

    private void Start()
    {
        if (GestorBase.Instancia.PuertaAbiertaHabitacion)
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
        foreach (var dardo in _Dardos)
        {
            dardo.SetActive(false);
        }
        GestorBase.Instancia.PuertaAbiertaHabitacion = true;
    }

    private void CerrarPuerta()
    {
        _Transform.eulerAngles = _RotacionPuertaCerrada;
        _ObjetoOn = false;
        foreach (var dardo in _Dardos)
        {
            dardo.SetActive(true);
        }
        GestorBase.Instancia.PuertaAbiertaHabitacion = false;
    }
}
