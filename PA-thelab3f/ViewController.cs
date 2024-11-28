using Foundation;
using System;
using AppKit;
using System.IO;

namespace PAthelab3f
{
    public partial class ViewController : NSViewController
    {
        private readonly BTree bTree;

        public ViewController(IntPtr handle) : base(handle)
        {
            bTree = new BTree();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        partial void AddButton(NSButton sender)
        {
            if (int.TryParse(IdField.StringValue, out int id) && !string.IsNullOrEmpty(DataField.StringValue))
            {
                bTree.Insert(id, DataField.StringValue);
                ShowMessage("Запис додано!");
                ClearFields();
            }
            else
            {
                ShowMessage("Некоректні дані. Введіть числовий ID та текстові дані.");
            }
        }

        partial void DeleteButton(NSButton sender)
        {
            if (int.TryParse(IdField.StringValue, out int id))
            {
                string result = bTree.Search(id);
                if (result != null)
                {
                    bTree.Delete(id);
                    ShowMessage("Запис видалено.");
                }
                else
                {
                    ShowMessage("Запис із таким ID не знайдено.");
                }
                ClearFields();
            }
            else
            {
                ShowMessage("Некоректний ID. Введіть числове значення.");
            }
        }

        partial void EditButton(NSButton sender)
        {
            if (int.TryParse(IdField.StringValue, out int id) && !string.IsNullOrEmpty(DataField.StringValue))
            {
                string result = bTree.Search(id);
                if (result != null)
                {
                    bTree.Delete(id);
                    bTree.Insert(id, DataField.StringValue);
                    ShowMessage("Запис відредаговано успішно!");
                }
                else
                {
                    ShowMessage("Запис із таким ID не знайдено.");
                }
                ClearFields();
            }
            else
            {
                ShowMessage("Некоректні дані. Введіть числовий ID та текстові дані.");
            }
        }

        partial void SearchButton(NSButton sender)
        {
            if (int.TryParse(IdField.StringValue, out int id))
            {
                string result = bTree.Search(id);
                if (result != null)
                {
                    DataField.StringValue = result;
                    ShowMessage($"Запис знайдено: {result}");
                }
                else
                {
                    ShowMessage("Запис із таким ID не знайдено.");
                }
            }
            else
            {
                ShowMessage("Некоректний ID. Введіть числове значення.");
            }
        }

        partial void SaveButton(NSButton sender)
        {
            var savePanel = new NSSavePanel
            {
                AllowedFileTypes = new string[] { "txt" },
                Title = "Зберегти файл"
            };

            savePanel.Begin((result) =>
            {
                if (result == 1 && savePanel.Url != null)
                {
                    string filePath = savePanel.Url.Path;
                    try
                    {
                        SaveToFile(filePath);
                        ShowMessage("Дані успішно збережено у файл.");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Помилка при збереженні даних: {ex.Message}");
                    }
                }
                else
                {
                    ShowMessage("Збереження скасовано.");
                }
            });
        }

        private void SaveToFile(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                SaveNodeToFile(writer, bTree.Root);
            }
        }

        private void SaveNodeToFile(StreamWriter writer, BTreeNode node)
        {
            for (int i = 0; i < node.Keys.Count; i++)
            {
                writer.WriteLine($"{node.Keys[i]}: {node.Data[i]}");
            }

            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    SaveNodeToFile(writer, child);
                }
            }
        }

        private void ClearFields()
        {
            IdField.StringValue = string.Empty;
            DataField.StringValue = string.Empty;
        }

        private void ShowMessage(string message)
        {
            var alert = new NSAlert
            {
                MessageText = message
            };
            alert.AddButton("ОК");
            alert.RunModal();
        }
    }
}
