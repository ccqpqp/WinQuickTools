// 파일: features/FileFeatures.cs  (전체 교체)
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WinForms = System.Windows.Forms;
using WinQuickTools.Windows;

namespace WinQuickTools.Features
{
    internal static class FileFeatures
    {
        private static bool Confirm(string title, string desc) => EtDialog.Confirm(title, desc);

        private static string? PickFolder()
        {
            using var dialog = new WinForms.FolderBrowserDialog
            {
                Description = "작업할 폴더 선택"
            };

            return dialog.ShowDialog() == WinForms.DialogResult.OK
                ? dialog.SelectedPath
                : null;
        }

        private static string? PickFile()
        {
            using var dialog = new WinForms.OpenFileDialog
            {
                Title = "파일 선택"
            };

            return dialog.ShowDialog() == WinForms.DialogResult.OK
                ? dialog.FileName
                : null;
        }

        public static void ExportList(string? _ = null)
        {
            if (!Confirm(
                "파일 목록 만들기",
                "선택한 폴더 안의 파일 이름 목록을\nfile_list.txt로 생성합니다.\n\n계속하시겠습니까?"))
                return;

            var path = PickFolder();
            if (path == null) return;

            var files = Directory.GetFiles(path)
                                 .Select(f => Path.GetFileName(f) ?? "");

            string output = Path.Combine(path, "file_list.txt");
            File.WriteAllLines(output, files, Encoding.UTF8);

            EtDialog.Alert("완료", output);
        }

        public static void RenameBatch(string? _ = null)
        {
            if (!Confirm(
                "파일 이름 정리",
                "파일명 앞뒤 공백 및\n중복 공백을 자동 정리합니다.\n\n계속하시겠습니까?"))
                return;

            var path = PickFolder();
            if (path == null) return;

            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                string dir = Path.GetDirectoryName(file)!;
                string name = Path.GetFileName(file);

                string clean = name.Replace("  ", " ").Trim();

                File.Move(file, Path.Combine(dir, clean), true);
            }

            EtDialog.Alert("WinQuickTools", "파일 이름 정리 완료");
        }

        public static void Renumber(string? _ = null)
        {
            var format = AskNumberFormat();
            if (format == null) return;

            bool keepName = EtDialog.Confirm(
                "파일명 처리",
@"기존 파일명을 유지할까요?

확인 → 번호 + 기존파일명
취소 → 번호만 사용");

            var path = PickFolder();
            if (path == null) return;

            var files = Directory.GetFiles(path)
                                 .OrderBy(f => f)
                                 .ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                string dir = Path.GetDirectoryName(files[i])!;
                string ext = Path.GetExtension(files[i]);
                string originalName = Path.GetFileNameWithoutExtension(files[i]);

                string number = format switch
                {
                    "n" => $"{i + 1}",
                    "n2" => $"{i + 1:00}",
                    "n3" => $"{i + 1:000}",
                    "date" => DateTime.Now.AddDays(i).ToString("yyyyMMdd"),
                    _ => $"{i + 1}"
                };

                string newName = keepName
                    ? $"{number}_{originalName}{ext}"
                    : $"{number}{ext}";

                File.Move(files[i], Path.Combine(dir, newName), true);
            }

            EtDialog.Alert("완료", "번호 붙이기 완료");
        }

        // ✅ FileFeatures.cs — AskNumberFormat 부분만 교체 (이거만 바꿔)

        private static string? AskNumberFormat()
        {
            var choice = EtDialog.Select(
                "번호 형식",
                "번호 형식을 선택하세요",
                new()
                {
            "001,002,003 (3자리)",
            "01,02,03 (2자리)",
            "1,2,3 (기본)",
            "날짜 yyyyMMdd"
                });

            return choice switch
            {
                "001,002,003 (3자리)" => "n3",
                "01,02,03 (2자리)" => "n2",
                "1,2,3 (기본)" => "n",
                "날짜 yyyyMMdd" => "date",
                _ => null
            };
        }

        public static void FullPathCopy(string? _ = null)
        {
            if (!Confirm(
                "파일 경로 복사",
                "선택한 파일의 전체 경로를\n클립보드에 복사합니다.\n\n계속하시겠습니까?"))
                return;

            var file = PickFile();
            if (file == null) return;

            Clipboard.SetText(file);

            EtDialog.Alert("WinQuickTools", "경로 복사 완료");
        }
    }
}