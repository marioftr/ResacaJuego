using System;
using System.Collections;
using UnityEngine;

public class LuzSalon : ObjetoInteractuable
{
    [SerializeField] private Light[] _Bombillas =  new Light[4];
    [SerializeField] private GameObject _Lampara;
    private Material[] _Materiales;
    private Transform _Transform;
    private string[] _Frase ={""};

    private void Awake()
    {
        _Transform = transform;
        _Materiales = _Lampara.GetComponent<Renderer>().materials;
    }
    private void Start()
    {
        if (GestorBase.Instancia.LuzSalonOn)
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
        
        _Materiales[0].EnableKeyword("_EMISSION");
        _Materiales[6].EnableKeyword("_EMISSION");
        _Transform.position = new Vector3(-0.836f, 1f, 0.215f);
        _Transform.eulerAngles = new Vector3(-10f, 0f, 180f);
        _ObjetoOn = true;
        GestorBase.Instancia.LuzSalonOn = true;
    }
    private void ApagarLuz()
    {
        foreach (var light in _Bombillas)
        {
            light.enabled = false;
        }
        _Materiales[0].DisableKeyword("_EMISSION");
        _Materiales[6].DisableKeyword("_EMISSION");
        _Transform.position = new Vector3(-0.85f, 1.08f, 0.215f);
        _Transform.eulerAngles = new Vector3(10f,0f,0f);
        _ObjetoOn = false;
        GestorBase.Instancia.LuzSalonOn = false;
    }
}
