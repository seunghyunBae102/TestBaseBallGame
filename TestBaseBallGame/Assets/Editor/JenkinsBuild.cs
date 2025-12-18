using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting; // ★ 이 줄이 빠져서 에러가 난 것입니다.

// 이 클래스는 반드시 "Assets/Editor" 폴더 안에 있어야 합니다.
public class JenkinsBuild
{
    // 젠킨스가 호출할 함수 (static이어야 함)
    public static void PerformBuild()
    {
        // 1. 빌드할 씬(Scene) 목록 확보
        string[] scenes = { "Assets/Scenes/Main.unity", "Assets/Scenes/Game.unity" };

        // 2. 결과물이 저장될 경로와 파일명 설정
        // (젠킨스 workspace 내의 Builds 폴더에 저장하도록 설정)
        string buildPath = "Builds/MyGame.exe"; // 안드로이드는 .apk

        // 3. 빌드 옵션 설정
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64; // 안드로이드는 BuildTarget.Android
        buildPlayerOptions.options = BuildOptions.None;

        // 4. ★실제 빌드 실행★ (이게 실행되어야 파일이 나옵니다!)
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // 5. 결과 로그 출력 (젠킨스 콘솔에서 확인용)
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build Succeeded: " + buildPath);
        }
        else
        {
            Debug.LogError("Build Failed");
        }
    }
}