using UnityEngine;

public class Mira : MonoBehaviour
{
    [Header("Referencias")]
    private GestorJuegoDiana _GestorJuegoDiana;
    private CargaDardos _CargaDardos;
    private RectTransform _Transform;

    [Header("Offset")]
    private float _OffsetH;
    private float _OffsetV;
    [SerializeField] private float _OffsetMaximoH;
    [SerializeField] private float _OffsetMaximoV;
    private float _BaseH;
    private float _BaseV;

    private Vector3 _PosicionInicial = new Vector3(-380, -400, 0);

    private void Awake()
    {
        _GestorJuegoDiana = FindAnyObjectByType<GestorJuegoDiana>();
        _CargaDardos = FindAnyObjectByType<CargaDardos>();
        _Transform = GetComponent<RectTransform>();
        _OffsetMaximoH = 450;
        _OffsetMaximoV = 850;
    }
    private void Start()
    {
        _Transform.localPosition = _PosicionInicial;
    }
    private void Update()
    {
        if (_GestorJuegoDiana.EstadoActual == GestorJuegoDiana.EstadoJuegoDiana.CargaHorizontal)
        {
            MovimientoMiraH();
        }
        if(_GestorJuegoDiana.EstadoActual == GestorJuegoDiana.EstadoJuegoDiana.CargaVertical)
        {
            MovimientoMiraV();
        }
    }
    private void MovimientoMiraH()
    {
        _OffsetH = (_CargaDardos.Carga - 50f) / 50f;
        _Transform.localPosition = _PosicionInicial + new Vector3(_OffsetH * _OffsetMaximoH, 0, 0);
    }
    private void MovimientoMiraV()
    {
        _OffsetV = (_CargaDardos.Carga / 100f);
        _Transform.localPosition = new Vector3(_BaseH, _BaseV + _OffsetV * _OffsetMaximoV);
    }
    public void ReiniciarHorizontal()
    {
        _BaseH = _PosicionInicial.x;
    }
    public void ReiniciarVertical()
    {
        _BaseH = _PosicionInicial.x + _CargaDardos.CargaHorizontalNormalizada[_GestorJuegoDiana.TiradaActual - 1] * _OffsetMaximoH;
        _BaseV = _Transform.localPosition.y;
    }
}
