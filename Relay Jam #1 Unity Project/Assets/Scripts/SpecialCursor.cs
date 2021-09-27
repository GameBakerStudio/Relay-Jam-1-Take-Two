using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCursor : MonoBehaviour
{
	public RectTransform myRectTransform;
	private Camera mainCamera;

	public Vector3 canvasPointerOffset;

	public Transform worldCursor;
	public Vector3 worldCursorOffset;

	// Start is called before the first frame update
	void Start()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
	}


	// Update is called once per frame
	void LateUpdate()
	{
		// refresh position of special cursor
		Vector3 mousePosition = Input.mousePosition;
		myRectTransform.anchoredPosition = mousePosition + canvasPointerOffset;

		worldCursor.gameObject.SetActive(Input.GetButton("Fire1"));

		worldCursor.position = mainCamera.ScreenToWorldPoint(mousePosition) + worldCursorOffset;

		// turn on/off the real cursor. Real cursor is on when out of screen.
		Cursor.visible = mousePosition.x <= 0.0f || mousePosition.x >= mainCamera.pixelWidth ||
			mousePosition.y <= 0.0f || mousePosition.y >= mainCamera.pixelHeight;
	}
}
