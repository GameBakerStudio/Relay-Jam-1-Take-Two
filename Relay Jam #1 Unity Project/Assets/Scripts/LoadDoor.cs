using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDoor : MonoBehaviour
{
    [SerializeField] [TextArea] string _notes;
    [SerializeField] private string _destinationScene;

    private bool _busy = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_busy && collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D))
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
