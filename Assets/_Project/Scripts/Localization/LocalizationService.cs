using UnityEngine;

namespace ClayForge.Localization
{
    public enum LocaleId
    {
        English = 0,
        Russian = 1
    }

    public sealed class LocalizationService : MonoBehaviour
    {
        public static LocalizationService Instance { get; private set; }
        public static bool IsRussian => Instance != null && Instance.Locale == LocaleId.Russian;

        public LocaleId Locale { get; private set; } = LocaleId.Russian;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Toggle() => Locale = Locale == LocaleId.Russian ? LocaleId.English : LocaleId.Russian;

        public static string T(string key)
        {
            var ru = Instance == null || Instance.Locale == LocaleId.Russian;
            return key switch
            {
                "play" => ru ? "Мастерская" : "Workshop",
                "gallery" => ru ? "Галерея" : "Gallery",
                "quit" => ru ? "Выход" : "Quit",
                "back" => ru ? "Назад" : "Back",
                "title" => ru ? "Гончарный круг" : "ClayForge",
                "subtitle" => ru ? "Прототип симулятора" : "Pottery prototype",
                "wheel" => ru ? "Круг" : "Wheel",
                "centering" => ru ? "Центр" : "Center",
                "moisture" => ru ? "Влажность" : "Moisture",
                "dry" => ru ? "Сушка" : "Dry",
                "fire" => ru ? "Обжиг" : "Fire",
                "glaze" => ru ? "Глазурь" : "Glaze",
                "reset" => ru ? "Новый ком" : "New lump",
                "lang" => ru ? "EN" : "RU",
                "hints" => ru
                    ? "ЛКМ/ПКМ — руки   Мышь — обход кома   WASD — сдвиг   Shift/Ctrl — сила/тонко   Колесо — обороты   Пробел — стоп   F — вода   C — срез   V — камера"
                    : "LMB/RMB hands   Mouse orbit   WASD nudge   Shift/Ctrl force   Scroll RPM   Space stop   F water   C cut   V camera",
                "galleryEmpty" => ru ? "Здесь появятся сохранённые работы." : "Saved pieces will appear here.",
                "Fixed" => ru ? "Камера: мастерская" : "Camera: workshop",
                "Orbit" => ru ? "Камера: вокруг" : "Camera: orbit",
                "Hands" => ru ? "Камера: руки" : "Camera: hands",
                _ => key
            };
        }
    }
}
