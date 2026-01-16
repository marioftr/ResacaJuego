using System.Collections;
using TMPro;
using UnityEngine;

public abstract class ObjetoInteractuable : MonoBehaviour, IInteractuable, IInfoDisponible
{
    [SerializeField] protected GameObject _InfoObjeto;
    [SerializeField] protected string _EfectoOn;
    [SerializeField] protected string _EfectoOff;
    
    [SerializeField] protected TMP_Text _TextoFrase;
    protected int _IndiceTextoAnterior;

    protected bool _ObjetoOn;

    private void Awake()
    {
        _InfoObjeto.SetActive(false);
        _TextoFrase.text = "";
    }
    
    public void AlInteractuar()
    {
        if (!_ObjetoOn)
        {
            ObjetoOn();
        }
        else
        {
            ObjetoOff();
        }
    }
    
    protected abstract void ObjetoOn();
    protected abstract void ObjetoOff();
    protected abstract IEnumerator Texto();

    public void AlVerInfo()
    {
        _InfoObjeto.SetActive(true);
        gameObject.layer = 3;
    }

    public void AlOcultarInfo()
    {
        _InfoObjeto.SetActive(false);
        gameObject.layer = 0;
    }
}
