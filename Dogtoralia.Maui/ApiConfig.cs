namespace Dogtoralia.Maui
{
    public static class ApiConfig
    {
        // The Android emulator reaches the host machine's localhost via 10.0.2.2.
        // iOS simulator, Mac Catalyst and Windows use localhost directly.
        public static string BaseUrl =>
#if ANDROID
            "http://10.0.2.2:5186";
#else
            "http://localhost:5186";
#endif
    }
}
