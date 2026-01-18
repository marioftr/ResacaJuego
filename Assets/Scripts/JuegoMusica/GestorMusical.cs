using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GestorMusical : MonoBehaviour
{
    public enum EstadoJuegoMusical
    {
        Inicio = 0,
        Tutorial = 1,
        Melodia = 2,
        Jugador = 3,
        Error = 4,
        Final = 5
    }

    public EstadoJuegoMusical EstadoActual = 0;
    public EstadoJuegoMusical EstadoAnterior = 0;

    [Header("Paneles")]
    [SerializeField] private GameObject _PanelInicio;
    [SerializeField] private GameObject _PanelTutorial;
    [SerializeField] private GameObject _PanelJuego;
    [SerializeField] private GameObject _PanelFinal;
    
    [Header("Listas")]
    public List<NotaMusical> NotasMusicales = new List<NotaMusical>(); // Notas disponibles
    public static List<List<(NotaMusical nota, float duracion)>> SeccionesMelodia = new List<List<(NotaMusical nota, float duracion)>>(); // Lista de melodías y secciones
    private List<(NotaMusical nota, float duracion)> _SecuenciaJugador = new List<(NotaMusical nota, float duracion)>(); // Secuencia del jugador

    [Header("Progreso")]
    private int _IndiceMelodiaActual = 0;
    private int _IndiceSeccionActual = 0;
    private int _IndiceNotaEsperada = 0;
    
    [Header("Jugador")]
    private float _ToleranciaDuracion = 0.1f;
    private Coroutine _CorrutinaActualEsperaMaxima;
    private bool _InputRecibidoEsteFrame = false;

    [Header("Sonido")]
    private AudioSource _AudioSourceGlobal;
    [SerializeField] private AudioClip _SonidoError;
    
    [Header("Resultados")]
    private int[] _ErroresFinales;
    [SerializeField] private TMP_Text _TextoRecordErrores;
    [SerializeField] private TMP_Text _TextoErrores;
    [SerializeField] private TMP_Text _TextoTotalErrores;
    [SerializeField] private TMP_Text _TextoTotalRecord;
    [SerializeField] private Button _BotonGuardarRecord;

    private void Awake()
    {
        _AudioSourceGlobal = GetComponent<AudioSource>();
        _AudioSourceGlobal.playOnAwake = false;
        _AudioSourceGlobal.clip = _SonidoError;
    }
    private void Start()
    {
        GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuFlauta");
        GestorJuego.LimitarRaton(false);
        _IndiceMelodiaActual = 0;
        GenerarMelodias();
        EstadoActual = EstadoJuegoMusical.Inicio;
        CambiarPaneles();
    }

    // FASES JUEGO
    public IEnumerator SiguienteEstado()
    {
        if (EstadoActual < EstadoJuegoMusical.Final)
        {
            yield return null;
            EstadoActual = (EstadoJuegoMusical)((int)EstadoActual + 1);
            CambiarEstado(EstadoActual);
        }
    }
    private void CambiarEstado(EstadoJuegoMusical estado)
    {
        EstadoActual = estado;
        if (EstadoActual == EstadoAnterior) return;

        switch (EstadoActual)
        {
            case EstadoJuegoMusical.Inicio:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuFlauta");
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoMusical.Tutorial:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("PanelTutorial");
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoMusical.Melodia:
                GestorSonido.Instancia.FinalizarMusicaDeFondo();
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoMusical.Jugador:
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoMusical.Error:
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoMusical.Final:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("PanelResultados");
                AlEntrar(EstadoActual);
                FinalJuegoMusical();
                break;
        }
    }
    private void AlEntrar(EstadoJuegoMusical estado)
    {
        print($"Anterior: {EstadoAnterior}\nActual: {estado}");
        CambiarPaneles();
    }
    private void FinalJuegoMusical()
    {
        StringBuilder textoErrores = new StringBuilder();
        for (int i = 0; i < SeccionesMelodia.Count; i++)
        {
            string numeroErrores = _ErroresFinales[i].ToString();
            textoErrores.AppendLine(numeroErrores);
        }
        _TextoErrores.text = textoErrores.ToString();
        _TextoTotalErrores.text = _ErroresFinales.Sum().ToString();

        int totalRecordErrores = 0;
        StringBuilder textoRecordErrores = new StringBuilder();
        for (int i = 0; i < SeccionesMelodia.Count; i++)
        {
            string clave = $"Errores_Melodia{_IndiceMelodiaActual.ToString()}_Seccion{i+1}";
            string numeroErrores = PlayerPrefs.GetInt(clave).ToString();
            totalRecordErrores += PlayerPrefs.GetInt(clave);
            textoRecordErrores.AppendLine(numeroErrores);
        }
        _TextoRecordErrores.text = textoRecordErrores.ToString();
        _TextoTotalRecord.text = totalRecordErrores.ToString();
        
        _BotonGuardarRecord.interactable = _ErroresFinales.Sum() < totalRecordErrores;
    }
    
    // MELODÍAS
    
    private void IniciarMelodia(int indiceMelodiaActual)
    {
        _IndiceSeccionActual = 0;
        _IndiceNotaEsperada = 0;
        
        _ErroresFinales = new int[SeccionesMelodia[_IndiceMelodiaActual].Count];
        for (int i = 0; i < _ErroresFinales.Length; i++)
        {
            _ErroresFinales[i] = 0;
        }

        foreach (var nota in NotasMusicales)
        {
            nota.EventoNotaJugadorCompletada += ComprobarNotaJugador;
        }
        
        ReproducirSeccionActual();
    }
    private void ReproducirSeccionActual()
    {
        StartCoroutine(ReproducirSeccion(_IndiceSeccionActual));
    }
    
    private void GenerarMelodias()
    {
        // Notas musicales utilizadas
        NotaMusical sol3 = NotasMusicales[0];
        NotaMusical la3 = NotasMusicales[1];
        NotaMusical si3 = NotasMusicales[2];
        NotaMusical do4 = NotasMusicales[3];
        NotaMusical re4 = NotasMusicales[4];
        NotaMusical mi4 = NotasMusicales[5];
        NotaMusical fa4 = NotasMusicales[6];
        NotaMusical sol4 = NotasMusicales[7];

        float corchea = 0.5f;
        float negra = 1f;
        float blanca = 2f;

        SeccionesMelodia.Clear();

        // Primera melodía - Cumpleaños feliz

        List<(NotaMusical, float)> seccion1 = new List<(NotaMusical, float)>
        {
            (sol3, corchea),
            (sol3, corchea),
            (la3, negra),
            (sol3, negra),
            (do4, negra),
            (si3, blanca)
        };
        List<(NotaMusical, float)> seccion2 = new List<(NotaMusical, float)>
        {
            (sol3, corchea),
            (sol3, corchea),
            (la3, negra),
            (sol3, negra),
            (re4, negra),
            (do4, blanca)
        };
        List<(NotaMusical, float)> seccion3 = new List<(NotaMusical, float)>
        {
            (do4, corchea),
            (mi4, corchea),
            (sol4, negra),
            (mi4, negra),
            (do4, negra),
            (si3, negra),
            (la3, negra)
        };
        List<(NotaMusical, float)> seccion4 = new List<(NotaMusical, float)>
        {
            (fa4, corchea),
            (fa4, corchea),
            (mi4, negra),
            (do4, negra),
            (re4, negra),
            (do4, blanca)
        };

        SeccionesMelodia.Add(seccion1);
        SeccionesMelodia.Add(seccion2);
        SeccionesMelodia.Add(seccion3);
        SeccionesMelodia.Add(seccion4);

        // Segunda melodía
    }

    // PROCESOS JUGADOR

    private void ComprobarNotaJugador(NotaMusical notaPulsada, float duracionPulsada)
    {
        _InputRecibidoEsteFrame = true;
        
        var seccionActual = SeccionesMelodia[_IndiceSeccionActual];
        var notaEsperada = seccionActual[_IndiceNotaEsperada].nota;
        var duracionEsperada = seccionActual[_IndiceNotaEsperada].duracion;
        
        // Comprobar espera correcta
        if (_CorrutinaActualEsperaMaxima != null)
        {
            StopCoroutine(_CorrutinaActualEsperaMaxima);
            _CorrutinaActualEsperaMaxima = null;
        }
        // Comprobar nota correcta
        if (notaPulsada != notaEsperada)
        {
            NotaEquivocada(notaPulsada);
            return;
        }
        // Comprobar duración correcta
        if (duracionPulsada < duracionEsperada - _ToleranciaDuracion)
        {
            NotaEquivocada(notaPulsada);
            return;
        }

        _IndiceNotaEsperada++; // Acierto

        if (_IndiceNotaEsperada < seccionActual.Count)
        {
            _CorrutinaActualEsperaMaxima = StartCoroutine(CorrutinaEsperaMaxima());
        }
        
        // Comprobar sección completada
        if (_IndiceNotaEsperada >= seccionActual.Count)
        {
            _IndiceSeccionActual++;

            if (_IndiceSeccionActual >= SeccionesMelodia.Count)
            {
                print("Melodía completada");
                CambiarEstado(EstadoJuegoMusical.Final);
            }
            else
            {
                ReproducirSeccionActual();
            }
        }
    }

    private void NotaEquivocada(NotaMusical notaPulsada)
    {
        EstadoActual = EstadoJuegoMusical.Error;
        _ErroresFinales[_IndiceSeccionActual]++;
        PararTodasLasNotas();
        _AudioSourceGlobal.time = _SonidoError.length * 0.3f;
        _AudioSourceGlobal.Play();
        notaPulsada.ParpadearEnRojo();
        StartCoroutine(CorrutinaNotaEquivocada());
    }
    private IEnumerator CorrutinaNotaEquivocada()
    {
        yield return new WaitForSeconds(1.5f);
        RepetirSeccionActual();
    }

    private IEnumerator CorrutinaEsperaMaxima()
    {
        float temporizador = 0f;
        while (temporizador < 4f)
        {
            temporizador += Time.deltaTime;
            if (_InputRecibidoEsteFrame)
            {
                yield break;
            }
            yield return null;
        }
        if (_IndiceNotaEsperada < SeccionesMelodia[_IndiceSeccionActual].Count)
        {
            NotaMusical notaEsperada = SeccionesMelodia[_IndiceSeccionActual][_IndiceNotaEsperada].nota;
            NotaEquivocada(notaEsperada);
        }
        _CorrutinaActualEsperaMaxima = null;
    }
    
    // REPRODUCCIÓN NOTAS
    
    private IEnumerator ReproducirSeccion(int indiceSeccion)
    {
        PararTodasLasNotas();
        CambiarEstado(EstadoJuegoMusical.Melodia);

        yield return new WaitForSeconds(1f);

        if (indiceSeccion < PartituraCumpleañosFeliz.Instancia.PosicionesPartitura.Length)
        {
            StartCoroutine(PartituraCumpleañosFeliz.Instancia.MoverPartitura(indiceSeccion));
        }
        
        Flauta.Instancia.IniciarBucle();

        foreach (var elemento in SeccionesMelodia[indiceSeccion])
        {
            NotaMusical nota = elemento.nota;
            float duracion = elemento.duracion;

            nota.IniciarRelleno(duracion);
            nota.ReproducirDurante(duracion);

            yield return new WaitForSeconds(duracion+0.1f);

            nota.DetenerRelleno();
        }
        
        _IndiceNotaEsperada = 0;
        _SecuenciaJugador.Clear();
        CambiarEstado(EstadoJuegoMusical.Jugador);
        
        Flauta.Instancia.DetenerMovimiento();

        if (_CorrutinaActualEsperaMaxima != null)
        {
            StopCoroutine(_CorrutinaActualEsperaMaxima);
        }

        _InputRecibidoEsteFrame = false;
        _CorrutinaActualEsperaMaxima = StartCoroutine(CorrutinaEsperaMaxima());
    }

    private void RepetirSeccionActual()
    {
        _IndiceNotaEsperada = 0;
        _SecuenciaJugador.Clear();
        StartCoroutine(ReproducirSeccion(_IndiceSeccionActual));
    }
    public void PararTodasLasNotas()
    {
        foreach (var nota in NotasMusicales)
        {
            if (nota.CorrutinaNotaActual != null)
            {
                nota.StopCoroutine(nota.CorrutinaNotaActual);
                nota.AudioSourceNota.Stop();
                nota.CorrutinaNotaActual = null;
            }
            nota.AudioSourceNota.volume = 1f;
        }
    }
    public float ObtenerDuracionNotaEsperada()
    {
        if (_IndiceNotaEsperada < SeccionesMelodia[_IndiceSeccionActual].Count)
        {
            return SeccionesMelodia[_IndiceSeccionActual][_IndiceNotaEsperada].duracion;
        }

        return 5f;
    }
    
    // BOTONES E INTERFAZ

    private void CambiarPaneles()
    {
        _PanelInicio.SetActive(EstadoActual == EstadoJuegoMusical.Inicio);
        _PanelTutorial.SetActive(EstadoActual == EstadoJuegoMusical.Tutorial);
        _PanelJuego.SetActive(EstadoActual == EstadoJuegoMusical.Melodia || EstadoActual == EstadoJuegoMusical.Jugador);
        _PanelFinal.SetActive(EstadoActual == EstadoJuegoMusical.Final);
    }
    public void BotonJugar()
    {
        CambiarEstado(EstadoJuegoMusical.Melodia);
        IniciarMelodia(_IndiceMelodiaActual);
    }
    public void BotonTutorial()
    {
        CambiarEstado(EstadoJuegoMusical.Tutorial);
    }
    public void BotonJugarOtraVez()
    {
        GestorJuego.RecargarEscena();
    }
    public void BotonSalir()
    {
        GestorBase.Instancia.DesbloquearMinijuego(1);
        GestorBase.Instancia.VolverAlOrigen();
    }
    public void GuardarRecord()
    {
        for (int i = 0; i < SeccionesMelodia.Count; i++)
        {
            string clave = $"Errores_Melodia{_IndiceMelodiaActual.ToString()}_Seccion{i+1}";
            PlayerPrefs.SetInt(clave, _ErroresFinales[i]);
        }
        PlayerPrefs.Save();
        
        StringBuilder textoRecordErrores = new StringBuilder();
        for (int i = 0; i < SeccionesMelodia.Count; i++)
        {
            string clave = $"Errores_Melodia{_IndiceMelodiaActual.ToString()}_Seccion{i+1}";
            string numeroErrores = PlayerPrefs.GetInt(clave).ToString();
            textoRecordErrores.AppendLine(numeroErrores);
        }
        _TextoRecordErrores.text = textoRecordErrores.ToString();    
    }
}
