using UnityEngine;
using UnityEngine.UI;

public class TerrainUIController : MonoBehaviour
{
    public TerrainGenerator terrainGen;
    public Slider noiseScaleSlider;
    public Slider heightMultiplierSlider;
    public Slider seedSlider;

    private void Start()
    {
        noiseScaleSlider.value = terrainGen.noiseScale;
        heightMultiplierSlider.value = terrainGen.heightMultiplier;
        seedSlider.value = terrainGen.seed;

        noiseScaleSlider.onValueChanged.AddListener(UpdateTerrain);
        heightMultiplierSlider.onValueChanged.AddListener(UpdateTerrain);
        seedSlider.onValueChanged.AddListener(UpdateTerrain);
    }

    public void UpdateTerrain(float _)
    {
        terrainGen.noiseScale = noiseScaleSlider.value;
        terrainGen.heightMultiplier = heightMultiplierSlider.value;
        terrainGen.seed = Mathf.RoundToInt(seedSlider.value);

        terrainGen.GenerateTerrain();
    }

    public void RegenerateTerrain()
    {
        terrainGen.seed = Random.Range(0, 99999);
        terrainGen.GenerateTerrain();
    }
}
