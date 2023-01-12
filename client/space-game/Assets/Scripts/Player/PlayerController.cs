using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Character _character;

    private void Awake()
    {
        if (_character == null) {
            _character = GetComponent<Character>();
        }
    }
}