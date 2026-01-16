using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CargaDardos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GestorJuegoDiana _GestorJuegoDiana;
    [SerializeField] private Image _IMGCargaVertical;
    [SerializeField] private Image _IMGCargaHorizontalIzquierda;
    [SerializeField] private Image _IMGCargaHorizontalDerecha;
    public TMP_Text TextoCarga;

    public float Carga;

    [Header("Carga Horizontal")]
    public float[] CargaHorizontal;
    public float[] CargaHorizontalNormalizada;
    public bool CargaHorizontalActiva = false;
    public bool DebeMoverseHorizontal = false;
    [SerializeField] private bool _CargaHorizontalSubiendo = true;
    [SerializeField] private float _VelocidadCargaHorizontal = 100f;

    [Header("Carga Vertical")]
    public float[] CargaVertical;
    public float[] CargaVerticalNormalizada;
    public bool CargaVerticalActiva = false;
    public bool DebeMoverseVertical = false;
    [SerializeField] private bool _CargaVerticalSubiendo = true;
    [SerializeField] private float _VelocidadCargaVertical = 100f;

    private void Awake()
    {
        _GestorJuegoDiana = FindAnyObjectByType<GestorJuegoDiana>();
    }
    private void Start()
    {
        ReiniciarCarga();
        CargaHorizontal = new float[_GestorJuegoDiana.TiradasMaximas];
        CargaHorizontalNormalizada = new float[_GestorJuegoDiana.TiradasMaximas];
        CargaVertical = new float[_GestorJuegoDiana.TiradasMaximas];
        CargaVerticalNormalizada = new float[_GestorJuegoDiana.TiradasMaximas];
    }
    private void Update()
    {
        if (DebeMoverseHorizontal)
        {
            MovimientoCargaHorizontal();
        }
        if (DebeMoverseVertical)
        {
            MovimientoCargaVertical();
        }
    }

    // CARGA HORIZONTAL
    public void InicioCargaHorizontal()
    {
        if (!CargaHorizontalActiva) return;
        DebeMoverseHorizontal = true;
        Carga = 50f;
        _IMGCargaHorizontalIzquierda.fillAmount = (Carga - 2f) / 100f;
        _IMGCargaHorizontalDerecha.fillAmount = (98f - Carga) / 100f;
    }
    public void MovimientoCargaHorizontal()
    {
        if (!CargaHorizontalActiva) return;
        if (_CargaHorizontalSubiendo)
        {
            Carga += Time.deltaTime * _VelocidadCargaHorizontal;
            if (Carga > 100)
            {
                _CargaHorizontalSubiendo = false;
                Carga = 100;
            }
        }
        else
        {
            Carga -= Time.deltaTime * _VelocidadCargaHorizontal;
            if (Carga < 0)
            {
                _CargaHorizontalSubiendo = true;
                Carga = 0;
            }
        }
        _IMGCargaHorizontalIzquierda.fillAmount = (Carga - 2f) / 100f;
        _IMGCargaHorizontalDerecha.fillAmount = (98f - Carga) / 100f;
    }
    public void FinCargaHorizontal()
    {
        if (_GestorJuegoDiana.EstadoActual != GestorJuegoDiana.EstadoJuegoDiana.CargaHorizontal) return;
        if (!CargaHorizontalActiva) return;
        DebeMoverseHorizontal = false;
        TextoCarga.text = "";
        _CargaHorizontalSubiendo = true;
        GuardarCargaHorizontal(Carga);
        StartCoroutine(_GestorJuegoDiana.SiguienteEstado());
    }
    private void GuardarCargaHorizontal(float cargaHorizontal)
    {
        int indice = _GestorJuegoDiana.TiradaActual - 1;
        if (indice < 0 || indice >= CargaHorizontal.Length)
        {
            Debug.LogWarning($"Índice inválido para carga horizontal: {cargaHorizontal}");
            return;
        }
        
        CargaHorizontal[indice] = cargaHorizontal;
        print(CargaHorizontal[indice].ToString("F0"));
        CargaHorizontalNormalizada[indice] = (cargaHorizontal - 50) / 50; // X normalizada (-1 a +1)
        Carga = 0;
        CargaHorizontalActiva = false;
    }

    // CARGA VERTICAL
    public void InicioCargaVertical()
    {
        if (_GestorJuegoDiana.EstadoActual != GestorJuegoDiana.EstadoJuegoDiana.CargaVertical) return;
        if (!CargaVerticalActiva) return;
        DebeMoverseVertical = true;
        Carga = 0;
    }
    public void MovimientoCargaVertical()
    {
        if (!CargaVerticalActiva) return;
        if (_CargaVerticalSubiendo)
        {
            Carga += Time.deltaTime * _VelocidadCargaVertical;

            if (Carga > 100)
            {
                _CargaVerticalSubiendo = false;
                Carga = 100;
            }
        }
        else
        {
            Carga -= Time.deltaTime * _VelocidadCargaVertical;
            if (Carga < 0)
            {
                _CargaVerticalSubiendo = true;
                Carga = 0;
            }
        }
        _IMGCargaVertical.fillAmount = Carga / 100;
    }
    public void FinCargaVertical()
    {
        if (_GestorJuegoDiana.EstadoActual != GestorJuegoDiana.EstadoJuegoDiana.CargaVertical) return;
        if (!CargaVerticalActiva) return;
        DebeMoverseVertical = false;
        TextoCarga.text = "";
        _CargaVerticalSubiendo = true;
        GuardarCargaVertical(Carga);
        StartCoroutine(_GestorJuegoDiana.SiguienteEstado());
    }
    private void GuardarCargaVertical(float cargaVertical)
    {
        int indice = _GestorJuegoDiana.TiradaActual - 1;
        if (indice < 0 || indice >= CargaVertical.Length)
        {
            Debug.LogWarning($"Índice inválido para carga vertical: {cargaVertical}");
            return;
        }
        
        CargaVertical[indice] = cargaVertical;
        print(CargaVertical[indice].ToString("F0"));
        CargaVerticalNormalizada[indice] = (cargaVertical - 50) / 50; // Y normalizada (-1 a +1)
        ReiniciarCarga();
    }
    private void ReiniciarCarga()
    {
        Carga = 0f;

        _IMGCargaHorizontalIzquierda.fillAmount = 0.48f;
        _IMGCargaHorizontalDerecha.fillAmount = 0.48f;
        CargaHorizontalActiva = false;
        DebeMoverseHorizontal = false;

        _IMGCargaVertical.fillAmount = 0f;
        CargaVerticalActiva = false;
        DebeMoverseVertical = false;
    }
}
