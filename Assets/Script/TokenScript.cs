using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TokenScript : MonoBehaviour, IPointerDownHandler
{
	Image tokenImg;
	public Sprite[] images;
	public int m_IndexNumber;
	public bool isFilled = false;
	public GameObject NetworkClient;

	private void Awake()
	{
		tokenImg = GetComponent<Image>();
		m_IndexNumber = transform.GetSiblingIndex();
        NetworkClient = GameObject.Find("NetworkedClient");

	}




	public void OnPointerDown(PointerEventData eventData)
	{
		if (!isFilled)
		{
			string msg;
			msg = ClientToServerSignifier.SendChoosenToken + "," + m_IndexNumber;
			NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

		}
	}

	public void SetToken(int index)
	{
		
		tokenImg.sprite = images[index];
		isFilled = true;
	}
}
