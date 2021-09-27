using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDoor : MonoBehaviour
{
	[SerializeField] [TextArea] string _notes;
	[SerializeField] private string _destinationScene;

	private bool _busy = false;

	public bool requiresKey = true;
	public AudioClip keyNeededSound;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Player player = collision.gameObject.GetComponent<Player>();
		if (player == null) return;

		if (!_busy)
		{

			if (requiresKey)
			{
				if (Player.hasKeys)
				{
					player.UseKey();
				}
				// needs key but have no key!!
				else
				{
					App.PlayAudio(keyNeededSound, transform.position);
					return;
				}
			}


			if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D))
			{
				rb2D.velocity = Vector3.zero;
				rb2D.isKinematic = true;
			}

			_busy = true;
			App.acceptingMoveInput = false;
			var operation = SceneManager.LoadSceneAsync(_destinationScene);
			operation.allowSceneActivation = false;
			App.DoSceneTransition(operation);
		}
	}
}
