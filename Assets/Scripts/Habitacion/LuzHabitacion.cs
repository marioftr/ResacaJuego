using System;
using System.Collections;
using UnityEngine;

public class LuzHabitacion : ObjetoInteractuable
{
    [SerializeField] private Light[] _Bombillas =  new Light[3];
    [SerializeField] private GameObject _Lampara;
    private Transform _Transform;
    private string[] _Frase ={""};

    private void Awake()
    {
        _Transform = transform;
    }
    private void Start()
    {
        if (GestorBase.Instancia.LuzHabitacionOn)
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
        foreach (var light in _Bombillas)
        {
            light.enabled = true;
        }
        _Lampara.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        _Transform.position = new Vector3(-3.815f, 1f, 0.4f);
        _Transform.eulerAngles = new Vector3(-10f, -90f, 180f);
        _ObjetoOn = true;
        GestorBase.Instancia.LuzHabitacionOn = true;
    }
    private void ApagarLuz()
    {
        foreach (var light in _Bombillas)
        {
            light.enabled = false;
        }
        _Lampara.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        _Transform.position = new Vector3(-3.815f, 1.08f, 0.3865f);
        _Transform.eulerAngles = new Vector3(10f, -90f, 0);
        _ObjetoOn = false;
        GestorBase.Instancia.LuzHabitacionOn = false;
    }
}
