using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


[System.Serializable] public class ConfiguracionAudioClip
{
    public string NombreClave;
    public AudioClip ClipDeAudio;
}
public class GestorSonido : MonoBehaviour
{
    public static GestorSonido Instancia { get; private set; } // Singleton protegido de cambios en otros scripts con set privado
    
    [Header("Configuración Audio")]
    public AudioSource MusicaDeFondo;
    [SerializeField] private AudioMixerGroup _SalidaMusica;
    [SerializeField] private float _VolumenMaximo;
    [SerializeField] private float _DuracionFade;

    [SerializeField] private List<ConfiguracionAudioClip> _ClipsMusica = new List<ConfiguracionAudioClip>();
    
    private Dictionary<string, AudioClip> _DiccionarioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        
        DontDestroyOnLoad(gameObject);
        IniciarDiccionario();
    }

    private void Start()
    {
        MusicaDeFondo = GetComponent<AudioSource>();
        MusicaDeFondo.playOnAwake = false;
        MusicaDeFondo.volume = _VolumenMaximo;
        MusicaDeFondo.loop = true;

        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void IniciarDiccionario()
    {
        _DiccionarioClips.Clear();
        foreach (var configuracion in _ClipsMusica)
        {
            if (configuracion.ClipDeAudio != null && !string.IsNullOrEmpty(configuracion.NombreClave))
            {
                _DiccionarioClips[configuracion.NombreClave] = configuracion.ClipDeAudio;
            }
        }
    }
    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        string claveEscena = "Escena_" + scene.name;
        ReproducirMusicaDeFondo(claveEscena);
    }
    
    // REPRODUCCIÓN DE CLIPS
    
    public void ReproducirMusicaDeFondo(string clave)
    {
        if(_DiccionarioClips.TryGetValue(clave, out AudioClip nuevoClip))
        {
            StartCoroutine(CambiarMusicaDeFondo(nuevoClip));
        }
        else
        {
            print($"Clip no encontrado: {clave}");
        }
    }
    public void FinalizarMusicaDeFondo()
    {
        MusicaDeFondo.Stop();
    }
    public void PausarMusicaDeFondo()
    {
        MusicaDeFondo.Pause();
    }
    public void ReanudarMusicaDeFondo()
    {
        MusicaDeFondo.UnPause();
    }
    public void CambiarVolumenMusicaDeFondo(float volumen)
    {
        _VolumenMaximo = Mathf.Clamp01(volumen);
        MusicaDeFondo.volume = _VolumenMaximo;
    }

    // TRANSICIÓN ENTRE CLIPS
    
    private IEnumerator CambiarMusicaDeFondo(AudioClip nuevoClip)
    {
        yield return StartCoroutine(FadeOut());
        
        MusicaDeFondo.clip = nuevoClip;
        MusicaDeFondo.Play();

        yield return StartCoroutine(FadeIn());
    }
    public IEnumerator FadeOut()
    {
        float tiempoInicio = Time.time;
        float volumenInicio = MusicaDeFondo.volume;

        while (Time.time < tiempoInicio + _DuracionFade)
        {
            MusicaDeFondo.volume = Mathf.Lerp(volumenInicio, 0f, (Time.time - tiempoInicio) / _DuracionFade);
            yield return null;
        }

        MusicaDeFondo.volume = 0f;
        MusicaDeFondo.Stop();
    }
    private IEnumerator FadeIn()
    {
        MusicaDeFondo.volume = 0f;
        MusicaDeFondo.Play();
        float tiempoInicio = Time.time;

        while (Time.time < tiempoInicio + _DuracionFade)
        {
            MusicaDeFondo.volume = Mathf.Lerp(0f, _VolumenMaximo, (Time.time - tiempoInicio) /  _DuracionFade);
            yield return null;
        }
        
        MusicaDeFondo.volume = _VolumenMaximo;
    }
    
}
