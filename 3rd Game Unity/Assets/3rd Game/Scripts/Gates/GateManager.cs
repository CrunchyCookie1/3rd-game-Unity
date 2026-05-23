using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GateManager : MonoBehaviour
{
    public GameObject gate;
    public GameObject gateNumberObj;
    public TextMeshPro gateNumber;

    public float gateNumberValue;

    private void Start()
    {
        if (gateNumberObj != null)
            gateNumber.text = gateNumberValue.ToString();
    }

    private void Update()
    {
        if (gateNumberValue < 0)
        {
            gateNumberValue = 0;
        }
        if (gateNumberValue == 0)
        {
            saveGateNumber();
            gate.SetActive(false);
            if (gateNumberObj != null)
                gateNumberObj.SetActive(false);
        }
    }

    public void decreaseGateNumber()
    {
        gateNumberValue--;
        if (gateNumber != null)
            gateNumber.text = gateNumberValue.ToString();
    }

    public void saveGateNumber()
    {
        PlayerPrefs.SetFloat(gameObject.name + "GateNumber", gateNumberValue);
    }

    public void loadSave()
    {
        if (PlayerPrefs.HasKey(gameObject.name + "GateNumber"))
        {
            gateNumberValue = PlayerPrefs.GetFloat(gameObject.name + "GateNumber");
        }
    }
}
