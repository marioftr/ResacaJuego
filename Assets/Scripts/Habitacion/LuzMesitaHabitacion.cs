using System;
using System.Collections;
using UnityEngine;

public class LuzMesitaHabitacion : ObjetoInteractuable
{
    [SerializeField] private Light _Bombilla;
    [SerializeField] private GameObject _Lampara;
    private string[] _Frase ={""};

    private void Start()
    {
        if (GestorBase.Instancia.LuzMesitaHabitacionOn)
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
        GestorBase.Instancia.LuzMesitaHabitacionOn = true;
    }
    private void ApagarLuz()
    {
        _Bombilla.enabled = false;
        _Lampara.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        _ObjetoOn = false;
        GestorBase.Instancia.LuzMesitaHabitacionOn = false;
    }
}
