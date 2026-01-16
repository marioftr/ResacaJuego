using UnityEngine;

public class ControlesPersonaje : MonoBehaviour
{
    private InputSystem_Actions _Controles;
    private SistemasPersonaje _Personaje;
    private GestorHabitacion _GestorHabitacion;

    private void Awake()
    {
        _Controles = new();
        _Personaje = GetComponent<SistemasPersonaje>();
        _GestorHabitacion = FindAnyObjectByType<GestorHabitacion>();
    }
    private void OnEnable()
    {
        _Controles.Enable();
    }
    private void Update()
    {
        _DeteccionControles();
    }
    private void _DeteccionControles()
    {
        _Personaje.Movimiento.DireccionXZ = _Controles.Player.Move.ReadValue<Vector2>();
        _Personaje.Camara.EjesRaton = _Controles.Player.Look.ReadValue<Vector2>();

        if (_Controles.Player.Sprint.WasPressedThisFrame())
        {
            _Personaje.Movimiento.SistemaCorrer(true);
        }
        if (_Controles.Player.Sprint.WasReleasedThisFrame())
        {
            _Personaje.Movimiento.SistemaCorrer(false);
        }
        if (_Controles.Player.Crouch.WasPressedThisFrame())
        {
            _Personaje.Movimiento.SistemaAgacharse(true);
        }
        if (_Controles.Player.Crouch.WasReleasedThisFrame())
        {
            _Personaje.Movimiento.SistemaAgacharse(false);
        }
        if (_Controles.UI.Pause.WasPressedThisFrame())
        {
            _GestorHabitacion.Pausa();
        }
        if (_Controles.Player.Interact.WasPressedThisFrame())
        {
            _Personaje.RayosPersonaje.Interactuar();
        }
        if (_Controles.Player.Play.WasPressedThisFrame())
        {
            _Personaje.RayosPersonaje.Jugar();
        }
    }
    private void OnDisable()
    {
        _Controles.Disable();
    }
}
