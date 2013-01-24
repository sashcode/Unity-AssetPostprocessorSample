using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Processer : AssetPostprocessor
{
	//Assetフォルダ内のファイルに変更があった場合に呼び出される
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string str in importedAssets) {
			string fileName = Path.GetFileName (str);
			string dirName = Path.GetDirectoryName (str);
			
			//データ定義ファイルが変更された場合
			if (fileName == ("data.txt")) {
				
				//データに対応するアセットを取得
				string dataAssetPath = dirName + "/data.asset";
				Data data = (Data)AssetDatabase.LoadAssetAtPath (dataAssetPath, typeof(Data));
				if (data == null) {
					//取得できない場合は新規に作成
					data = ScriptableObject.CreateInstance<Data> ();
					AssetDatabase.CreateAsset (data, dataAssetPath);
				}
				
				//既存のデータを格納
				Dictionary<string , WallData> existsData = new Dictionary <string , WallData> ();
				if (data.walls != null) {
					foreach (WallData wd in data.walls) {
						existsData [wd.id] = wd;
					}
				}
				
				//データをテキストとして読み込む
				TextAsset t = (TextAsset)Resources.LoadAssetAtPath (str, typeof(TextAsset));
				//JSON形式を解析
				JSONObject root = new JSONObject (t.text);
				JSONObject walls = root ["walls"];
				foreach (JSONObject wall in walls.list) {
					JSONObject id = wall ["id"];
					JSONObject name = wall ["name"];
					JSONObject color = wall ["color"];
					JSONObject position = wall ["position"];
					JSONObject size = wall ["size"];
					Debug.Log (" id = " + id.str);
					Debug.Log (" name = " + name.str);
					Debug.Log (" color = " + color [0].n + " " + color [1].n + " " + color [2].n);
					Debug.Log (" position = " + position [0].n + " " + position [1].n + " " + position [2].n);
					Debug.Log (" size = " + size [0].n + " " + size [1].n + " " + size [2].n);
					
					WallData wallData = null;
					
					if (existsData.ContainsKey (id.str)) {
						//すでに存在する場合
						wallData = existsData [id.str];
						existsData.Remove (id.str);
					} else {
						//新規に追加された場合
						wallData = ScriptableObject.CreateInstance<WallData> ();
						data.walls.Add (wallData);
						AssetDatabase.AddObjectToAsset (wallData, dataAssetPath);
					}
					
					//オブジェクトの内容を更新する
					wallData.id = id.str;
					wallData.name = name.str;
					wallData.color = new Color ((float)color [0].n, (float)color [1].n, (float)color [2].n);
					wallData.position = new Vector3 ((float)position [0].n, (float)position [1].n, (float)position [2].n);
					wallData.size = new Vector3 ((float)size [0].n, (float)size [1].n, (float)size [2].n);
				}
				
				//存在していたものでデータから削除されたものはアセットからも削除する
				foreach (WallData wd in existsData.Values) {
					data.walls.Remove (wd);
					Object.DestroyImmediate (wd, true);
				}
				
				AssetDatabase.SaveAssets ();
			}
		}
		
		
	}
}
