// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PAthelab3f
{
    [Register("ViewController")]
    partial class ViewController
    {
        [Outlet]
        AppKit.NSTextField DataField { get; set; }

        [Outlet]
        AppKit.NSTextField IdField { get; set; }

        [Action("AddButton:")]
        partial void AddButton(AppKit.NSButton sender);

        [Action("DeleteButton:")]
        partial void DeleteButton(AppKit.NSButton sender);

        [Action("EditButton:")]
        partial void EditButton(AppKit.NSButton sender);

        [Action("SaveButton:")]
        partial void SaveButton(AppKit.NSButton sender);

        [Action("SearchButton:")]
        partial void SearchButton(AppKit.NSButton sender);

        void ReleaseDesignerOutlets()
        {
            if (DataField != null)
            {
                DataField.Dispose();
                DataField = null;
            }

            if (IdField != null)
            {
                IdField.Dispose();
                IdField = null;
            }
        }
    }
}
