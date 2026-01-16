using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GestorInterfaz : MonoBehaviour
{
    public static GestorInterfaz Instancia { get; private set; }
    
    [SerializeField] private Toggle _MuteVolumen;
    [SerializeField] private Image _FondoVolumen;
    [SerializeField] private Image _VolumenOn;
    [SerializeField] private Image _VolumenOff;
    [SerializeField] private Toggle _Creditos;
    [SerializeField] private Image _FondoCreditos;
    [SerializeField] private GameObject _PanelCreditos;
    [SerializeField] private Button _BotonRecords;
    [SerializeField] private Image _FondoRecords;
    
    [SerializeField] private TMP_Text _RecordDardos3;
    [SerializeField] private TMP_Text _RecordDardos5;
    [SerializeField] private TMP_Text _RecordFlautaCF;
    [SerializeField] private TMP_Text _RecordFlautaSeccionesCF;
    
    private readonly Color _Naranja = new Color(0.8f, 0.2901961f, 0f, 1f);

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        
        _MuteVolumen.isOn = false;
        _Creditos.isOn = false;
    }
    private void Start()
    {
        SilenciarVolumen();
        AbrirCreditos();
    }

    public void SilenciarVolumen()
    {
        if (_MuteVolumen.isOn)
        {
            _VolumenOn.gameObject.SetActive(false);
            _VolumenOff.gameObject.SetActive(true);
            _FondoVolumen.color = _Naranja;
            AudioListener.volume = 0;
        }
        else
        {
            _VolumenOn.gameObject.SetActive(true);
            _VolumenOff.gameObject.SetActive(false);
            _FondoVolumen.color = Color.white;
            AudioListener.volume = 1;
        }
    }
    public void AbrirCreditos()
    {
        if (_Creditos.isOn)
        {
            _FondoCreditos.color = _Naranja;
            _PanelCreditos.SetActive(true);
        }
        else
        {
            _FondoCreditos.color = Color.white;
            _PanelCreditos.SetActive(false);
        }
    }

    public void CambiarToggleRecords()
    {
        bool recordsDisponible = GestorBase.Instancia.EstaDesbloqueado(0) && GestorBase.Instancia.EstaDesbloqueado(1);
        if (recordsDisponible)
        {
            _BotonRecords.interactable = true;
            _FondoRecords.color = _Naranja;
        }
        else
        {
            _BotonRecords.interactable = false;
            _FondoRecords.color = Color.gray;
        }
    }
    public void AbrirRecords()
    {
        GestorJuego.MostrarMenuRecords();
        GestorBase.Instancia.CargarPartidaManual();

        var dardos3 = GestorBase.Instancia.PuntuacionDardos3;
        var dardos5 = GestorBase.Instancia.PuntuacionDardos5;
        var flautaCF = GestorBase.Instancia.PuntuacionFlautaTotalCF;
        var cf = new int[4]
        {
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[0],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[1],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[2],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[3]
        };

        _RecordDardos3.text = dardos3.ToString();
        
        _RecordDardos3.text = $"{dardos3} {(dardos3 == 1 ? "punto" : "puntos")}";
        _RecordDardos5.text = $"{dardos5} {(dardos5 == 1 ? "punto" : "puntos")}";
        _RecordFlautaCF.text = $"{flautaCF} {(flautaCF == 1 ? "error" : "errores")}";

        _RecordFlautaSeccionesCF.text = $"({cf[0]}+{cf[1]}+{cf[2]}+{cf[3]})";
    }
}
