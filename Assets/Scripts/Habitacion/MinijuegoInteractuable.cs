using System.Collections;
using TMPro;
using UnityEngine;

public abstract class MinijuegoInteractuable : MonoBehaviour, IInteractuable, IInfoDisponible, IJugable
{
    protected SistemasPersonaje _Personaje;
    
    [SerializeField] protected GameObject _InfoObjeto;
    [SerializeField] protected GameObject _InfoJugar;
    
    [SerializeField] protected string _EfectoOn;
    [SerializeField] protected string _EfectoOff;
    
    [SerializeField] protected int _NumeroEscenaMinijuego;
    
    [SerializeField] protected TMP_Text _TextoFrase;
    protected int _IndiceTextoAnterior;

    protected bool _ObjetoOn;

    private void Awake()
    {
        _Personaje = FindAnyObjectByType<SistemasPersonaje>();
        _InfoObjeto.SetActive(false);
        _InfoJugar.SetActive(false);
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
        _InfoJugar.SetActive(true);
        CambiarCapa(gameObject, 3);
    }
    public void AlOcultarInfo()
    {
        _InfoObjeto.SetActive(false);
        _InfoJugar.SetActive(false);
        CambiarCapa(gameObject, 0);
    }
    private void CambiarCapa(GameObject objeto, int capa)
    {
        objeto.layer = capa;
        foreach (Transform hijo in objeto.transform)
        {
            CambiarCapa(hijo.gameObject, capa);
        }
    }
    public void AlJugar()
    {
        if (!GestorBase.Instancia.EnModoHistoria)
        {
            GestorBase.Instancia.EstablecerOrigenModoHistoria();
        }
        GestorBase.Instancia.GuardarPosicionPersonaje();
        switch (_NumeroEscenaMinijuego)
        {
            case 3:
                GestorJuego.JugarDardos();
                break;
            case 4:
                GestorJuego.CargarEscena(_NumeroEscenaMinijuego);
                break;
            default:
                print($"{_NumeroEscenaMinijuego} es un número de escena erróneo");
                break;
        }
    }
}
