using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TabToySpace
{
    public class ExcelReaderEditorWindow : EditorWindow
    {
        private const string VERSION = "1.0.0";
        
        [Serializable]
        public class ExcelReaderPrefs
        {
            public string prefsName;
            public string combineClassName;//组合类名
            public List<string> InputPaths = new List<string>();
            public string CodeOutputPath;
            public string DataOutputPath;

            public bool ExportJson;
            public bool ExportLua;

            public bool ExportPartialClass = true;

            public bool ShareClass = false;

            public string PartialCodeOutputPath
            {
                get { return CodeOutputPath.Replace(combineClassName, combineClassName + "Partial"); }
            }

            public bool DataPerfect
            {
                get { return InputPaths.Count > 0 && !string.IsNullOrEmpty(CodeOutputPath) && !string.IsNullOrEmpty(DataOutputPath); }
            }

            public List<string> ShareClassDataOutputPaths()
            {
                List<string> paths = new List<string>();
                foreach (var inputPath in InputPaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(inputPath);
                    paths.Add(Path.Combine(DataOutputPath, fileName + ".bytes"));
                }
                return paths;
            }
            
            public static string OptionalDataOutputPath(string oriOutputPath, string suffix)
            {
                return oriOutputPath.Replace(".bytes", string.Format("_{0}.{1}", suffix, suffix));
            }
        }

        [Serializable]
        public class ExcelReaderPrefsFile
        {
            public List<ExcelReaderPrefs> PrefsList = new List<ExcelReaderPrefs>();
            public string CacheInputDirectory;
            public string CacheCodeOutputDirectory;
            public string CacheDataOutputDirectory;

            public string Version;
        }

        private string EXCELREADER_PATH;
        private string SAVE_PATH;
        private string SHELL_PATH;
        
        private string DEFAULT_ROOT_PATH
        {
            get { return Application.dataPath; }
        }

        private string InputDirectory
        {
            get
            {
                return !string.IsNullOrEmpty(mPrefsFile.CacheInputDirectory)
                    ? Path.GetFullPath(mPrefsFile.CacheInputDirectory)
                    : DEFAULT_ROOT_PATH;
            }
            set { mPrefsFile.CacheInputDirectory = value; }
        }

        private string CodeOutputDirectory
        {
            get
            {
                return !string.IsNullOrEmpty(mPrefsFile.CacheCodeOutputDirectory)
                    ? Path.GetFullPath(mPrefsFile.CacheCodeOutputDirectory)
                    : DEFAULT_ROOT_PATH;
            }
            set { mPrefsFile.CacheCodeOutputDirectory = value; }
        }

        private string DataOutputDirectory
        {
            get
            {
                return !string.IsNullOrEmpty(mPrefsFile.CacheDataOutputDirectory)
                    ? Path.GetFullPath(mPrefsFile.CacheDataOutputDirectory)
                    : DEFAULT_ROOT_PATH;
            }
            set { mPrefsFile.CacheDataOutputDirectory = value; }
        }

        private ExcelReaderPrefsFile mPrefsFile;
        private List<ExcelReaderPrefs> mPrefsDatas { get { return mPrefsFile.PrefsList; } }
        private List<string> mPrefsNames;
        private ExcelReaderPrefs mSelectedPrefs;
        private int mSelectedPrefsIndex = -1;
        private bool mIsDirty = false;


        private Vector2 mScrollViewPos;

        private string outputStr = null;

        [MenuItem("Tabtoy/Open Edit Window")]
        static void ShowExcelReaderWindow()
        {
            ExcelReaderEditorWindow window = (ExcelReaderEditorWindow) EditorWindow.GetWindow(typeof(ExcelReaderEditorWindow), true);
            window.Show();
        }

        void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            path = Path.GetDirectoryName(path);
            EXCELREADER_PATH = Path.GetDirectoryName(path);
            if(System.Environment.OSVersion.Platform == PlatformID.Unix)
            {
                SHELL_PATH = Path.Combine(path, "generate_configs_bin_mac.sh");
            }
            else
            {
               SHELL_PATH = Path.Combine(path, "generate_configs_bin_window.bat");
            }
            SAVE_PATH = Path.Combine(path, "SaveInfo.json");
            
            mPrefsNames = new List<string>();
            mSelectedPrefs = null;
            mSelectedPrefsIndex = -1;

            if (File.Exists(SAVE_PATH))
            {
                mPrefsFile = (ExcelReaderPrefsFile) JsonUtility.FromJson(System.IO.File.ReadAllText(SAVE_PATH), typeof(ExcelReaderPrefsFile));
                foreach (ExcelReaderPrefs prefs in mPrefsFile.PrefsList)
                {
                    if (string.IsNullOrEmpty(prefs.prefsName))
                        prefs.prefsName = prefs.combineClassName;
                    mPrefsNames.Add(prefs.prefsName);
                }
            }
            else
            {
                mPrefsFile = new ExcelReaderPrefsFile();
            }

            mPrefsFile.Version = VERSION;
        }

        void SavePrefs()
        {
            System.IO.File.WriteAllText(SAVE_PATH, JsonUtility.ToJson(mPrefsFile, true));
        }

        void OnGUI()
        {
            mIsDirty = false;

            mScrollViewPos = GUILayout.BeginScrollView(mScrollViewPos);

            GUILayout.Label("Excel Reader", EditorStyles.boldLabel);

            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("根路径", GUILayout.Width(120));
            GUILayout.Box(Path.GetFullPath(DEFAULT_ROOT_PATH + "/../"));
            GUILayout.EndHorizontal();
            GUILayout.Label("*建议xlsx放在此路径下的文件夹内");

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            mSelectedPrefsIndex = EditorGUILayout.Popup("选择一组配置表", mSelectedPrefsIndex, mPrefsNames.ToArray(), GUILayout.Width(300));
            if (mSelectedPrefsIndex >= 0 && mPrefsDatas.Count > mSelectedPrefsIndex)
            {
                mSelectedPrefs = mPrefsDatas[mSelectedPrefsIndex];
            }

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                mSelectedPrefs = new ExcelReaderPrefs();
                mSelectedPrefs.combineClassName = "NewConfig" + mPrefsDatas.Count;
                mSelectedPrefs.prefsName = mSelectedPrefs.combineClassName;
                mPrefsDatas.Add(mSelectedPrefs);
                mPrefsNames.Add(mSelectedPrefs.prefsName);
                mIsDirty = true;
                mSelectedPrefsIndex = mPrefsNames.Count - 1;
            }

            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                if (mSelectedPrefs != null)
                {
                    mPrefsDatas.RemoveAt(mSelectedPrefsIndex);
                    mPrefsNames.RemoveAt(mSelectedPrefsIndex);
                    mSelectedPrefs = null;
                    mIsDirty = true;
                }
            }

            GUILayout.EndHorizontal();

            if (mSelectedPrefs != null)
            {
                EditSelectPrefs();
            }

            //全部导出
            if (GUILayout.Button("全部导出~.~", GUILayout.Width(100)))
            {
                ExportAll();
            }
            
            if (!string.IsNullOrEmpty(outputStr))
            {
                GUILayout.Label("输出：");
                EditorGUILayout.HelpBox(outputStr, MessageType.Info);
            }

            GUILayout.EndScrollView();

            if (mIsDirty)
            {
                SavePrefs();
            }
        }
        
        string AdaptPrefsName(int index, string prefsName)
        {
            Regex rgx = new Regex(@"^" + prefsName + @"-\d*$");
            int i = -1;

            for (int j = 0; j < mPrefsNames.Count; j++)
            {
                if (j == index) continue;
                string s = mPrefsNames[j];
                if (s.Equals(prefsName))
                {
                    if (i == -1)
                        i = 0;
                }
                else if (rgx.IsMatch(s))
                {
                    int suffixid = int.Parse(s.Substring(prefsName.Length + 1));
                    if (suffixid > i) i = suffixid;
                }
            }
            if (i >= 0)
            {
                ++i;
                return prefsName + "-" + i;
            }
            return prefsName;
        }

        void EditSelectPrefs()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            
            string classname = EditorGUILayout.TextField("导出文件名： ", mSelectedPrefs.combineClassName, GUILayout.Width(300));
            if (!string.IsNullOrEmpty(classname) && !classname.Equals(mSelectedPrefs.combineClassName))
            {
                mSelectedPrefs.combineClassName = classname;
                mSelectedPrefs.prefsName = AdaptPrefsName(mSelectedPrefsIndex, classname);
                mPrefsNames[mSelectedPrefsIndex] = mSelectedPrefs.prefsName;
                mIsDirty = true;
            }

            bool shareClass = false; //GUILayout.Toggle(mSelectedPrefs.ShareClass, "是：所有配置表共用该类；否：所有配置表生成的类被该类包裹。", GUILayout.Width(400));
            if (shareClass != mSelectedPrefs.ShareClass)
            {
                mSelectedPrefs.ShareClass = shareClass;
                mSelectedPrefs.DataOutputPath = null;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("选择配置表", GUILayout.Width(120));
            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                string filePath = EditorUtility.OpenFilePanel("Select Excel File", InputDirectory, "xlsx");
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = MakeRelativePath(filePath, DEFAULT_ROOT_PATH);
                    mSelectedPrefs.InputPaths.Add(filePath);
                    InputDirectory = Path.GetDirectoryName(filePath);
                    mIsDirty = true;
                }
            }

            if (mSelectedPrefs.InputPaths.Count > 0)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < mSelectedPrefs.InputPaths.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(mSelectedPrefs.InputPaths[i]);
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        mSelectedPrefs.InputPaths.RemoveAt(i);
                        mIsDirty = true;
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("选择代码输出路径", GUILayout.Width(120));
            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                string filePath = EditorUtility.SaveFilePanel("Select Code Output", CodeOutputDirectory, mSelectedPrefs.combineClassName, "cs");
                if (!string.IsNullOrEmpty(filePath))
                {
                    mSelectedPrefs.CodeOutputPath = MakeRelativePath(filePath, DEFAULT_ROOT_PATH);
                    CodeOutputDirectory = Path.GetDirectoryName(mSelectedPrefs.CodeOutputPath);
                    mIsDirty = true;
                }
            }
            if (!string.IsNullOrEmpty(mSelectedPrefs.CodeOutputPath))
            {
                //make sure file name equals to combine name
                string fileName = Path.GetFileName(mSelectedPrefs.CodeOutputPath);
                string rightFileName = mSelectedPrefs.combineClassName + ".cs";
                if (!fileName.Equals(rightFileName))
                {
                    mSelectedPrefs.CodeOutputPath = mSelectedPrefs.CodeOutputPath.Replace(fileName, rightFileName);
                }
                GUILayout.Box(mSelectedPrefs.CodeOutputPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("选择.bytes输出路径", GUILayout.Width(120));
            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                if (!mSelectedPrefs.ShareClass)
                {
                    string filePath = EditorUtility.SaveFilePanel("Select Data Output", DataOutputDirectory, mSelectedPrefs.combineClassName, "bytes");
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        mSelectedPrefs.DataOutputPath = MakeRelativePath(filePath, DEFAULT_ROOT_PATH);
                        DataOutputDirectory = Path.GetDirectoryName(mSelectedPrefs.DataOutputPath);
                        mIsDirty = true;
                    }
                }
                else
                {
                    string folderPath = EditorUtility.SaveFolderPanel("Select Data Output Folder", DataOutputDirectory, "");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        mSelectedPrefs.DataOutputPath = MakeRelativePath(folderPath, DEFAULT_ROOT_PATH);
                        DataOutputDirectory = Path.GetDirectoryName(mSelectedPrefs.DataOutputPath);
                        mIsDirty = true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(mSelectedPrefs.DataOutputPath))
            {
                GUILayout.Box(mSelectedPrefs.DataOutputPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("另外导出选项:", GUILayout.Width(120));
            bool exportPartial = GUILayout.Toggle(mSelectedPrefs.ExportPartialClass, "partial c# class", GUILayout.Width(120));
            if (exportPartial != mSelectedPrefs.ExportPartialClass)
            {
                mSelectedPrefs.ExportPartialClass = exportPartial;
                mIsDirty = true;
            }
            bool exportJson = GUILayout.Toggle(mSelectedPrefs.ExportJson, "json", GUILayout.Width(60));
            if (exportJson != mSelectedPrefs.ExportJson)
            {
                mSelectedPrefs.ExportJson = exportJson;
                mIsDirty = true;
            }
            bool exportLua = GUILayout.Toggle(mSelectedPrefs.ExportLua, "lua", GUILayout.Width(60));
            if (exportLua != mSelectedPrefs.ExportLua)
            {
                mSelectedPrefs.ExportLua = exportLua;
                mIsDirty = true;
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("导出*.*", GUILayout.Width(100)))
            {
                if (mSelectedPrefs.DataPerfect)
                {
                    outputStr = "";
                    Export(mSelectedPrefs);
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("", "请完善数据！", "OK");
                }
            }

            GUILayout.EndVertical();
        }

        void Export(ExcelReaderPrefs xlsxPrefs)
        {
            Func<string, string, string> buildArgs = (dataOutput, input) =>
            {
                StringBuilder sbArgs = new StringBuilder();

                //0. shell
                sbArgs.Append(SHELL_PATH);
                //1. v3 index name
                string indexPath = Path.GetFileName(xlsxPrefs.InputPaths[0]);
                sbArgs.AppendFormat(" \"{0}\"", indexPath);
                //2. code output
                sbArgs.AppendFormat(" \"{0}\"", Path.GetFullPath(xlsxPrefs.CodeOutputPath));
                //3. data output
                sbArgs.AppendFormat(" \"{0}\"", dataOutput);
                //4. json output
                sbArgs.AppendFormat(" \"{0}\"", xlsxPrefs.ExportJson ? Path.GetFullPath(ExcelReaderPrefs.OptionalDataOutputPath(dataOutput, "json")) : "null");
                //5. lua output
                sbArgs.AppendFormat(" \"{0}\"", xlsxPrefs.ExportLua ? Path.GetFullPath(ExcelReaderPrefs.OptionalDataOutputPath(dataOutput, "lua")) : "null");
                //6. cd index.xlsx pre path
                string fullPath = Path.GetFullPath(xlsxPrefs.InputPaths[0]);
                string dir = Path.GetDirectoryName(fullPath)+"/";
                sbArgs.AppendFormat(" \"{0}\"", dir);
                //7. combine name
                sbArgs.AppendFormat(" \"{0}\"", "Table_"+xlsxPrefs.combineClassName);
                
                //4. xlsx input
                sbArgs.Append(input);

                
                
                return sbArgs.ToString();
            };

            //shell tabtoy
            Action<string> shellProcess = (args) =>
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                if(System.Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    process.StartInfo = new System.Diagnostics.ProcessStartInfo("/bin/bash", args);
                }
                else
                {
                   process.StartInfo = new System.Diagnostics.ProcessStartInfo("bash", args);
                }
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                outputStr += process.StandardOutput.ReadToEnd() + "\n";
                process.WaitForExit();
                process.Close();
            };

            if (!xlsxPrefs.ShareClass)
            {
                StringBuilder inputSb = new StringBuilder();
                for (int i = 0; i < xlsxPrefs.InputPaths.Count; i++)
                {
                    inputSb.AppendFormat(" \"{0}\"", Path.GetFullPath(xlsxPrefs.InputPaths[i]));
                }
                string args = buildArgs(Path.GetFullPath(xlsxPrefs.DataOutputPath), inputSb.ToString());
                //Debug.Log(">>>>>> shell args: " + args);
                shellProcess(args);
            }
            else
            {
                List<string> dataOutputs = xlsxPrefs.ShareClassDataOutputPaths();
                for (int i = 0; i < xlsxPrefs.InputPaths.Count; i++)
                {
                    string args = buildArgs(Path.GetFullPath(dataOutputs[i]), string.Format(" \"{0}\"", Path.GetFullPath(xlsxPrefs.InputPaths[i])));
                    //Debug.Log(">>>>>> shell args: " + args);
                    shellProcess(args);
                }
            }
            
            //export partial class
            if (xlsxPrefs.ExportPartialClass)
            {
                string partialClassPath = Path.GetFullPath(xlsxPrefs.PartialCodeOutputPath);
                if (!File.Exists(partialClassPath))
                {
                    //only export once
                    string str = System.IO.File.ReadAllText(Path.GetFullPath(xlsxPrefs.CodeOutputPath));
                    if (!string.IsNullOrEmpty(str))
                    {
                        string namespaceStr = str.Substring(str.IndexOf("namespace"), str.IndexOf("{") - str.IndexOf("namespace"));
                        namespaceStr = namespaceStr.Trim();
                        string[] strs = namespaceStr.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                        namespaceStr = strs[1];

                        StringBuilder sb = new StringBuilder();
                        sb.Append("// Generated by Putao.PTXlsxReader only once\n");
                        sb.Append("// Free to edit\n\n");
                        sb.AppendFormat("namespace {0}\n", namespaceStr);
                        sb.Append("{\n");
                        sb.AppendFormat("\tpublic partial class {0}\n", xlsxPrefs.combineClassName);
                        sb.Append("\t{\n\n");
                        sb.Append("\t}\n");
                        sb.Append("}");

                        System.IO.File.WriteAllText(partialClassPath, sb.ToString());
                    }
                }
            }
        }

        void ExportAll()
        {
            outputStr = "";
            StringBuilder sb = new StringBuilder();
            foreach (ExcelReaderPrefs xlsxPrefs in mPrefsDatas)
            {
                if (xlsxPrefs.DataPerfect)
                {
                    Export(xlsxPrefs);
                }
                else
                {
                    sb.Append("\n" + xlsxPrefs.combineClassName);
                }
            }

            AssetDatabase.Refresh();
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                EditorUtility.DisplayDialog("", "请完善数据！" + sb.ToString(), "OK");
            }
        }

        static string MakeRelativePath(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            Uri referenceUri = new Uri(referencePath);
            string relativePath = Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            return relativePath;
        }
    }
}
