using UnityEngine;

public static class Bootstrapper {
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
public static void Execute() => Object.DontDestroyOnLoad(Object.Instantiate(ResourceLoader.LoadResource<Object>("Systems")));
}
