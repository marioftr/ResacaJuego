using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;  // Para Handles.Label (opcionalmente)
#endif

public class DianaGizmos : MonoBehaviour
{
    public Transform centroDiana;       // Centro de la diana (GameObject vac�o)
    public Transform ultimoImpacto;     // Transform del �ltimo dardo clavado (posici�n de impacto)
    [SerializeField] private AnimacionDardo _AnimacionDardo;

    // Colores personalizables
    public Color colorLineasSectores = Color.white;
    public Color colorSectorImpacto = Color.yellow;
    public Color colorAngulo = Color.cyan;

    void OnDrawGizmos()
    {
        Transform ultimoImpacto = null;

        if (_AnimacionDardo != null && _AnimacionDardo.DardosClavados.Count > 0)
        {
            int ultimaTirada = _AnimacionDardo.DardosClavados.Count - 1;
            ultimoImpacto = _AnimacionDardo.DardosClavados[ultimaTirada];
        }

        if (centroDiana == null) return;
        Vector3 centro = centroDiana.position;
        float radio = 1.0f; 
        int totalSectores = 20;
        float anguloSector = 360f / totalSectores;
        float halfSector = anguloSector / 2f;

        // Determinar �ngulo de impacto actual (en grados desde arriba, horario)
        float anguloDardo = 0f;
        if (ultimoImpacto != null)
        {
            Vector3 dir = ultimoImpacto.position - centro;
            anguloDardo = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            if (anguloDardo < 0) anguloDardo += 360f;
            // Aplicar mismo offset de correcci�n que en la l�gica de juego, si existe:
            anguloDardo = (anguloDardo - 90f + 360f) % 360f;  // <- ejemplo restando 90� de offset
        }

        // **1. Dibujar las l�neas de todos los bordes de sector**
        Gizmos.color = colorLineasSectores;
        for (int i = 0; i < totalSectores; i++)
        {
            // L�nea en �ngulo correspondiente al borde (i * 18� - 9� para centrar 20 en 0�)
            float anguloBorde = (i * anguloSector) - halfSector;
            // Aplicar el mismo offset de orientaci�n del modelo
            anguloBorde = (anguloBorde - 90f) * Mathf.Deg2Rad;  // convertir a radianes tras offset
            // Calcular punto en el per�metro de la diana
            float x = Mathf.Sin(anguloBorde) * radio;
            float y = Mathf.Cos(anguloBorde) * radio;
            Vector3 puntoBorde = centro + new Vector3(x, y, 0f);
            Gizmos.DrawLine(centro, puntoBorde);
        }

        // **2. Resaltar el sector del impacto con color diferente**
        if (ultimoImpacto != null)
        {
            // Calcular �ndice de sector del dardo usando el anguloDardo
            int indiceSector = Mathf.FloorToInt(((anguloDardo + halfSector) % 360f) / anguloSector);
            Gizmos.color = colorSectorImpacto;
            // Calcular �ngulos de los dos bordes de ese sector (inferior y superior)
            float anguloInicio = (indiceSector * anguloSector) - halfSector;
            float anguloFin = anguloInicio + anguloSector;
            // Mismos c�lculos de puntos de borde:
            anguloInicio = (anguloInicio - 90f) * Mathf.Deg2Rad;
            anguloFin = (anguloFin - 90f) * Mathf.Deg2Rad;
            Vector3 puntoInicio = centro + new Vector3(Mathf.Sin(anguloInicio) * radio, Mathf.Cos(anguloInicio) * radio, 0f);
            Vector3 puntoFin = centro + new Vector3(Mathf.Sin(anguloFin) * radio, Mathf.Cos(anguloFin) * radio, 0f);
            Gizmos.DrawLine(centro, puntoInicio);
            Gizmos.DrawLine(centro, puntoFin);
        }

        // **3. Dibujar la l�nea del �ngulo de impacto**
        if (ultimoImpacto != null)
        {
            Gizmos.color = colorAngulo;
            Gizmos.DrawLine(centro, ultimoImpacto.position);
            // (Opcional) dibujar un peque�o punto en el impacto
            Gizmos.DrawSphere(ultimoImpacto.position, 0.02f);
#if UNITY_EDITOR
            // (Opcional) Mostrar etiqueta con grados y sector
            Handles.Label(ultimoImpacto.position + Vector3.up * 0.1f, $"{anguloDardo:F1}�");
#endif
        }
    }
}
