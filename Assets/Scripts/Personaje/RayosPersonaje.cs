using System;
using UnityEngine;

public class RayosPersonaje : MonoBehaviour
{
    private Transform _Transform;
    private Transform _TransformCamara;
    private SistemasPersonaje _Personaje;
    [SerializeField] private float _RangoDeInteraccion = 2f;

    private IInfoDisponible _InfoInteraccionAnterior;

    private void Awake()
    {
        TryGetComponent(out _Personaje);
        _Transform = transform;
        _TransformCamara = _Personaje.Camara.TransformCamara;
    }

    private void Update()
    {
        RayoInfo();
    }

    private void RayoInfo()
    {
        Ray rayo = new();
        rayo.origin = _TransformCamara.position;
        rayo.direction = _TransformCamara.forward;
        if(Physics.Raycast(rayo,out RaycastHit datosInteraccion, _RangoDeInteraccion))
        {
            Debug.DrawRay(rayo.origin, rayo.direction * datosInteraccion.distance, Color.blue);

            // Si tiene información que mostrar
            if(datosInteraccion.transform.TryGetComponent(out IInfoDisponible infoDisponible))
            {
                Debug.DrawRay(rayo.origin, rayo.direction * datosInteraccion.distance, Color.green);

                if (infoDisponible == _InfoInteraccionAnterior) return;
                if (_InfoInteraccionAnterior != null) _InfoInteraccionAnterior.AlOcultarInfo();
                infoDisponible.AlVerInfo();
                _InfoInteraccionAnterior = infoDisponible;
            }
            else
            {
                if (_InfoInteraccionAnterior != null)
                {
                    _InfoInteraccionAnterior.AlOcultarInfo();
                    _InfoInteraccionAnterior = null;
                }
            }
        }
        else
        {
            if (_InfoInteraccionAnterior != null)
            {
                _InfoInteraccionAnterior.AlOcultarInfo();
                _InfoInteraccionAnterior = null;
            }
        }
    }
    public void Interactuar()
    {
        Ray rayo = new(_TransformCamara.position, _TransformCamara.forward);
        if(Physics.Raycast(rayo,out RaycastHit datosInteraccion, _RangoDeInteraccion))
        {
            // Si es interactuable
            if(datosInteraccion.transform.TryGetComponent(out IInteractuable interactuable))
            {
                interactuable.AlInteractuar();
            }
        }
    }

    public void Jugar()
    {
        Ray rayo = new(_TransformCamara.position, _TransformCamara.forward);
        if (Physics.Raycast(rayo, out RaycastHit datosInteraccion, _RangoDeInteraccion))
        {
            // Si tiene un minijuego
            if(datosInteraccion.transform.TryGetComponent(out IJugable jugable))
            {
                jugable.AlJugar();
            }
        }

    }
    
}
