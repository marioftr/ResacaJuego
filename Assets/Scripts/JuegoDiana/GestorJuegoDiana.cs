using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GestorJuegoDiana : MonoBehaviour
{
    public enum EstadoJuegoDiana
    {
        Inicio = 0,
        Tutorial = 1,
        CargaHorizontal = 2,
        CargaVertical = 3,
        AnimacionDardo = 4,
        Final = 5
    }

    public EstadoJuegoDiana EstadoActual = 0;
    public EstadoJuegoDiana EstadoAnterior = 0;
    
    [Header("Referencias")]
    [SerializeField] private Mira _Mira;
    [SerializeField] private CargaDardos _CargaDardos;
    [SerializeField] private AnimacionDardo _AnimacionDardo;
    [SerializeField] private GestorResultados _GestorResultados;

    [SerializeField] private Dardo _Dardo;
    [SerializeField] private CinemachineCamera _VCamJuego;
    [SerializeField] private CinemachineCamera _VCamFinal;

    [SerializeField] private GameObject _PanelInicio;
    [SerializeField] private GameObject _PanelTutorial;
    [SerializeField] private GameObject _PanelJuego;
    [SerializeField] private GameObject _PanelFinal;
    [SerializeField] private TMP_Text _TextoTirada;
    [SerializeField] private TMP_Text _TextoResultados;
    [SerializeField] private TMP_Text _TextoTotal;
    [SerializeField] private TMP_Text _TextoRecord;
    [SerializeField] private Button _BotonGuardarRecord;

    public int RecordDiana;

    [Header("Tiradas")]
    public int TiradasMaximas;
    public int TiradaActual;
    private int[] _Resultados;
    private int _ResultadoFinal;

    [Header("Dardos")]
    private List<Dardo> _DardosCreados;

    private void Awake()
    {
        _CargaDardos = FindAnyObjectByType<CargaDardos>();
        _AnimacionDardo = FindAnyObjectByType<AnimacionDardo>();
        _GestorResultados = FindAnyObjectByType<GestorResultados>();
        
        TiradasMaximas = PlayerPrefs.GetInt("TiradasDardos", 3);
        if (GestorBase.Instancia.EnModoHistoria)
        {
            TiradasMaximas = 3;
        }
        TiradaActual = 0;
        
        _DardosCreados = new List<Dardo>(TiradasMaximas);
        _Resultados = new int[TiradasMaximas];
    }
    private void Start()
    {
        GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuDardos");
        GestorJuego.LimitarRaton(false);
        
        _VCamJuego.Priority = 10;
        _VCamFinal.Priority = 0;
        
        CambiarPaneles();
        CrearDardos();
        
        _TextoTirada.text = "";
    }

    // MODIFICAR ESTADO DEL JUEGO
    public IEnumerator SiguienteEstado()
    {
        if (EstadoActual < EstadoJuegoDiana.Final)
        {
            yield return null; // Esperar 1 frame para que no se ejecuten varios estados a la vez
            EstadoActual = (EstadoJuegoDiana)((int)EstadoActual + 1); // Convertir enum a int, sumarle 1, volver a convertirlo a enum
            CambiarEstado(EstadoActual);
        }
    }
    private void CambiarEstado(EstadoJuegoDiana estado)
    {
        EstadoActual = estado;
        if (EstadoActual == EstadoAnterior)
        {
            return;
        }
        switch (EstadoActual)
        {
            case EstadoJuegoDiana.Inicio:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("MenuDardos");
                AlEntrar(EstadoActual);
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoDiana.Tutorial:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("PanelTutorial");
                AlEntrar(EstadoActual);
                AlEntrarTutorial();
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoDiana.CargaHorizontal:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("FondoDardos");
                AlEntrar(EstadoActual);
                AlEntrarCargaHorizontal();
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoDiana.CargaVertical:
                AlEntrar(EstadoActual);
                AlEntrarCargaVertical();
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoDiana.AnimacionDardo:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("LanzarDardos");
                AlEntrar(EstadoActual);
                AlEntrarAnimacionDardo();
                EstadoAnterior = EstadoActual;
                break;
            case EstadoJuegoDiana.Final:
                GestorSonido.Instancia.ReproducirMusicaDeFondo("PanelResultados");
                AlEntrar(EstadoActual);
                FinalJuegoDardos();
                break;
        }
    }

    // ESTADOS DEL JUEGO
    private void AlEntrar(EstadoJuegoDiana estado)
    {
        print($"Anterior: {EstadoAnterior}\nActual: {estado}");
        CambiarPaneles();
    }
    private void AlEntrarTutorial()
    {
        TiradaActual = 0;
        _TextoTirada.text = "";
    }
    private void AlEntrarCargaHorizontal()
    {
        IniciarTirada();
        _CargaDardos.CargaHorizontalActiva = true;
        _CargaDardos.TextoCarga.text = "Pulsa";
        _Mira.ReiniciarHorizontal();
        _DardosCreados[TiradaActual - 1].PrepararHorizontal();
        _CargaDardos.InicioCargaHorizontal();
    }
    private void AlEntrarCargaVertical()
    {
        _CargaDardos.CargaVerticalActiva = true;
        _CargaDardos.TextoCarga.text = "Mantén";
        _Mira.ReiniciarVertical();
        _DardosCreados[TiradaActual - 1].PrepararVertical();
        // InicioCargaVertical() al mantener el botón
    }
    private void AlEntrarAnimacionDardo()
    {
        GestorEfectosSonido.ReproducirEfecto("EfectoLanzarDardo");
        StartCoroutine(_AnimacionDardo.AnimacionCompleta());
    }
    private void FinalJuegoDardos()
    {
        print("Minijuego de dardos terminado!");
        _VCamJuego.Priority = 0;
        _VCamFinal.Priority = 10;
        CalcularResultados();
        _BotonGuardarRecord.interactable = _ResultadoFinal > PlayerPrefs.GetInt($"Record_{TiradasMaximas}");
        StringBuilder textoResultados = new StringBuilder();
        for (int i = 0; i < TiradasMaximas; i++)
        {
            print($"Resultados en tirada {i + 1}:\nHorizontal: {_CargaDardos.CargaHorizontal[i].ToString("F0")} || Vertical: {_CargaDardos.CargaVertical[i].ToString("F0")}");
            //textoResultados.AppendLine($"TIRADA {i + 1}: Horizontal: {_CargaDardos.CargaHorizontal[i].ToString("F0")} || Vertical: {_CargaDardos.CargaVertical[i].ToString("F0")}");
            textoResultados.AppendLine($"TIRADA {i + 1}: {_Resultados[i]} puntos.");
        }
        _TextoResultados.text = textoResultados.ToString();
        _TextoTotal.text = $"{_ResultadoFinal} puntos.";
        _TextoRecord.text = $"{PlayerPrefs.GetInt($"Record_{TiradasMaximas}")} puntos.";
        // Cambiar de escena
    }

    // TIRADAS
    private void IniciarTirada()
    {
        TiradaActual++;
        _TextoTirada.text = $"Tiradas: {TiradaActual} / {TiradasMaximas}";
    }
    public void FinalizarTirada()
    {
        if (TiradaActual < TiradasMaximas)
        {
            SiguienteTirada();
        }
        else
        {
            StartCoroutine(SiguienteEstado()); // Pasar al último estado
        }
    }
    public void SiguienteTirada() // No borrar !! Accesible desde botón Jugar en Unity
    {
        CambiarEstado(EstadoJuegoDiana.CargaHorizontal); // Volver a CargaHorizontal para siguiente tirada
    }
    public void IniciarTutorial()
    {
        CambiarEstado(EstadoJuegoDiana.Tutorial);
    }

    // DARDOS
    private void CrearDardos()
    {
        for (int i = 0; i < TiradasMaximas; i++)
        {
            Dardo nuevoDardo = Instantiate(_Dardo);
            _DardosCreados.Add(nuevoDardo);
            _DardosCreados[i].ActivarDesactivarDardo(false);
        }
        DefinirDardos();
    }
    private void DefinirDardos()
    {
        if (_DardosCreados.Count == 0)
        {
            print("No hay dardos creados");
            return;
        }
        for (int i = 0; i < _DardosCreados.Count; i++)
        {
            _DardosCreados[i].DefinirDardo(i);
        }
    }

    // CÁLCULOS ETC
    private void CambiarPaneles()
    {
        _PanelInicio.SetActive(EstadoActual == EstadoJuegoDiana.Inicio);
        _PanelTutorial.SetActive(EstadoActual == EstadoJuegoDiana.Tutorial);
        _PanelJuego.SetActive(EstadoActual == EstadoJuegoDiana.CargaHorizontal || EstadoActual == EstadoJuegoDiana.CargaVertical);
        _PanelFinal.SetActive(EstadoActual == EstadoJuegoDiana.Final);
    }
    private void CalcularResultados()
    {
        for (int i = 0; i < TiradasMaximas; i++)
        {
            _Resultados[i] = _GestorResultados.CalcularPuntosTirada(i);

            // Log para depurar
            Vector3 pos = _AnimacionDardo.DardosClavados[i].position;
            float dist = Vector3.Distance(pos, _GestorResultados._CentroDiana.position);
            Debug.Log($"Tirada {i + 1}: distanciaWorld={dist:F2}, radio={dist / _GestorResultados._RadioRealDiana:F3}, pts={_Resultados[i]}");
        }
        _ResultadoFinal = _Resultados.Sum();
    }
    public void JugarOtraVez()
    {
        GestorJuego.RecargarEscena();
    }
    public void BotonSalir()
    {
        GestorBase.Instancia.DesbloquearMinijuego(0);
        GestorBase.Instancia.VolverAlOrigen();
    }
    public void GuardarRecord()
    {
        PlayerPrefs.SetInt($"Record_{TiradasMaximas}", _ResultadoFinal);
        _TextoRecord.text = $"{_ResultadoFinal} puntos.";
    }
}