using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartituraCumpleañosFeliz : MonoBehaviour
{
    public static PartituraCumpleañosFeliz Instancia { get; private set; }

    private Transform _Transform;

    public Vector3[] PosicionesPartitura;

    private void Awake()
    {
        _Transform = transform;
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        PosicionesPartitura = new Vector3[5]
        {
            new Vector3(14f, -1.25f, -3.55f),
            new Vector3(9.5f, -1.25f, -3.55f),
            new Vector3(1.5f, -1.25f, -3.55f),
            new Vector3(-5, -1.25f, -3.55f),
            new Vector3(-11f, -1.25f, -3.55f)
        };
    }

    private void Start()
    {
        _Transform.position = PosicionesPartitura[0];
    }

    public IEnumerator MoverPartitura(int indiceSeccion)
    {
        Vector3 posicionInicial = PosicionesPartitura[indiceSeccion];
        Vector3 posicionFinal = PosicionesPartitura[indiceSeccion+1];

        float temporizador = 0f;
        float duracionTotalSeccion = 0f;

        foreach (var elemento in GestorMusical.SeccionesMelodia[indiceSeccion])
        {
            duracionTotalSeccion += elemento.duracion + 0.1f;
        }

        while (temporizador < duracionTotalSeccion)
        {
            temporizador += Time.deltaTime;
            _Transform.position = Vector3.Lerp(posicionInicial, posicionFinal, temporizador/duracionTotalSeccion);
            yield return null;
        }
        _Transform.position = posicionFinal;
    }
}
