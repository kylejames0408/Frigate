using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationUI : MonoBehaviour
{
    // Content
    [SerializeField] private string buildingBuilt = "Do you really want to demolish this building? Only 50% of the cost will be refunded.";
    [SerializeField] private string buildingUnbuilt = "Do you really want to demolish this building? 100% of the cost will be refunded.";
    // Components
    [SerializeField] private TextMeshProUGUI tmpContent;
    public Button btnApprove;
    public Button btnDecline;

    internal void SetDemolishState(bool isBuilt)
    {
        if(isBuilt)
        {
            tmpContent.text = buildingBuilt;
        }
        else
        {
            tmpContent.text = buildingUnbuilt;
        }
    }
}
