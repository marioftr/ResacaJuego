using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GestorEfectosSonido : MonoBehaviour
{
    [SerializeField] private AudioSource[] _EfectosSonido;

    private Dictionary<string, AudioClip> _DiccionarioEfectos;
    
    private List<AudioClip> _AudioClips;
    [SerializeField] private AudioClip _EfectoLanzarDardo;
    [SerializeField] private AudioClip _EfectoClavarDardo;
    [SerializeField] private AudioClip _EfectoBoton;
    [SerializeField] private AudioClip _EfectoClickGrave;
    [SerializeField] private AudioClip _EfectoClickAgudo;
    [SerializeField] private AudioClip _EfectoPuertaAbierta;
    [SerializeField] private AudioClip _EfectoPuertaCerrada;
    [SerializeField] private AudioClip _EfectoPuertaBloqueada;
    [SerializeField] private AudioClip _EfectoArmarioBloqueado;
    [SerializeField] private AudioClip _EfectoAcordeGuitarra;
    [SerializeField] private AudioClip _EfectoLucesOn;
    [SerializeField] private AudioClip _EfectoLucesOff;
    [SerializeField] private AudioClip _EfectoAbrirArmarioCorredera;
    [SerializeField] private AudioClip _EfectoCerrarArmarioCorredera;

    [SerializeField] private AudioMixerGroup _SalidaEfectos;
    
    [SerializeField] [Range(0f, 1f)] private float _VolumenEfectos;
    
    public static GestorEfectosSonido Instancia { get; private set; }
    
    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

        _DiccionarioEfectos = new Dictionary<string, AudioClip>()
        {
            {"EfectoLanzarDardo", _EfectoLanzarDardo},
            {"EfectoClavarDardo", _EfectoClavarDardo},
            {"EfectoBoton", _EfectoBoton},
            {"EfectoClickGrave", _EfectoClickGrave},
            {"EfectoClickAgudo", _EfectoClickAgudo},
            {"EfectoPuertaAbierta", _EfectoPuertaAbierta},
            {"EfectoPuertaCerrada",  _EfectoPuertaCerrada},
            {"EfectoPuertaBloqueada",  _EfectoPuertaBloqueada},
            {"EfectoArmarioBloqueado", _EfectoArmarioBloqueado},
            {"EfectoAcordeGuitarra", _EfectoAcordeGuitarra},
            {"EfectoLucesOn", _EfectoLucesOn},
            {"EfectoLucesOff", _EfectoLucesOff},
            {"EfectoAbrirArmarioCorredera",  _EfectoAbrirArmarioCorredera},
            {"EfectoCerrarArmarioCorredera", _EfectoCerrarArmarioCorredera},
        };
        
        _AudioClips = new List<AudioClip>(_DiccionarioEfectos.Values);
        _EfectosSonido = new AudioSource[_AudioClips.Count];
        
        for (int i = 0; i < _EfectosSonido.Length; i++)
        {
            GameObject AudioSource = new GameObject($"AudioSource_{i}");
            _EfectosSonido[i] = AudioSource.AddComponent<AudioSource>();
            DontDestroyOnLoad(AudioSource);
            _EfectosSonido[i].volume = _VolumenEfectos;
            _EfectosSonido[i].playOnAwake = false;
            _EfectosSonido[i].loop = false;
            _EfectosSonido[i].clip = _AudioClips[i];
            _EfectosSonido[i].outputAudioMixerGroup = _SalidaEfectos;
        }
    }

    public static void ReproducirEfecto(string nombreEfecto)
    {
        if (!Instancia._DiccionarioEfectos.TryGetValue(nombreEfecto, out AudioClip clip))
        {
            print($"Efecto no encontrado: {nombreEfecto}");
        }
        Instancia.ReproducirEfectoPorClip(clip);
    }
    private void ReproducirEfectoPorClip(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("Clip nulo al intentar reproducir");
            return;
        }
        for (int i = 0; i < _EfectosSonido.Length; i++)
        {
            if (_EfectosSonido[i] != null && _EfectosSonido[i].clip == audioClip)
            {
                _EfectosSonido[i].Play();
                return;
            }
        }
        Debug.LogWarning($"No se encontró AudioSource para el clip: {audioClip.name}");
    }
}
