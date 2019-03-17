using GalaSoft.MvvmLight;
using SatisfactorySaveEditor.Model;
using SatisfactorySaveParser;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using SatisfactorySaveEditor.Util;

namespace SatisfactorySaveEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SaveNodeItem rootItem;
        private SaveNodeItem selectedItem;

        public ObservableCollection<SaveNodeItem> RootItem => new ObservableCollection<SaveNodeItem> { rootItem };

        public SaveNodeItem SelectedItem
        {
            get => selectedItem;
            set { Set(() => SelectedItem, ref selectedItem, value); }
        }

        public RelayCommand<SaveNodeItem> TreeSelectCommand { get; }

        public MainViewModel()
        {
            var save = new SatisfactorySave(@"%userprofile%\Documents\My Games\FactoryGame\SaveGame\space war_090319-135233 - Copy.sav");

            rootItem = new SaveNodeItem("Root");
            var saveTree = new EditorTreeNode("Root");

            foreach (var entry in save.Entries)
            {
                var parts = entry.TypePath.TrimStart('/').Split('/');
                saveTree.AddChild(parts, entry);
            }

            BuildNode(rootItem.Items, saveTree);

            TreeSelectCommand = new RelayCommand<SaveNodeItem>(SelectNode);
        }

        private void SelectNode(SaveNodeItem node)
        {
            SelectedItem = node;
        }

        private void BuildNode(ObservableCollection<SaveNodeItem> items, EditorTreeNode node)
        {
            foreach (var child in node.Children)
            {
                var childItem = new SaveNodeItem(child.Value.Name);
                BuildNode(childItem.Items, child.Value);
                items.Add(childItem);
            }

            foreach (var entry in node.Content)
            {
                items.Add(new SaveNodeItem(entry.InstanceName, entry.DataFields));
            }
        }
    }
}