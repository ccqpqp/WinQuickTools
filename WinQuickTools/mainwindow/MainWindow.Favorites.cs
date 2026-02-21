using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        private void InitDefaultFavorites()
        {
            string[] defaults =
            {
                "ext",          // 확장자 표시
                "hidden",       // 숨김파일
                "restart",      // 탐색기 재시작
                "copypath",     // 경로 복사
                "quickcapture", // F1 캡처
                "restoreall"   // ⭐ 추가 (파일 이름 정리)
            };

            foreach (var id in defaults)
            {
                var item = FindItem(id);
                if (item == null) continue;

                item.IsFavorite = true;
                PutIntoFirstEmptySlot(item);
            }

            RebuildNonFavorites();
        }



        private void SaveFavorites()
        {
            try
            {
                var dir = Path.GetDirectoryName(_favPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir!);

                var favIds = _allItems
                    .Where(x => x.IsFavorite)
                    .Select(x => x.Id)
                    .ToList();

                File.WriteAllText(_favPath,
                    JsonSerializer.Serialize(favIds));
                SetStatus("저장됨");
            }
            catch { }
        }

        private bool LoadFavorites()
        {
            if (!File.Exists(_favPath))
                return false;

            try
            {
                var json = File.ReadAllText(_favPath);
                var favIds = JsonSerializer.Deserialize<List<string>>(json);

                if (favIds == null || favIds.Count == 0)
                    return false;

                foreach (var id in favIds)
                {
                    var item = FindItem(id);
                    if (item == null) continue;

                    item.IsFavorite = true;
                    PutIntoFirstEmptySlot(item);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private FeatureItem? FindItem(string id)
        {
            var item = _allItems.FirstOrDefault(x => x.Id == id);
            if (item != null) return item;

            return RightClickItems.FirstOrDefault(x => x.Id == id);
        }

        private void PutIntoFirstEmptySlot(FeatureItem item)
        {
            for (int i = 0; i < FavoriteSlots.Count; i++)
            {
                if (FavoriteSlots[i] == null)
                {
                    FavoriteSlots[i] = item;
                    return;
                }
            }
        }

        private void RemoveFromSlots(FeatureItem item)
        {
            for (int i = 0; i < FavoriteSlots.Count; i++)
            {
                if (ReferenceEquals(FavoriteSlots[i], item))
                {
                    FavoriteSlots[i] = null;
                    return;
                }
            }
        }

        private int FavoriteCount()
            => FavoriteSlots.Count(x => x != null);

        private void RebuildNonFavorites()
        {
            NonFavoriteItems.Clear();
            foreach (var it in _allItems.Where(x => !x.IsFavorite))
                NonFavoriteItems.Add(it);
        }

        private void ToggleFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.Tag is not string id) return;

            var item = FindItem(id);
            if (item == null) return;

            if (item.IsFavorite)
            {
                // ⭐ 해제
                item.IsFavorite = false;
                RemoveFromSlots(item);
                RebuildNonFavorites();
            }
            else
            {
                // ⭐ 추가
                if (FavoriteCount() >= 6)
                {
                    SetStatus("즐겨찾기는 최대 6개까지 가능합니다.");
                    return;
                }

                item.IsFavorite = true;
                PutIntoFirstEmptySlot(item);
                RebuildNonFavorites();
            }

            // ⭐⭐⭐ 여기로 이동 (핵심)
            SaveFavorites();
            SetStatus("저장됨");

            e.Handled = true;
        }
    }
}