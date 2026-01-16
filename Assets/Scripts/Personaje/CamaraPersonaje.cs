using NUnit.Framework.Constraints;
using UnityEngine;

public class CamaraPersonaje : MonoBehaviour
{
    [SerializeField] public Transform TransformCamara;
    [SerializeField] private float _Sensibilidad = 0.1f;
    [SerializeField] private float _LimiteCamara = 90;
    public Vector2 EjesRaton;
    public Vector2 RotacionCamara;
    private Transform _Transform;
    
    public bool CamaraActiva;

    private void Awake()
    {
        _Transform = transform;
    }
    private void Start()
    {
        GestorJuego.LimitarRaton(true);
    }
    private void Update()
    {
        MovimientoCamara();
    }
    public void LimitarRaton(bool bloqueado)
    {
        if (bloqueado)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void MovimientoCamara()
    {
        if (TransformCamara == null)
        {
            Debug.LogError("No tengo asignada una cámara en Unity.");
            return;
        }

        if (CamaraActiva == false) return;
        
        // Girar el personaje en su eje Y usando el EjesRaton.x
        RotacionCamara.y = EjesRaton.x * _Sensibilidad;
        _Transform.localEulerAngles += new Vector3(0, RotacionCamara.y, 0);

        // Girar la cámara en su eje X usando el EjesRaton.y
        RotacionCamara.x += EjesRaton.y * _Sensibilidad;
        // Clamp limita un valor entre dos valores
        RotacionCamara.x = Mathf.Clamp(RotacionCamara.x, -_LimiteCamara, _LimiteCamara);

        TransformCamara.localEulerAngles = new Vector3(-RotacionCamara.x, 0, 0);
    }
}