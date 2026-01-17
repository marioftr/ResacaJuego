using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Velocidad")]
    [SerializeField] private float _VelocidadBase = 1.5f;
    [SerializeField] private float _ModificadorAlCorrer = 1.5f;
    private float _VelocidadFinal;

    [Header("Información")]
    [SerializeField] private Vector3 _Direccion;
    public Vector3 DireccionXZ;
    private Rigidbody _Rigidbody;
    private Transform _Transform;
    private SistemasPersonaje _Personaje;

    [Header("Animaciones")]
    [SerializeField] private float _AlturaDePie = 2f;
    [SerializeField] private float _AlturaAgachado = 1.5f;
    private CapsuleCollider _Collider;
    private bool _Corriendo;
    private bool _Agachado;
    
    [Header("Audios")]
    [SerializeField] private AudioMixerGroup _SalidaEfectos;
    [SerializeField] private AudioSource _Audio;
    [SerializeField] private AudioClip _EfectoCorrer;

    private void Awake()
    {
        TryGetComponent(out _Personaje);
        _Rigidbody = GetComponent<Rigidbody>();
        _Collider = GetComponent<CapsuleCollider>();
        _Transform = transform;
        ConfigurarAudios();
    }
    private void Start()
    {
        SistemaCorrer(false);
        SistemaAgacharse(false);
        if (!File.Exists(GestorBase.Instancia.RutaDeGuardado))
        {
            _Transform.position = new Vector3(-6f,0.8f,0.3f);
            _Transform.localEulerAngles = new Vector3(0, 45, 0);
        }
        _Transform.position = GestorBase.Instancia.PosicionJugadorEnHabitacion;
        _Transform.rotation = GestorBase.Instancia.RotacionJugadorEnHabitacion;
    }
    private void Update()
    {
        CalcularMovimiento();
        CalcularSonidoPasos();
    }
    private void CalcularMovimiento()
    {
        if (_Rigidbody == null)
        {
            Debug.LogError("Falta añadir un Rigidbody");
            return;
        }
        _Direccion.x = DireccionXZ.x;
        _Direccion.z = DireccionXZ.y;
        _Direccion *= _VelocidadFinal;
        _Direccion.y = 0;
        _Direccion = _Transform.TransformDirection(_Direccion);
        _Rigidbody.linearVelocity = _Direccion;
    }
    public void SistemaCorrer (bool corriendo)
    {
        _Corriendo= corriendo;
        if (_Corriendo)
        {
            _VelocidadFinal = _VelocidadBase * _ModificadorAlCorrer;
        }
        else
        {
            _VelocidadFinal = _VelocidadBase;
        }
    }
    public void SistemaAgacharse(bool agachado)
    {
        _Agachado = agachado;
        if (_Agachado)
        {
            _VelocidadFinal = _VelocidadFinal / _ModificadorAlCorrer;
            _Collider.height = _AlturaAgachado;
            _Collider.center = new Vector3(0, -0.25f, 0);
        }
        else
        {
            _VelocidadFinal = _VelocidadBase;
            _Collider.height = _AlturaDePie;
            _Collider.center = Vector3.zero;
        }
    }

    private void ConfigurarAudios()
    {
        _Audio.playOnAwake = false;
        _Audio.loop = true;
        _Audio.outputAudioMixerGroup = _SalidaEfectos;
        _Audio.volume = 0.3f;
        _Audio.clip = _EfectoCorrer;
    }
    private void CalcularSonidoPasos()
    {
        float velocidad = _Rigidbody.linearVelocity.magnitude;
        if (velocidad >= _VelocidadBase && !_Corriendo)
        {
            _Audio.pitch = 0.8f;
            if(!_Audio.isPlaying) _Audio.Play();
        }
        else if (velocidad >= _VelocidadBase && _Corriendo)
        {
            _Audio.pitch = 1f;
            if(!_Audio.isPlaying) _Audio.Play();
        }
        else
        {
            _Audio.pitch = 1f;
            _Audio.Stop();
        }
    }
}