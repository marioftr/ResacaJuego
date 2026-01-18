using System;
using DG.Tweening;
using UnityEngine;

public class Flauta : MonoBehaviour
{
    public static Flauta Instancia { get; private set; }

    [SerializeField] private ParticleSystem _Particulas;

    private Transform _Transform;
    private float _Duracion = 0.2f;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        
        _Transform = transform;
        _Particulas.Stop();
    }

    public void IniciarBucle()
    {
        _Transform.DOKill();
        _Transform.DOScale(_Transform.localScale*1.1f, _Duracion)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        
        _Particulas.Play();
    }
    public void DetenerMovimiento()
    {
        _Transform.DOKill();
        _Transform.DOScale(5, 0.3f);
        
        _Particulas.Stop();
    }
}
