using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private Image panelImage;
    
    private bool disappering = false;

    public void SetText(string newText)
    {
        textMesh.text = newText;
    }

    private void Start()
    {
        panelImage = GetComponent<Image>();
        StartCoroutine(Disappear());
    }

    private void Update()
    {
        if (!disappering) return;

        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, Mathf.Lerp(textMesh.color.a, 0f, Time.deltaTime));
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, Mathf.Lerp(panelImage.color.a, 0f, Time.deltaTime));
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(3f);
        disappering = true;
        Destroy(gameObject, 2f);
    }
}
