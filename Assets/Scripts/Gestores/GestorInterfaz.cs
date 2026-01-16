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
    [SerializeField] private Toggle _ToggleRecords;
    [SerializeField] private Image _FondoRecords;
    
    [SerializeField] private TMP_Text _RecordDardos3;
    [SerializeField] private TMP_Text _RecordDardos5;
    [SerializeField] private TMP_Text _RecordFlautaCF;
    [SerializeField] private TMP_Text _RecordFlautaSeccionesCF;
    
    private Color _Naranja = new Color(0.8f, 0.2901961f, 0f, 1f);

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
            _ToggleRecords.isOn = true;
            _ToggleRecords.enabled = true;
            _FondoRecords.color = _Naranja;
        }
        else
        {
            _ToggleRecords.isOn = false;
            _ToggleRecords.enabled = false;
            _FondoRecords.color = Color.gray;
        }
    }
    public void AbrirRecords()
    {
        if (!_ToggleRecords.isOn) return;
        
        GestorJuego.MostrarMenuRecords();
        GestorBase.Instancia.CargarPartidaManual();
        
        Instancia._RecordDardos3.text = GestorBase.Instancia.PuntuacionDardos3.ToString();
        Instancia._RecordDardos5.text = GestorBase.Instancia.PuntuacionDardos5.ToString();
        Instancia._RecordFlautaCF.text = GestorBase.Instancia.PuntuacionFlautaTotalCF.ToString();
        int[] cf = new int[4]
        {
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[0],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[1],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[2],
            GestorBase.Instancia.PuntuacionFlautaSeccionesCF[3]
        };
        Instancia._RecordFlautaSeccionesCF.text = new string($"{cf[0]}+{cf[1]}+{cf[2]}+{cf[3]}");
    }
}
