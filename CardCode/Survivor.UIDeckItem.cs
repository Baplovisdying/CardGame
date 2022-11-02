// Survivor.UIDeckItem
using Survivor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDeckItem : MonoBehaviour
{
	[SerializeField]
	public UICardItem cardItem;

	[SerializeField]
	public Image lockImg;

	[SerializeField]
	public GameObject priceObj;

	[SerializeField]
	public TextMeshProUGUI priceNum;
}
