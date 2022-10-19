using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Kogane
{
    /// <summary>
    /// iOS ビルド完了時に Xcode プロジェクトの Product Name を設定するエディタ拡張
    /// Xcode プロジェクトの Product Name に日本語が含まれていると
    /// 通知許可ダイアログを正常に開くことができない
    /// https://developer.apple.com/forums/thread/660715
    /// Unity で iOS ビルドすると Display Name と Product Name に同じ文字列が設定されてしまうため
    /// Display Name に日本語が含まれていると Product Name にも日本語が設定されてしまう
    /// そのため、iOS ビルド完了後に Xcode プロジェクトの Product Name を
    /// 直接英数字の文字列に書き換えられるようにするエディタ拡張
    /// </summary>
    public static class SetXcodeProductNameOnPostProcessBuild
    {
        //================================================================================
        // プロパティ(static)
        //================================================================================
        public static Func<string> OnGetProductName { get; set; }

        //================================================================================
        // 関数(static)
        //================================================================================
        /// <summary>
        /// ビルド完了時に呼び出されます
        /// </summary>
        [PostProcessBuild]
        private static void OnPostProcessBuild
        (
            BuildTarget buildTarget,
            string      pathToBuiltProject
        )
        {
            if ( buildTarget != BuildTarget.iOS ) return;
            if ( OnGetProductName == null ) return;

            var projectPath = PBXProject.GetPBXProjectPath( pathToBuiltProject );
            var project     = new PBXProject();

            project.ReadFromFile( projectPath );

            var targetGuid = project.GetUnityMainTargetGuid();

            project.SetBuildProperty( targetGuid, "PRODUCT_NAME", OnGetProductName() );
            project.WriteToFile( projectPath );
        }
    }
}