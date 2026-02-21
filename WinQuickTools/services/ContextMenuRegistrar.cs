using Microsoft.Win32;
using System.Diagnostics;

namespace WinQuickTools.Services
{
    internal static class ContextMenuRegistrar
    {
        private static readonly string[] ROOTS =
        {
            @"Software\Classes\*\shell",
            @"Software\Classes\Directory\shell",
            @"Software\Classes\Directory\Background\shell"
        };

        public static bool IsEnabled()
        {
            using var k = Registry.CurrentUser.OpenSubKey(
                @"Software\Classes\*\shell\WinQuickTools.exportlist");

            return k != null;
        }

        public static void Enable()
        {
            string exe = Process.GetCurrentProcess().MainModule!.FileName!;

            Create("exportlist", "파일 목록 만들기 (WinQuickTools)", exe);
            Create("renamebatch", "파일 이름 정리 (WinQuickTools)", exe);
            Create("renumber", "번호 자동 붙이기 (WinQuickTools)", exe);
            Create("fullpathcopy", "전체 경로 복사 (WinQuickTools)", exe);
        }

        public static void Disable()
        {
            foreach (var root in ROOTS)
            {
                Delete(root + @"\WinQuickTools.exportlist");
                Delete(root + @"\WinQuickTools.renamebatch");
                Delete(root + @"\WinQuickTools.renumber");
                Delete(root + @"\WinQuickTools.fullpathcopy");
            }
        }

        private static void Create(string id, string text, string exe)
        {
            foreach (var root in ROOTS)
            {
                using var key =
                    Registry.CurrentUser.CreateSubKey(
                        $@"{root}\WinQuickTools.{id}");

                key.SetValue("", text);
                key.SetValue("Icon", exe);

                using var cmd = key.CreateSubKey("command");

                string arg = root.Contains("Background")
                    ? "\"%V\""
                    : "\"%1\"";

                cmd.SetValue("", $"\"{exe}\" {id} {arg}");
            }
        }

        private static void Delete(string path)
        {
            Registry.CurrentUser.DeleteSubKeyTree(path, false);
        }
    }
}