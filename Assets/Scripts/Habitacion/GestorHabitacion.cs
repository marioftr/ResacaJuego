using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GestorHabitacion : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject _PanelOpciones;
    [SerializeField] private GameObject _Pointer;
    [SerializeField] private GameObject _PuertaHabCerrada;
    [SerializeField] private GameObject _PuertaHabAbierta;
    [SerializeField] private GameObject _PuertaBañoCerrada;
    [SerializeField] private GameObject _PuertaBañoAbierta;
    [SerializeField] private TMP_Text _TextoFrase;
    
    private InputSystem_Actions _Controles;
    private SistemasPersonaje _Personaje;
    
    [Header("Gestión Opciones")]
    [SerializeField] private AudioMixer _AudioMixer;
    [SerializeField] private Toggle _TogglePantallaCompleta;
    [SerializeField] private Slider _SliderVolumenMusica;
    [SerializeField] private Slider _SliderVolumenEfectos;
    private const string _ParametroVolumenMusica = "Musica";
    private const string _ParametroVolumenEfectos = "Efectos";
    private const string _ClavePantallaCompleta = "PantallaCompleta";

    private void Awake()
    {
        _Controles = new InputSystem_Actions();
        _Personaje = FindAnyObjectByType<SistemasPersonaje>();
    }
    private void OnEnable()
    {
        _Controles.Enable();
        
        _SliderVolumenMusica.value = PlayerPrefs.GetFloat(_ParametroVolumenMusica, 0.75f);
        _SliderVolumenEfectos.value = PlayerPrefs.GetFloat(_ParametroVolumenEfectos, 0.75f);
        _TogglePantallaCompleta.isOn = PlayerPrefs.GetInt(_ClavePantallaCompleta, 0) == 1;
        
        AplicarVolumenMusica();
        AplicarVolumenEfectos();
        AplicarPantallaCompleta();
    }
    private void OnDisable()
    {
        _Controles.Disable();
    }

    private void Start()
    {
        GestorSonido.Instancia.ReproducirMusicaDeFondo("Lluvia");
        GestorBase.Instancia.CargarPartidaManual();
        CerrarOpciones();

        GestorBase.Instancia.EnModoHistoria = true;
        BloquearODesbloquearPuertas();
    }

    public void Pausa()
    {
        if(!_PanelOpciones.activeInHierarchy) AbrirOpciones();
        else CerrarOpciones();
    }
    private void AbrirOpciones()
    {
        _PanelOpciones.SetActive(true);
        _Pointer.SetActive(false);
        GestorSonido.Instancia.PausarMusicaDeFondo();
        GestorJuego.LimitarRaton(false);
        _Personaje.Camara.CamaraActiva = false;
        _TextoFrase.enabled = false;
    }
    private void CerrarOpciones()
    {
        _PanelOpciones.SetActive(false);
        _Pointer.SetActive(true);
        GestorSonido.Instancia.ReanudarMusicaDeFondo();
        GestorJuego.LimitarRaton(true);
        _Personaje.Camara.CamaraActiva = true;
        _TextoFrase.enabled = true;
    }

    private void BloquearODesbloquearPuertas()
    {
        if (!GestorBase.Instancia.EstaDesbloqueado(0))
        {
            _PuertaHabCerrada.SetActive(true);
            _PuertaHabAbierta.SetActive(false);
        }
        else
        {
            _PuertaHabCerrada.SetActive(false);
            _PuertaHabAbierta.SetActive(true);
        }

        if (!GestorBase.Instancia.EstaDesbloqueado(1))
        {
            _PuertaBañoCerrada.SetActive(true);
            _PuertaBañoAbierta.SetActive(false);
        }
        else
        {
            _PuertaBañoCerrada.SetActive(false);
            _PuertaBañoAbierta.SetActive(true);
        }
    }

    public void AplicarVolumenMusica()
    {
        float valor = _SliderVolumenMusica.value;
        float db = valor > 0.0001f ? Mathf.Log10(valor) * 20 : -80f;
        _AudioMixer.SetFloat(_ParametroVolumenMusica, db);
        PlayerPrefs.SetFloat(_ParametroVolumenMusica, valor);
    }
    public void AplicarVolumenEfectos()
    {
        float valor = _SliderVolumenEfectos.value;
        float db = valor > 0.0001f ? Mathf.Log10(valor) * 20 : -80f;
        _AudioMixer.SetFloat(_ParametroVolumenEfectos, db);
        PlayerPrefs.SetFloat(_ParametroVolumenEfectos, valor);
    }
    public void AplicarPantallaCompleta()
    {
        bool pantallaCompleta = _TogglePantallaCompleta.isOn;
        if (pantallaCompleta)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        Screen.fullScreen = pantallaCompleta;
        PlayerPrefs.SetInt(_ClavePantallaCompleta, pantallaCompleta ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Pantalla completa: " + pantallaCompleta);
    }
    public void VolverAlMenu()
    {
        GestorBase.Instancia.GuardarPosicionPersonaje();
        GestorBase.Instancia.EstablecerOrigenMenuPrincipal(); // Ya guarda partida
        GestorJuego.CargarEscena(0);
    }
}