using UnityEngine;

public class Dardo : MonoBehaviour
{
    [Header("Referencias")]
    private GestorJuegoDiana _GestorJuegoDiana;
    private CargaDardos _CargaDardos;
    private Transform _Transform;
    private bool _Activo;
    private bool _ActivoPrevio = false;

    public int Numero;

    private Vector3 _PosicionInicial = new Vector3(-40, -25, 150);
    private Vector3 _RotacionInicial = new Vector3(180, -15, 0);

    [Header("Offset")]
    private float _OffsetRealH;
    private float _OffsetRealV;
    private float _OffsetSuaveH;
    private float _OffsetSuaveV;
    private float _OffsetMaximoH;
    private float _OffsetMaximoV;
    private float _VelocidadSuavizado;
    private float _BaseVerticalX;
    private float _BaseVerticalY;

    private void Awake()
    {
        _GestorJuegoDiana = FindAnyObjectByType<GestorJuegoDiana>();
        _CargaDardos = FindAnyObjectByType<CargaDardos>();
        _Transform = transform;
    }
    private void Start()
    {
        _Transform.position = _PosicionInicial;
        _Transform.eulerAngles = _RotacionInicial;
        _VelocidadSuavizado = 5f;
        _OffsetMaximoH = 50f;
        _OffsetMaximoV = 80f;
    }
    private void Update()
    {
        _Activo = (Numero == _GestorJuegoDiana.TiradaActual); // Comprueba si el número del dardo coincide con el número de la tirada
        if (_Activo != _ActivoPrevio)
        {
            ActivarDesactivarDardo(_Activo); // Cambia su estado
            _ActivoPrevio = _Activo; // Guarda su estado para no volver a cambiarlo
        }
        if (!_Activo) return;
        MovimientoDardo();
    }
    private void MovimientoDardo()
    {
        if (_GestorJuegoDiana.EstadoActual == GestorJuegoDiana.EstadoJuegoDiana.CargaHorizontal)
        {
            _OffsetRealH = (_CargaDardos.Carga - 50f) / 50f; // -1 (-50)... 0 ... +1 (+50)
            _OffsetSuaveH = Mathf.Lerp(_OffsetSuaveH, _OffsetRealH, _VelocidadSuavizado * Time.deltaTime); // Dardo sigue el nivel de Carga
            _Transform.position = _PosicionInicial + new Vector3(_OffsetSuaveH * _OffsetMaximoH, 0, 0);
        }
        if (_GestorJuegoDiana.EstadoActual == GestorJuegoDiana.EstadoJuegoDiana.CargaVertical)
        {
            _OffsetRealV = (_CargaDardos.Carga / 100f); // -1 (0) ... 0 (+50) ... +1 (+100)
            _OffsetSuaveV = Mathf.Lerp(_OffsetSuaveV, _OffsetRealV, _VelocidadSuavizado * Time.deltaTime);
            _Transform.position = new Vector3(_BaseVerticalX, _BaseVerticalY + _OffsetSuaveV * _OffsetMaximoV, _Transform.position.z);
        }
    }
    public void PrepararHorizontal()
    {
        _OffsetSuaveH = 0f;
    }
    public void PrepararVertical()
    {
        _OffsetSuaveV = 0f;
        _BaseVerticalX = _Transform.position.x;
        _BaseVerticalY = _Transform.position.y;
    }
    public void DefinirDardo(int numero)
    {
        Numero = numero + 1;
    }
    public void ActivarDesactivarDardo(bool opcion)
    {
        GestorJuego.ActivarDesactivarObjeto(gameObject, opcion);
    }
}
