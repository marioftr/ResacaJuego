using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class NotaMusical : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GestorMusical _GestorMusical;

    [Header("Audio")]
    public AudioSource AudioSourceNota;
    [SerializeField] private AudioClip _Nota;
    [SerializeField, Range(0f, 1f)] private float _InicioNormalizado = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _FinalNormalizado = 0.6f;
    [SerializeField, Range(0.01f, 0.3f)] private float _FadeMaximo = 0.08f;

    [Header("Botones")]
    [SerializeField] private Image _ImagenRelleno;
    private float _TiempoInicioPulsacion;
    private bool _NotaPulsada = false;

    private Coroutine _CorrutinaRelleno;

    public Coroutine CorrutinaNotaActual;

    public delegate void NotaJugadorCompletada(NotaMusical nota, float duracion);
    public event NotaJugadorCompletada EventoNotaJugadorCompletada;

    private void Awake()
    {
        AudioSourceNota = GetComponent<AudioSource>();
        AudioSourceNota.playOnAwake = false;
        AudioSourceNota.clip = _Nota;

        _GestorMusical = FindAnyObjectByType<GestorMusical>();

        _ImagenRelleno = transform.GetChild(0).GetComponent<Image>();
        _ImagenRelleno.fillAmount = 0f;
    }

    public void ReproducirNota() // Al pulsar el botón
    {
        float inicio = _Nota.length * _InicioNormalizado;
        float duracion = _Nota.length * (_FinalNormalizado - _InicioNormalizado);
        
        _GestorMusical.PararTodasLasNotas();

        AudioSourceNota.time = inicio;
        AudioSourceNota.loop = false;

        CorrutinaNotaActual = StartCoroutine(FadeInOut(duracion));
    }

    public void ReproducirDurante(float duracion)
    {
        _GestorMusical.PararTodasLasNotas();
        
        float inicio = _Nota.length * _InicioNormalizado;
        AudioSourceNota.time = inicio;
        AudioSourceNota.loop = false;
        CorrutinaNotaActual = StartCoroutine(FadeInOut(duracion));
    }

    private IEnumerator FadeInOut(float duracion)
    {
        AudioSourceNota.volume = 0f;
        AudioSourceNota.Play();

        // Fade in
        float temporizador = 0f;
        while (temporizador < _FadeMaximo)
        {
            temporizador += Time.deltaTime;
            AudioSourceNota.volume = Mathf.Lerp(0f, 1f, temporizador / _FadeMaximo);
            yield return null;
        }

        AudioSourceNota.volume = 1f;

        float tiempoRestante = duracion - 2 * _FadeMaximo;
        float sostenido = Mathf.Max(0f, tiempoRestante);
        
        // Comprobar si el jugador soltó el botón para cortar el sonido
        temporizador = 0f;
        while (temporizador < sostenido)
        {
            temporizador += Time.deltaTime;
            if (_GestorMusical.EstadoActual == GestorMusical.EstadoJuegoMusical.Jugador && !_NotaPulsada)
            {
                break;
            }
            yield return null;
        }
        
        //Fade out
        temporizador = 0f;
        while (temporizador < _FadeMaximo)
        {
            temporizador += Time.deltaTime;
            AudioSourceNota.volume = Mathf.Lerp(1f, 0f, temporizador / _FadeMaximo);
            yield return null;
        }

        AudioSourceNota.Stop();
        AudioSourceNota.volume = 1f;
        CorrutinaNotaActual = null;
    }

    // RELLENO BOTONES

    public void IniciarRelleno(float duracion)
    {
        if (_CorrutinaRelleno != null) StopCoroutine(_CorrutinaRelleno);
        if (_ImagenRelleno != null) _ImagenRelleno.fillAmount = 0f;
        
        _CorrutinaRelleno = StartCoroutine(RellenarBoton(duracion));
    }

    public void DetenerRelleno()
    {
        if (_CorrutinaRelleno != null)
        {
            StopCoroutine(_CorrutinaRelleno);
            _CorrutinaRelleno = null;
        }

        if (_ImagenRelleno != null) _ImagenRelleno.fillAmount = 0f;
    }

    private IEnumerator RellenarBoton(float duracionRelleno)
    {
        if (_ImagenRelleno == null) yield break;

        float temporizador = 0f;
        while (temporizador < duracionRelleno)
        {
            temporizador += Time.deltaTime;
            _ImagenRelleno.fillAmount = temporizador / duracionRelleno;
            yield return null;
        }

        _ImagenRelleno.fillAmount = 1f;
    }

    // OTROS
    
    public void ParpadearEnRojo()
    {
        StartCoroutine(CorrutinaParpadearEnRojo());
    }

    private IEnumerator CorrutinaParpadearEnRojo()
    {
        Image imagenBoton = GetComponent<Image>();
        if (imagenBoton == null) yield break;
        
        Color colorOriginal = imagenBoton.color;

        for (int i = 0; i < 3; i++)
        {
            imagenBoton.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            imagenBoton.color = colorOriginal;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_GestorMusical.EstadoActual != GestorMusical.EstadoJuegoMusical.Jugador) return;
        if (_NotaPulsada) return;
        
        _NotaPulsada = true;
        _TiempoInicioPulsacion = Time.time;
        
        ReproducirDurante(5f);

        float duracionEsperada = _GestorMusical.ObtenerDuracionNotaEsperada();
        IniciarRelleno(duracionEsperada);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!_NotaPulsada) return;
        _NotaPulsada = false;
        
        float duracionPulsacion = Time.time - _TiempoInicioPulsacion;
        
        DetenerRelleno();
        
        EventoNotaJugadorCompletada?.Invoke(this, duracionPulsacion);
        /*if (EventoNotaJugadorCompletada != null)
        {
            EventoNotaJugadorCompletada(this, duracionPulsacion);
        }*/
    }
}