using UnityEngine;

public class PixelArtResolutionManager : MonoBehaviour
{
    public int targetWidth = 1024;
    public int targetHeight = 1344;

    void Start()
    {
        // Для ПК: устанавливаем окно. 
        // false = оконный режим. Игрок сможет растянуть его, 
        // а Pixel Perfect Camera корректно масштабирует картинку без мыла.
        Screen.SetResolution(targetWidth, targetHeight, false);
        
        // Фиксируем соотношение сторон для камеры на всякий случай
        if (Camera.main != null)
        {
            Camera.main.aspect = (float)targetWidth / targetHeight;
        }
    }
}