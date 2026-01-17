using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DatosGuardado
{
    // PERSONAJE
    public List<bool> DG_MinijuegosDesbloqueados;
    public bool DG_EnModoHistoria;
    public Vector3 DG_PosicionJugadorEnHabitacion;
    public Quaternion DG_RotacionJugadorEnHabitacion;
    
    // PUNTUACIONES
    public int DG_PuntuacionDardos3;
    public int DG_PuntuacionDardos5;
    public int DG_PuntuacionFlautaTotalCF;
    public int[] DG_PuntuacionesFlautaSeccionesCF = new int[4];
    
    // PUERTAS
    public bool DG_PuertaAbiertaHabitacion;
    public bool DG_PuertaAbiertaBaño;
    public bool DG_ArmarioFuncionalSalon;
    public bool DG_CabineteFuncionalSalon;
    
    // LUCES
    public bool DG_LuzMesitaHabitacionOn;
    public bool DG_LuzHabitacionOn;
    public bool DG_LuzBañoOn;
    public bool DG_LuzEscritorioOn;
    public bool DG_LuzSalonOn;
}

public class GestorBase : MonoBehaviour
{
    public static GestorBase Instancia { get; private set; }
    
    [Header("Configuración")]
    [SerializeField] private int _TotalMinijuegos = 2;

    // PERSONAJE
    private List<bool> _MinijuegosDesbloqueados;
    public bool EnModoHistoria = false;
    public Vector3 PosicionJugadorEnHabitacion;
    public Quaternion RotacionJugadorEnHabitacion;
    
    // PUNTUACIONES
    public int PuntuacionDardos3;
    public int PuntuacionDardos5;
    public int PuntuacionFlautaTotalCF;
    public int[] PuntuacionFlautaSeccionesCF;
    
    // PUERTAS
    public bool PuertaAbiertaHabitacion = false;
    public bool PuertaAbiertaBaño = false;
    public bool ArmarioFuncionalSalon = false;
    public bool CabineteFuncionalSalon = false;
    
    // LUCES
    public bool LuzMesitaHabitacionOn = false;
    public bool LuzHabitacionOn = false;
    public bool LuzBañoOn = false;
    public bool LuzEscritorioOn = false;
    public bool LuzSalonOn = false;

    public string RutaDeGuardado;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
        
        RutaDeGuardado = Path.Combine(Application.persistentDataPath, "ArchivoDeGuardado");
        print("Ruta de guardado: " + RutaDeGuardado);
        
        string carpeta = Path.GetDirectoryName(RutaDeGuardado);
        if (!Directory.Exists(carpeta))
        {
            Directory.CreateDirectory(carpeta);
            print("Carpeta creada: " + carpeta);
        }
        
        _MinijuegosDesbloqueados = new List<bool>(_TotalMinijuegos);
        for (int i = 0; i < _TotalMinijuegos; i++)
        {
            _MinijuegosDesbloqueados.Add(false);
        }
        
        CargarJuego();
    }
    private void Update()
    {
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            print("Desbloqueando...");
            DesbloquearMinijuego(0);
            DesbloquearMinijuego(1);
        }
    }

    // DESBLOQUEO DE MINIJUEGOS
    
    public void DesbloquearMinijuego(int indice)
    {
        if (indice < 0 || indice >= _TotalMinijuegos) return;
        _MinijuegosDesbloqueados[indice] = true;
        GuardarJuego();
    }
    public bool EstaDesbloqueado(int indice)
    {
        if (indice < 0 || indice >= _TotalMinijuegos) return false;
        return _MinijuegosDesbloqueados[indice];
    }
    
    // ORIGEN MINIJUEGOS

    public void EstablecerOrigenModoHistoria()
    {
        EnModoHistoria = true;
        GuardarPosicionPersonaje();
        GuardarJuego();
    }
    public void EstablecerOrigenMenuPrincipal()
    {
        EnModoHistoria = false;
        GuardarJuego();
    }
    public void VolverAlOrigen()
    {
        if (EnModoHistoria)
        {
            GestorJuego.CargarEscena(2); // Habitación
        }
        else
        {
            GestorJuego.CargarEscena(0); // Menú principal
            EnModoHistoria = false;
        }
    }
    
    // GUARDAR Y CARGAR JUEGO

    private void GuardarJuego()
    {
        DatosGuardado datosGuardado = new DatosGuardado();
        
        // PERSONAJE
        datosGuardado.DG_MinijuegosDesbloqueados = _MinijuegosDesbloqueados;
        datosGuardado.DG_EnModoHistoria = EnModoHistoria;
        datosGuardado.DG_PosicionJugadorEnHabitacion = PosicionJugadorEnHabitacion;
        datosGuardado.DG_RotacionJugadorEnHabitacion = RotacionJugadorEnHabitacion;
        
        // PUNTUACIONES
        datosGuardado.DG_PuntuacionDardos3 = PlayerPrefs.GetInt("Record_3");
        datosGuardado.DG_PuntuacionDardos5 = PlayerPrefs.GetInt("Record_5");
        
        PuntuacionFlautaTotalCF =
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion0") +
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion1") +
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion2") +
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion3");
        datosGuardado.DG_PuntuacionFlautaTotalCF = PuntuacionFlautaTotalCF;
        
        PuntuacionFlautaSeccionesCF = new int[]
        {
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion0"),
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion1"),
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion2"),
            PlayerPrefs.GetInt("Errores_Melodia0_Seccion3")
        };
        datosGuardado.DG_PuntuacionesFlautaSeccionesCF = PuntuacionFlautaSeccionesCF;
        
        // PUERTAS
        datosGuardado.DG_PuertaAbiertaHabitacion = PuertaAbiertaHabitacion;
        datosGuardado.DG_PuertaAbiertaBaño = PuertaAbiertaBaño;
        datosGuardado.DG_ArmarioFuncionalSalon = ArmarioFuncionalSalon;
        datosGuardado.DG_CabineteFuncionalSalon = CabineteFuncionalSalon;
        
        // LUCES
        datosGuardado.DG_LuzMesitaHabitacionOn = LuzMesitaHabitacionOn;
        datosGuardado.DG_LuzHabitacionOn = LuzHabitacionOn;
        datosGuardado.DG_LuzBañoOn = LuzBañoOn;
        datosGuardado.DG_LuzEscritorioOn = LuzEscritorioOn;
        datosGuardado.DG_LuzSalonOn = LuzSalonOn;

        try
        {
            File.WriteAllText(RutaDeGuardado, JsonUtility.ToJson(datosGuardado, true));
            print("Guardado correctamente en: " + RutaDeGuardado);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al guardar JSON: " + e.Message);
        }
    }
    private void CargarJuego()
    {
        if (!File.Exists(RutaDeGuardado))
        {
            print("No hay JSON previo. Iniciando nuevo juego.");
            IniciarValoresPorDefecto();
            return;
        }

        try
        {
            DatosGuardado datosGuardado = JsonUtility.FromJson<DatosGuardado>(File.ReadAllText(RutaDeGuardado));

            if (datosGuardado == null)
            {
                print("JSON inválido. Usando valores por defecto.");
                IniciarValoresPorDefecto();
                return;
            }
            
            // PERSONAJE
            _MinijuegosDesbloqueados = datosGuardado.DG_MinijuegosDesbloqueados;
            EnModoHistoria = datosGuardado.DG_EnModoHistoria;
            PosicionJugadorEnHabitacion = datosGuardado.DG_PosicionJugadorEnHabitacion;
            RotacionJugadorEnHabitacion = datosGuardado.DG_RotacionJugadorEnHabitacion;
            
            // PUNTUACIONES
            PuntuacionDardos3 = datosGuardado.DG_PuntuacionDardos3;
            PuntuacionDardos5 = datosGuardado.DG_PuntuacionDardos5;
            PuntuacionFlautaTotalCF = datosGuardado.DG_PuntuacionFlautaTotalCF;
            PuntuacionFlautaSeccionesCF = datosGuardado.DG_PuntuacionesFlautaSeccionesCF;
            
            // PUERTAS
            PuertaAbiertaHabitacion = datosGuardado.DG_PuertaAbiertaHabitacion;
            PuertaAbiertaBaño = datosGuardado.DG_PuertaAbiertaBaño;
            ArmarioFuncionalSalon = datosGuardado.DG_ArmarioFuncionalSalon;
            CabineteFuncionalSalon = datosGuardado.DG_CabineteFuncionalSalon;
            
            // LUCES
            LuzMesitaHabitacionOn = datosGuardado.DG_LuzMesitaHabitacionOn;
            LuzHabitacionOn = datosGuardado.DG_LuzHabitacionOn;
            LuzBañoOn = datosGuardado.DG_LuzBañoOn;
            LuzEscritorioOn = datosGuardado.DG_LuzEscritorioOn;
            LuzSalonOn = datosGuardado.DG_LuzSalonOn;

            print("JSON cargado correctamente.");
        }
        catch(Exception e)
        {
            Debug.LogError("Error al cargar JSON: " + e.Message);
            IniciarValoresPorDefecto();
        }
    }
    public void GuardarPartidaManual()
    {
        GuardarJuego();
    }
    public void CargarPartidaManual()
    {
        CargarJuego();
    }

    public void GuardarPosicionPersonaje()
    {
        GameObject personaje = GameObject.FindGameObjectWithTag("Player");
        if (personaje != null)
        {
            PosicionJugadorEnHabitacion = personaje.transform.position;
            RotacionJugadorEnHabitacion = personaje.transform.rotation;
            EnModoHistoria = true;
            print("Posición actual del personaje guardada");
        }
        else
        {
            Debug.LogWarning("No se ha encontrado al personaje con tag 'Player'.");
        }
    }

    private void IniciarValoresPorDefecto()
    {
        _MinijuegosDesbloqueados = new List<bool>();
        for (int i = 0; i < _TotalMinijuegos; i++)
        {
            _MinijuegosDesbloqueados.Add(false);
        }
        EnModoHistoria = false;
        PosicionJugadorEnHabitacion = new Vector3(-6f,0.8f,0.3f);
        RotacionJugadorEnHabitacion = Quaternion.Euler(0, 45, 0);
        PuntuacionDardos3 = 0;
        PuntuacionDardos5 = 0;
        PuntuacionFlautaTotalCF = 0;
        PuntuacionFlautaSeccionesCF = new int[4] { 0, 0, 0, 0 };
    }
}
