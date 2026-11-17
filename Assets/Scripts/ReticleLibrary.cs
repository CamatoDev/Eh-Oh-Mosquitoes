using UnityEngine;

/// <summary>
/// Bibliothèque centralisée des réticules de visée pour Eh-Oh-Mosquitoes.
/// Placez vos propres sprites dans Assets/Resources/ReticleLibrary.asset :
/// ils seront automatiquement utilisés dans le Panel Paramètres (vignettes + aperçu)
/// ET sur le viseur central (Focus) dans les scènes de jeu.
/// </summary>
[CreateAssetMenu(fileName = "ReticleLibrary", menuName = "Eh-Oh-Mosquitoes/Reticle Library")]
public class ReticleLibrary : ScriptableObject
{
    [Header("Sprites des Réticules (0=Classique, 1=Ndop Guerrier, 2=Précision)")]
    public Sprite[] reticleSprites = new Sprite[3];

    [Header("Échelle Optionnelle par Réticule (1.0 = Taille Normale)")]
    public float[] scaleMultipliers = new float[] { 1f, 1f, 1f };

    public Sprite GetSprite(int index)
    {
        if (reticleSprites == null || reticleSprites.Length == 0) return null;
        int safeIdx = Mathf.Clamp(index, 0, reticleSprites.Length - 1);
        return reticleSprites[safeIdx];
    }

    public float GetScaleMultiplier(int index)
    {
        if (scaleMultipliers == null || scaleMultipliers.Length == 0) return 1f;
        int safeIdx = Mathf.Clamp(index, 0, scaleMultipliers.Length - 1);
        float mult = scaleMultipliers[safeIdx];
        return mult > 0.05f ? mult : 1f;
    }
}
