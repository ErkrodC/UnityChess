// Author:
//       weezah <ts_matt@hotmail.com>
//
// Copyright (c) 2015 weezah
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class UnitTestSolutionPostProcessor : AssetPostprocessor
{
	// solution file name
	private const string SolutionFilename = "UnityChess.sln";
	
	// unit test project folder
	private const string UnitTestPrjPath = "UnityChess.Test";
	
	// unit test project file
	private const string UnitTestPrjFilename = "UnityChess.Test.csproj";
	//
	// unit test project name (as will appear in the solution)
	private const string UnitTestPrjName = "UnityChess.Test";


	private const string MsbuildSchema = "http://schemas.microsoft.com/developer/msbuild/2003"; // ms build schema needed for XmlNamespaceManager
	private const string PrjTemplate = "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"\r\nEndProject\r\n"; //slnGuid, prjName, prjFullPath, prjGuid
	private const string BuildConfigLine = "GlobalSection(ProjectConfigurationPlatforms) = postSolution";
	private const string BuildConfigTemplate = "\r\n\t\t{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n\t\t{0}.Debug|Any CPU.Build.0 = Debug|Any CPU"; //prjGuid

	private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		string currentDir = Directory.GetCurrentDirectory ();
		string slnPath = Path.Combine (currentDir, SolutionFilename);
		string utFullPath = Path.Combine (UnitTestPrjPath, UnitTestPrjFilename);

		try {

			// check if project is already in the sln
			string slnFileContent = File.ReadAllText (slnPath);	
			Regex prjRegex = new Regex (UnitTestPrjFilename);
			if (prjRegex.IsMatch (slnFileContent))
				return;

			// get unit test project guid
			XmlDocument unitTestPrj = new XmlDocument ();
			unitTestPrj.LoadXml (File.ReadAllText (Path.Combine (currentDir, utFullPath)));
			XmlNamespaceManager nsmgr = new XmlNamespaceManager (unitTestPrj.NameTable);
			nsmgr.AddNamespace ("ms", MsbuildSchema);
			string utGuid = unitTestPrj.SelectSingleNode ("//ms:ProjectGuid", nsmgr)?.InnerText;

			// get slnGuid
			Regex slnGuidRegex = new Regex ("Project[(]\"(.*)\"[)]");
			Match m = slnGuidRegex.Match (slnFileContent);
			string slnGuid = m.Groups [1].ToString ();

			// inject project
			int idx = slnFileContent.IndexOf ("Global", StringComparison.Ordinal);
			slnFileContent = slnFileContent.Insert (idx, string.Format (PrjTemplate, slnGuid, UnitTestPrjName, utFullPath, utGuid));

			// inject build config
			idx = slnFileContent.IndexOf (BuildConfigLine, StringComparison.Ordinal) + BuildConfigLine.Length;
			slnFileContent = slnFileContent.Insert (idx, string.Format (BuildConfigTemplate, utGuid));

			File.WriteAllText (slnPath, slnFileContent);

		} catch (Exception ex) {
			UnityEngine.Debug.LogErrorFormat ("UnitTestSolutionPostProcessor: {0}\n{1}", ex.Message, ex.StackTrace);
		}
	}
}
