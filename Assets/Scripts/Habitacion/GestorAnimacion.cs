using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GestorAnimacion : MonoBehaviour
{
    [SerializeField] private Camera _MainCamera;
    [SerializeField] private CinemachineCamera _CamaraAnimacion1;
    [SerializeField] private CinemachineCamera _CamaraAnimacion2;
    [SerializeField] private CinemachineCamera _CamaraAnimacion3;
    [SerializeField] private CinemachineCamera _CamaraAnimacion4;

    [SerializeField] private Volume _VolumenPostprocesado;
    private ColorAdjustments _ColorEscena;
    private Coroutine _CorutinaActual;
    
    private float _DuracionPlanos = 5.5f;
    private float _DuracionFade = 4f;
    
    private InputSystem_Actions _Controles;

    private void Awake()
    {
        _VolumenPostprocesado.profile.TryGet<ColorAdjustments>(out _ColorEscena);
        GestorJuego.LimitarRaton(true);
        _Controles = new();
    }

    private void OnEnable()
    {
        _Controles.Enable();
    }

    private void OnDisable()
    {
        _Controles.Disable();
    }
    private void Start()
    {
        GestorSonido.Instancia.ReproducirMusicaDeFondo("AnimacionInicio");
        StartCoroutine(PostprocesadoFundidos());
        StartCoroutine(PostprocesadoColores());
        StartCoroutine(Animacion());
    }
    private void Update()
    {
        if (_Controles.UI.Pause.WasPressedThisFrame())
        {
            StopAllCoroutines();
            GestorJuego.CargarEscena(2);
        }
    }
    private IEnumerator PostprocesadoColores()
    {
        _ColorEscena.colorFilter.value = Color.HSVToRGB(0f, 0.5f, 1f);
        yield return new WaitForSeconds(1f);
        float temporizador = 0f;
        while (temporizador < 25f)
        {
            temporizador += Time.deltaTime;
            float t = Mathf.PingPong(temporizador / 25f, 1f);
            float hue = Mathf.Lerp(0f, 1f, t);
            _ColorEscena.colorFilter.value = Color.HSVToRGB(hue, 0.5f, 1f);
            yield return null;
        }
        _ColorEscena.colorFilter.value = Color.HSVToRGB(1f, 0.5f, 1f);
        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator PostprocesadoFundidos()
    {
        _ColorEscena.postExposure.value = -10f;
        float temporizador = 0f;
        while (temporizador < _DuracionFade)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionFade;
            _ColorEscena.postExposure.value = Mathf.Lerp(-10,0,t);
            yield return null;
        }
        yield return new WaitForSeconds(20f);
        temporizador = 0f;
        while (temporizador < _DuracionFade)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionFade;
            _ColorEscena.postExposure.value = Mathf.Lerp(0,-10,t);
            yield return null;
        }
        _ColorEscena.postExposure.value = -10f;
        GestorSonido.Instancia.FadeOut();
    }
    
    private IEnumerator Animacion()
    {
        ResetearCamaras();
        _CamaraAnimacion1.Priority = 10;
        yield return StartCoroutine(Plano1());
        ResetearCamaras();
        _CamaraAnimacion2.Priority = 10;
        yield return StartCoroutine(Plano2());
        ResetearCamaras();
        _CamaraAnimacion3.Priority = 10;
        yield return StartCoroutine(Plano3());
        ResetearCamaras();
        _CamaraAnimacion4.Priority = 10;
        yield return StartCoroutine(Plano4());
        
        StopAllCoroutines();
        GestorJuego.CargarEscena(2);
    }
    private IEnumerator Plano1()
    {
        var posicionInicial = new Vector3(-0.55f, 0.45f, 3.55f);
        var euler = Quaternion.Euler(78.5f, 6.5f, 0f);
        
        var posicionFinal = new Vector3(-0.55f, 2.5f, 3.55f);
        
        _CamaraAnimacion1.ForceCameraPosition(posicionInicial, euler);
        _CamaraAnimacion1.Lens.Dutch = 0f;
        
        yield return new WaitForSeconds(2f);
        
        float temporizador = 0f;
        while (temporizador < _DuracionPlanos)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionPlanos;
            Vector3 posicionObjetivo = Vector3.Lerp(posicionInicial, posicionFinal, t);
            _CamaraAnimacion1.ForceCameraPosition(posicionObjetivo, euler);
            _CamaraAnimacion1.Lens.Dutch = Mathf.Lerp(0, 180, t);
            yield return null;
        }
        
        _CamaraAnimacion1.ForceCameraPosition(posicionFinal, euler);
        _CamaraAnimacion1.Lens.Dutch = 180f;
    }

    private IEnumerator Plano2()
    {
        var posicionInicial = new Vector3(-5.5f, 2.5f, 3.5f);
        var eulerInicial = Quaternion.Euler(56.5f, 5f, 0f);
        var rotacionInicial = eulerInicial.eulerAngles;
        
        var posicionFinal = new Vector3(-5.4f, 0.5f, 4.35f);
        var eulerFinal = Quaternion.Euler(56.5f, 365f, 0f);
        var rotacionFinal = eulerFinal.eulerAngles;
        
        _CamaraAnimacion2.ForceCameraPosition(posicionInicial, eulerInicial);
        _CamaraAnimacion2.Lens.Dutch = 180f;
        
        float temporizador = 0f;
        while (temporizador < _DuracionPlanos)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionPlanos;
            Vector3 posicionObjetivo = Vector3.Lerp(posicionInicial, posicionFinal, t);
            Vector3 rotacionObjetivo = Vector3.Lerp(rotacionInicial,rotacionFinal, t);
            Quaternion eulerObjetivo = Quaternion.Euler(rotacionObjetivo);
            _CamaraAnimacion2.ForceCameraPosition(posicionObjetivo, eulerObjetivo);
            _CamaraAnimacion2.Lens.Dutch = Mathf.Lerp(180, 0, t);
            yield return null;
        }
        
        _CamaraAnimacion2.ForceCameraPosition(posicionFinal, eulerFinal);
        _CamaraAnimacion2.Lens.Dutch = 0f;
    }

    private IEnumerator Plano3()
    {
        var posicionInicial = new Vector3(-6.15f, 0.03f, 0.15f);
        var euler = Quaternion.Euler(-28f, 225f, 0f);
        
        var posicionFinal = new Vector3(-4f, 0.03f, 1.5f);
        
        _CamaraAnimacion3.ForceCameraPosition(posicionInicial, euler);
        
        float temporizador = 0f;
        while (temporizador < _DuracionPlanos)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionPlanos;
            Vector3 posicionObjetivo = Vector3.Lerp(posicionInicial, posicionFinal, t);
            _CamaraAnimacion3.ForceCameraPosition(posicionObjetivo, euler);
            yield return null;
        }
        
        _CamaraAnimacion3.ForceCameraPosition(posicionFinal, euler);
    }

    private IEnumerator Plano4()
    {
        var posicionInicial = new Vector3(-4f, 1.3f, -0.65f);
        var eulerInicial = Quaternion.Euler(-1.5f, 80f, 0f);
        var rotacionInicial = eulerInicial.eulerAngles;

        var posicionFinal = new Vector3(-6.5f, 0.85f, -0.75f);
        var eulerFinal = Quaternion.Euler(-8.35f, 67f, 0f);
        var rotacionFinal = eulerFinal.eulerAngles;

        _CamaraAnimacion4.ForceCameraPosition(posicionInicial, eulerInicial);
        
        float temporizador = 0f;
        while (temporizador < _DuracionPlanos)
        {
            temporizador += Time.deltaTime;
            float t = temporizador / _DuracionPlanos;
            Vector3 posicionObjetivo = Vector3.Lerp(posicionInicial, posicionFinal, t);
            Vector3 rotacionObjetivo = Vector3.Lerp(rotacionInicial, rotacionFinal, t);
            Quaternion eulerObjetivo = Quaternion.Euler(rotacionObjetivo);
            _CamaraAnimacion4.ForceCameraPosition(posicionObjetivo, eulerObjetivo);
            yield return null;
        }
        
        _CamaraAnimacion4.ForceCameraPosition(posicionFinal, eulerFinal);

        yield return new WaitForSeconds(4f);
    }
    private void ResetearCamaras()
    {
        _CamaraAnimacion1.Priority = 0;
        _CamaraAnimacion2.Priority = 0;
        _CamaraAnimacion3.Priority = 0; 
        _CamaraAnimacion4.Priority = 0;
    }
}
