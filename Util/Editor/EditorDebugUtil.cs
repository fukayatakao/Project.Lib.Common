using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Cherry {
	public class EditorDebugUtil {
		const string BattleScenePath = "Assets/Application/Scene/Game/Battle.unity";
		const string TownScenePath = "Assets/Application/Scene/Game/Town.unity";
		const string TitleScenePath = "Assets/Application/Scene/Game/Title.unity";
		const string OrganizeScenePath = "Assets/Application/Scene/Game/Organize.unity";
		const string StrategyScenePath = "Assets/Application/Scene/Game/Strategy.unity";

		//battleシーンをメニューからオープン
		[MenuItem("Editor/Scene/Battle", false, 98)]
		public static void BootBattle() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BattleScenePath);
		}

		//townシーンをメニューからオープン
		[MenuItem("Editor/Scene/Town", false, 98)]
		public static void BootTown() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TownScenePath);
		}

		//titleシーンをメニューからオープン
		[MenuItem("Editor/Scene/Title", false, 98)]
		public static void BootTitle() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TitleScenePath);
		}

		//Organizeシーンをメニューからオープン
		[MenuItem("Editor/Scene/Organize", false, 98)]
		public static void BootOrganize() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(OrganizeScenePath);
		}

		//Strategyシーンをメニューからオープン
		[MenuItem("Editor/Scene/Strategy", false, 98)]
		public static void BootStrategy() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(StrategyScenePath);
		}


		//ReverseAnimationシーンをメニューからオープン
		[MenuItem("Editor/Scene/Sandbox/ReverseAnimation", false, 98)]
		public static void BootReverseAnimation()
		{
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Application/Scene/Sandbox/ReverseAnimation.unity");
		}

		//WoodyDummyシーンをメニューからオープン
		[MenuItem("Editor/Scene/Sandbox/WoodyDummy", false, 98)]
		public static void BootWoodyDummy()
		{
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Application/Scene/Sandbox/WoodyDummy.unity");
		}

		//ゲームスピードを遅くする
		[MenuItem("Editor/Time/Time Slow %u", false, 98)]
		public static void TimeScaleDown() {
			Time.timeScale *= 0.75f;
			if (Time.timeScale < 0.1f) {
				Time.timeScale = 0.1f;
			}
		}


		//ゲームスピードをリセットする
		[MenuItem("Editor/Time/Time Reset %i", false, 98)]
		public static void TimeScaleReset() {
			Time.timeScale = 1f;
		}



	}
}
