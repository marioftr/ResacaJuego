using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorJuego : MonoBehaviour
{
    public static GameObject PanelPrincipal;
    public static GameObject PanelOpciones;
    public static GameObject PanelMinijuegos;
    public static GameObject PanelTiroAlBlanco;
    public static GameObject PanelRecords;
    public static Button BotonDiana;
    public static Button BotonFlauta;
    public static bool VerCinematica;

    [Header("Gestión Paneles")]
    [SerializeField] private GameObject _PanelPrincipal;
    [SerializeField] private GameObject _PanelOpciones;
    [SerializeField] private GameObject _PanelMinijuegos;
    [SerializeField] private GameObject _PanelTiroAlBlanco;
    [SerializeField] private GameObject _PanelRecords;

    [Header("Gestión Opciones")]
    [SerializeField] private AudioMixer _AudioMixer;

    [SerializeField] private Toggle _ToggleVerCinematica;
    [SerializeField] private Toggle _TogglePantallaCompleta;
    [SerializeField] private Slider _SliderVolumenMusica;
    [SerializeField] private Slider _SliderVolumenEfectos;
    // "const" para no modificar sin querer el nombre de la variable en otra línea
    private const string _ParametroVolumenMusica = "Musica";
    private const string _ParametroVolumenEfectos = "Efectos";
    private const string _ClavePantallaCompleta = "PantallaCompleta";
    private const string _ClaveVerCinematica = "VerCinematica";
    
    [Header("Gestión Minijuegos")]
    [SerializeField] private Button _BotonDiana;
    [SerializeField] private Button _BotonFlauta;

    private void Awake()
    {
        PanelPrincipal = _PanelPrincipal;
        PanelOpciones = _PanelOpciones;
        PanelMinijuegos = _PanelMinijuegos;
        PanelTiroAlBlanco = _PanelTiroAlBlanco;
        PanelRecords  = _PanelRecords;
        BotonDiana = _BotonDiana;
        BotonFlauta = _BotonFlauta;
    }
    public static void CargarEscena(int id)
    {
        SceneManager.LoadScene(id);
    }
    public static void RecargarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void Salir()
    {
#if UNITY_EDITOR
        GestorBase.Instancia.GuardarPartidaManual();
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        GestorBase.Instancia.GuardarPartidaManual();
        Application.Quit();
#endif
    }

    private void Start()
    {
        MostrarMenuPrincipal();
    }
    public static void MostrarMenuPrincipal()
    {
        OcultarTodos();
        PanelPrincipal.SetActive(true);
        GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuPrincipal");
        GestorInterfaz.Instancia.CambiarToggleRecords();
    }
    public static void MostrarMenuOpciones()
    {
        OcultarTodos();
        PanelOpciones.SetActive(true);
    }
    public static void MostrarMenuMinijuegos()
    {
        OcultarTodos();
        PanelMinijuegos.SetActive(true);
        GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuOpciones");
        BotonDiana.interactable = GestorBase.Instancia.EstaDesbloqueado(0);
        BotonFlauta.interactable = GestorBase.Instancia.EstaDesbloqueado(1);
    }
    public static void MostrarMenuTiroAlBlanco()
    {
        OcultarTodos();
        PanelTiroAlBlanco.SetActive(true);
    }
    public static void MostrarMenuRecords()
    {
        OcultarTodos();
        PanelRecords.SetActive(true);
        GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuOpciones");
    }
    public static void OcultarTodos()
    {
        PanelPrincipal.SetActive(false);
        PanelOpciones.SetActive(false);
        PanelMinijuegos.SetActive(false);
        PanelTiroAlBlanco.SetActive(false);
        PanelRecords.SetActive(false);
    }

    private void OnEnable()
    {
        // Carga valores guardados
        // PlayerPrefs.GetFloat lee el valor guardado en los archivos del jugador (1. el valor guardado, 2. el valor por defecto si no hay otro)
        _SliderVolumenMusica.value = PlayerPrefs.GetFloat(_ParametroVolumenMusica, 0.75f);
        _SliderVolumenEfectos.value = PlayerPrefs.GetFloat(_ParametroVolumenEfectos, 0.75f);
        // isOn = Screen.fullScreen sirve para asignar el valor "true" al toggle si lee que el juego está en pantalla completa, y "false" si está en ventana
        _TogglePantallaCompleta.isOn = PlayerPrefs.GetInt(_ClavePantallaCompleta, 0) == 1;
        _ToggleVerCinematica.isOn = PlayerPrefs.GetInt(_ClaveVerCinematica, 1) == 1;

        // Aplica los valores
        AplicarVolumenMusica();
        AplicarVolumenEfectos();
        AplicarPantallaCompleta();
        AplicarVerCinematica();
    }
    public void AplicarVolumenMusica()
    {
        float valor = _SliderVolumenMusica.value; // Asigna el valor del Slider a una variable local

        // Asigna al AudioMixer el volumen con una fórmula logarítmica para que se sienta más natural
        /* _AudioMixer.SetFloat(_ParametroVolumen, Mathf.Log10(valor) * 20); */
        // Con el volumen a 0 el logaritmo devuelve infinito negativo, así que se asigna un valor por defecto para niveles muy bajos
        /*float db;
        if (valor > 0.0001f)
        {
            db = Mathf.Log10(valor) * 20;
        }
        else db = -80f;*/
        // Versión más corta:
        float db = valor > 0.0001f ? Mathf.Log10(valor) * 20 : -80f;

        _AudioMixer.SetFloat(_ParametroVolumenMusica, db); // Ahora sí, asigna al AudioMixer el volumen
        PlayerPrefs.SetFloat(_ParametroVolumenMusica, valor); // Guarda el valor en los archivos del jugador
        PlayerPrefs.Save();
    }

    public void AplicarVolumenEfectos()
    {
        float valor = _SliderVolumenEfectos.value;
        float db = valor > 0.0001f ? Mathf.Log10(valor) * 20 : -80f;
        _AudioMixer.SetFloat(_ParametroVolumenEfectos, db);
        PlayerPrefs.SetFloat(_ParametroVolumenEfectos, valor);
        PlayerPrefs.Save();
    }
    public void AplicarPantallaCompleta()
    {
        bool pantallaCompleta = _TogglePantallaCompleta.isOn; // Aplica la pantalla completa o ventana según esté seleccionado en el toggle

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
        
        Debug.Log("Pantalla completa: " + pantallaCompleta); // Prueba para ver si funciona la pantalla completa en el editor
    }
    public void AplicarVerCinematica()
    {
        VerCinematica = _ToggleVerCinematica.isOn;
        if (GestorBase.Instancia.EstaDesbloqueado(0))
        {
            _ToggleVerCinematica.gameObject.SetActive(true);
        }
        else
        {
            _ToggleVerCinematica.gameObject.SetActive(false);
        }
        
        PlayerPrefs.SetInt(_ClaveVerCinematica, VerCinematica ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static void ActivarDesactivarObjeto(GameObject objeto, bool opcion)
    {
        if (objeto.TryGetComponent(out Collider colision))
        {
            colision.enabled = opcion;
        }
        if(objeto.TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = opcion;
        }
        if(objeto.TryGetComponent(out Light luz))
        {
            luz.enabled = opcion;
        }
        for (int i = 0; i < objeto.transform.childCount; i++)
        {
            ActivarDesactivarObjeto(objeto.transform.GetChild(i).gameObject, opcion);
        }
    }
    public static void LimitarRaton(bool bloqueado)
    {
        if (bloqueado)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public static void CargarMinijuego(int escena)
    {
        CargarEscena(escena);
    }
    public static void SeleccionarTiradasDardos(int numeroTiradas)
    {
        PlayerPrefs.SetInt("TiradasDardos", numeroTiradas);
        PlayerPrefs.Save();
        CargarMinijuego(3);
    }
    public static void JugarDardos()
    {
        SeleccionarTiradasDardos(3);
        CargarMinijuego(3);
    }
    public static void IniciarPartida()
    {
        if (!GestorBase.Instancia.EstaDesbloqueado(0))
        {
            CargarEscena(1);
        }
        else if (VerCinematica)
        {
            CargarEscena(1);
        }
        else
        {
            CargarEscena(2);
        }
    }
}