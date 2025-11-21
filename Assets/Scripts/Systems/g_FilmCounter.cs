using UnityEngine;
using TMPro;

public class g_FilmCounter : MonoBehaviour
{
    public int currentFilms = 12;
    public int maxFilms = 12;
    public TextMeshProUGUI filmCountText;

    void Start()
    {
        UpdateUI();
    }

    public bool HasFilm()
    {
        return currentFilms > 0;
    }

    public void UseFilm()
    {
        if (currentFilms > 0)
        {
            currentFilms--;
            UpdateUI();
        }
    }

    public void AddFilm(int amount)
    {
        currentFilms += amount;
        if (currentFilms > maxFilms)
        {
            currentFilms = maxFilms;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (filmCountText != null)
        {
            filmCountText.text = currentFilms + "x / " + maxFilms;
        }
    }
}
