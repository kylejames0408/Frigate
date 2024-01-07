using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button mainMenuBtn;

    private void Start()
    {
        resumeBtn.onClick.AddListener(CloseInterface);
        mainMenuBtn.onClick.AddListener(GameManager.ToMainMenu);

        CloseInterface();
    }

    public void OpenInterface()
    {
        gameObject.SetActive(true);
    }
    public void CloseInterface()
    {
        gameObject.SetActive(false);
    }
}
