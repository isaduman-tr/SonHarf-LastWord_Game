using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelsManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject offlineRandomAvatarPanel;
    public GameObject offlineChatPanel;
    public GameObject onlineCoinBetPanel;
    public GameObject onlineRandomAvatarPanel;
    public GameObject onlineChatPanel;

    public Button offlineButton;
    public Button offlineRandomToChatButton;
    public Button offlineChatBackButton;
    public Button onlineButton;
    public Button onlineCoinBetToRandomButton;
    public Button onlineRandomToChatButton;
    public Button offlineBackButton;
    public Button onlineBackButton;
    public Button onlineRandomBackButton;
    public Button onlineChatBackButton;

    public Animator offlineAnimator;
    public Animator onlineAnimator;
    public Animator offlineChatAnimator;
    public Animator onlineRandomAnimator;
    public Animator onlineChatAnimator;
    public Animator mainPanelAnimator;

    private bool isAnimating = false;

    private void Start()
    {
        offlineButton.onClick.AddListener(() => ShowPanel(offlineRandomAvatarPanel, offlineAnimator, "CardAnimation"));
        onlineButton.onClick.AddListener(() => ShowPanel(onlineCoinBetPanel, onlineAnimator, "OnlineCardAnimation"));
        onlineCoinBetToRandomButton.onClick.AddListener(() => ShowPanel(onlineRandomAvatarPanel, onlineRandomAnimator, "OnlineCardAnimation"));
        onlineRandomToChatButton.onClick.AddListener(() => ShowPanel(onlineChatPanel, onlineChatAnimator, "OnlineCardAnimation"));
        offlineRandomToChatButton.onClick.AddListener(() => ShowPanel(offlineChatPanel, offlineChatAnimator, "CardAnimation"));

        offlineBackButton.onClick.AddListener(() => HidePanel(offlineRandomAvatarPanel, offlineAnimator, "TersCardAnimation", mainPanel));
        onlineBackButton.onClick.AddListener(() => HidePanel(onlineCoinBetPanel, onlineAnimator, "OnlineTersCardAnimation", mainPanel));
        onlineRandomBackButton.onClick.AddListener(() => HidePanel(onlineRandomAvatarPanel, onlineRandomAnimator, "OnlineTersCardAnimation", onlineCoinBetPanel));
        onlineChatBackButton.onClick.AddListener(() =>
        {
            HidePanel(onlineChatPanel, onlineAnimator, "OnlineTersCardAnimation", mainPanel);
            StartCoroutine(ResetAnimatorsAndDeactivatePanels(0.5f));
        });

        offlineChatBackButton.onClick.AddListener(() =>
        {
            HidePanel(offlineChatPanel, offlineAnimator, "TersCardAnimation", mainPanel);
            StartCoroutine(ResetAnimatorsAndDeactivatePanels(0.5f));
        });
    }

    private void ShowPanel(GameObject panel, Animator animator, string animationName)
    {
        if (isAnimating) return;
        isAnimating = true;

        panel.SetActive(true);
        animator.Play(animationName);

        StartCoroutine(ResetAnimationState(0.5f));
    }

    private void HidePanel(GameObject panel, Animator animator, string animationName, GameObject panelToShow)
    {
        if (isAnimating) return;
        isAnimating = true;

        animator.Play(animationName);

        StartCoroutine(SwitchPanel(panel, panelToShow, 0.5f));
    }

    private IEnumerator ResetAnimationState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAnimating = false;
    }

    private IEnumerator SwitchPanel(GameObject panelToHide, GameObject panelToShow, float delay)
    {
        yield return new WaitForSeconds(delay);
        panelToHide.SetActive(false);
        panelToShow.SetActive(true);
        isAnimating = false;
    }

    private IEnumerator ResetAnimatorsAndDeactivatePanels(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Panellerin aktiflik kontrol, değilse aktif
        if (!offlineRandomAvatarPanel.activeSelf) offlineRandomAvatarPanel.SetActive(true);
        if (!onlineCoinBetPanel.activeSelf) onlineCoinBetPanel.SetActive(true);
        if (!onlineRandomAvatarPanel.activeSelf) onlineRandomAvatarPanel.SetActive(true);
        if (!onlineChatPanel.activeSelf) onlineChatPanel.SetActive(true);
        if (!offlineChatPanel.activeSelf) offlineChatPanel.SetActive(true);

        // Animatorleri aktif paneller üzerinde sıfırlama
        if (offlineRandomAvatarPanel.activeSelf)
        {
            offlineAnimator.Rebind();
            offlineAnimator.Update(0);
        }

        if (onlineCoinBetPanel.activeSelf)
        {
            onlineAnimator.Rebind();
            onlineAnimator.Update(0);
        }

        if (onlineRandomAvatarPanel.activeSelf)
        {
            onlineRandomAnimator.Rebind();
            onlineRandomAnimator.Update(0);
        }

        if (onlineChatPanel.activeSelf)
        {
            onlineChatAnimator.Rebind();
            onlineChatAnimator.Update(0);
        }

        if (offlineChatPanel.activeSelf)
        {
            offlineChatAnimator.Rebind();
            offlineChatAnimator.Update(0);
        }

        offlineRandomAvatarPanel.SetActive(false);
        onlineCoinBetPanel.SetActive(false);
        onlineRandomAvatarPanel.SetActive(false);
        onlineChatPanel.SetActive(false);
        offlineChatPanel.SetActive(false);
    }
}