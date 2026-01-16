using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class AnimacionDardo : MonoBehaviour
{
    [Header("Referencias")]
    private GestorJuegoDiana _GestorJuegoDiana;
    private CargaDardos _CargaDardos;
    private ParticleSystem _Particulas;

    [SerializeField] private GameObject _DardoVolando;
    [SerializeField] private GameObject _DardoDiana;
    [SerializeField] private Transform _DardoClavado;
    public List<Transform> DardosClavados = new List<Transform>();

    [Header("C�maras")]
    [SerializeField] private CinemachineCamera _VCamJuego;
    [SerializeField] private CinemachineCamera _VCamLanzamiento;
    [SerializeField] private CinemachineCamera _VCamDiana;

    [Header("Diana")]
    [SerializeField] private Transform _CentroDiana;
    [SerializeField] private float _RadioDiana = 130f;

    private void Awake()
    {
        _GestorJuegoDiana = FindAnyObjectByType<GestorJuegoDiana>();
        _CargaDardos = FindAnyObjectByType<CargaDardos>();
        _Particulas = _DardoVolando.GetComponentInChildren<ParticleSystem>();
    }
    private void Start()
    {
        _DardoVolando.SetActive(false);
        _DardoDiana.SetActive(false);
        DardosClavados.Clear();

        for (int i=0; i < _GestorJuegoDiana.TiradasMaximas; i++)
        {
            Transform nuevoDardo = Instantiate(_DardoClavado);
            nuevoDardo.gameObject.SetActive(false);
            DardosClavados.Add(nuevoDardo);
        }
        _VCamJuego.Priority = 10;
        _VCamLanzamiento.Priority = 0;
        _VCamDiana.Priority = 0;
    }
    public IEnumerator AnimacionCompleta()
    {
        _DardoVolando.SetActive(true);
        _DardoDiana.SetActive(true);

        Vector3 posicionInicialVolando = new Vector3 (-90,130,-280);
        Vector3 posicionFinalVolando = new Vector3(-90, 45, -130);
        Vector3 posicionInicialDiana = new Vector3 (-120,100,170);
        Vector3 posicionDardoFinal = CalcularPosicionDardoFinal();

        _Particulas.Clear();
        _Particulas.Play();

        yield return StartCoroutine(FaseLanzamiento(posicionInicialVolando, posicionFinalVolando));
        yield return StartCoroutine(FaseDiana(posicionInicialDiana, posicionDardoFinal));

        _DardoVolando.SetActive(false);
        ColocarDardoClavado(posicionDardoFinal);
        yield return new WaitForSeconds(0.5f);

        _VCamJuego.Priority = 10;
        _VCamLanzamiento.Priority = 0;
        _VCamDiana.Priority = 0;
        _GestorJuegoDiana.FinalizarTirada();
    }
    private IEnumerator FaseLanzamiento(Vector3 posicionInicial, Vector3 posicionFinal)
    {
        _DardoVolando.SetActive(true);
        _DardoDiana.SetActive(false);

        _VCamJuego.Priority= 0;
        _VCamLanzamiento.Priority= 10;
        _VCamDiana.Priority= 0;

        float duracionMaxima = 2f;
        float temporizador = 0f;
        while (temporizador < duracionMaxima)
        {
            temporizador += Time.deltaTime;
            _DardoVolando.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, temporizador/duracionMaxima);
            _VCamLanzamiento.Lens.FieldOfView = Mathf.Lerp(5f, 15f, temporizador / duracionMaxima);
            yield return null;
        }
    }
    private IEnumerator FaseDiana(Vector3 posicionInicial, Vector3 posicionFinal)
    {
        _DardoVolando.SetActive(false);
        _DardoDiana.SetActive(true);

        _VCamJuego.Priority = 0;
        _VCamLanzamiento.Priority = 0;
        _VCamDiana.Priority = 10;

        float duracionMaxima = 3f;
        float temporizador = 0f;
        while (temporizador < duracionMaxima)
        {
            temporizador += Time.deltaTime;
            _DardoDiana.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, temporizador/duracionMaxima);
            _DardoDiana.transform.LookAt(posicionFinal);
            _DardoDiana.transform.Rotate(0, 180, 0);
            yield return null;
        }

        _Particulas.Stop();
        _Particulas.Clear();
    }
    public Vector3 CalcularPosicionDardoFinal()
    {
        float hNormalizada = _CargaDardos.CargaHorizontalNormalizada[_GestorJuegoDiana.TiradaActual - 1];
        float vNormalizada= _CargaDardos.CargaVerticalNormalizada[_GestorJuegoDiana.TiradaActual-1];
        float offsetX = hNormalizada * _RadioDiana;
        float offsetY = vNormalizada * _RadioDiana;
        return _CentroDiana.position + new Vector3(offsetX, offsetY, 0f);
    }
    private void ColocarDardoClavado(Vector3 posicionFinal)
    {
        GestorEfectosSonido.ReproducirEfecto("EfectoClavarDardo");
        Transform dardoActual = DardosClavados[_GestorJuegoDiana.TiradaActual - 1];
        dardoActual.position = posicionFinal;
        dardoActual.eulerAngles = new Vector3(180f, 0f, 0f);
        dardoActual.gameObject.SetActive(true);
        
        _DardoVolando.SetActive(false);
        _DardoDiana.SetActive(false);
    }
}
