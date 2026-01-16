using UnityEngine;

public class GestorResultados : MonoBehaviour
{
    [Header("Referencias")]
    private GestorJuegoDiana _GestorJuegoDiana;
    private CargaDardos _CargaDardos;
    private AnimacionDardo _AnimacionDardo;

    [Header("Diana")]
    private int[] _Sectores = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };
    private int _IndiceSector;
    [SerializeField] private float _RadioCentroInterior = 5f;
    [SerializeField] private float _RadioCentroExterior = 11f;
    [SerializeField] private float _RadioInicioTriple = 56f;
    [SerializeField] private float _RadioFinTriple = 64f;
    [SerializeField] private float _RadioInicioDoble = 97f;
    [SerializeField] private float _RadioFinDoble = 106f;

    public float _Angulo;
    public float _Radio;

    public Transform _CentroDiana;
    public float _RadioRealDiana = 130f;

    [Header("Puntos")]
    public int _PuntosBase;
    private int _Multiplicador;

    private void Awake()
    {
        _GestorJuegoDiana = FindAnyObjectByType<GestorJuegoDiana>();
        _CargaDardos = FindAnyObjectByType<CargaDardos>();
        _AnimacionDardo = FindAnyObjectByType<AnimacionDardo>();
    }
    public int CalcularPuntosTirada(int indiceTirada)
    {
        if (indiceTirada < 0 || indiceTirada >= _GestorJuegoDiana.TiradasMaximas)
        {
            Debug.LogWarning($"Índice inválido: {indiceTirada}");
            return 0;
        }
        
        Vector3 posicionDardo = _AnimacionDardo.DardosClavados[indiceTirada].position;
        Vector3 direccionDardo = posicionDardo - _CentroDiana.position;
        _Radio = direccionDardo.magnitude;

        if (_Radio > _RadioFinDoble) return 0;

        if (_Radio < _RadioCentroInterior) return 50;
        if (_Radio < _RadioCentroExterior) return 25;

        // Calcular el ángulo en radianes (Atan2) y pasarlo a grados (Rad2Deg), de -180º a +180º
        _Angulo = Mathf.Atan2(direccionDardo.x, direccionDardo.y) * Mathf.Rad2Deg;
        print($"Angulo {indiceTirada}: { _Angulo}");
        if (_Angulo < 0f) _Angulo += 360f; // Convierte a 0º-360º para utilizar valores positivos

        // Dividir la diana en 20 sectores
        float anguloSector = 360f / 20f; // 18º por sector
        float medioSector = anguloSector / 2f; // 9º
        float anguloAjustado = (_Angulo + medioSector) % 360f;
        _IndiceSector = Mathf.FloorToInt(anguloAjustado / anguloSector) % 20;
        // Divididmos el ángulo ajustado por los 18º que mide cada sector
        // Mathf.FloorToInt redondea el resultado hacia abajo y lo devuelve como int (para obtener el nº del sector)
        // %20 aplica módulo 20 (devuelve el resto de dividir entre 20), asegurando índice válido en un array de 20 elementos
        
        _PuntosBase = _Sectores[_IndiceSector];

        _Multiplicador = 1;
        if (_Radio > _RadioInicioTriple && _Radio < _RadioFinTriple)
        {
            _Multiplicador = 3;
        }
        else if (_Radio > _RadioInicioDoble && _Radio < _RadioFinDoble)
        {
            _Multiplicador = 2;
        }
        Debug.Log($"Radio: {_Radio:F3}, Multi: {_Multiplicador}");

        return _PuntosBase * _Multiplicador;
    }
}
