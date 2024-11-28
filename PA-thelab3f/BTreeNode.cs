using System;
using System.Collections.Generic;

namespace PAthelab3f
{
    public class BTreeNode
    {
        public List<int> Keys { get; set; }
        public List<string> Data { get; set; }
        public List<BTreeNode> Children { get; set; }
        public bool IsLeaf { get; set; }

        public BTreeNode(bool isLeaf)
        {
            Keys = new List<int>();
            Data = new List<string>();
            Children = new List<BTreeNode>();
            IsLeaf = isLeaf;
        }
    }

    public class BTree
    {
        private int t = 10;
        public BTreeNode Root { get; private set; }
        private int comparisonCount = 0;

        public BTree()
        {
            Root = new BTreeNode(true);
        }

        public void Insert(int key, string data)
        {
            if (Root.Keys.Count == (2 * t - 1))
            {
                var newRoot = new BTreeNode(false);
                newRoot.Children.Add(Root);
                SplitChild(newRoot, 0);
                Root = newRoot;
            }
            InsertNonFull(Root, key, data);
        }

        private void InsertNonFull(BTreeNode node, int key, string data)
        {
            int i = node.Keys.Count - 1;

            if (node.IsLeaf)
            {
                while (i >= 0 && key < node.Keys[i])
                    i--;

                node.Keys.Insert(i + 1, key);
                node.Data.Insert(i + 1, data);
            }
            else
            {
                while (i >= 0 && key < node.Keys[i])
                    i--;

                i++;

                if (node.Children[i].Keys.Count == (2 * t - 1))
                {
                    SplitChild(node, i);
                    if (key > node.Keys[i])
                        i++;
                }
                InsertNonFull(node.Children[i], key, data);
            }
        }

        private void SplitChild(BTreeNode parent, int index)
        {
            var fullChild = parent.Children[index];
            var newNode = new BTreeNode(fullChild.IsLeaf);

            parent.Keys.Insert(index, fullChild.Keys[t - 1]);
            parent.Data.Insert(index, fullChild.Data[t - 1]);
            parent.Children.Insert(index + 1, newNode);

            newNode.Keys.AddRange(fullChild.Keys.GetRange(t, t - 1));
            newNode.Data.AddRange(fullChild.Data.GetRange(t, t - 1));
            fullChild.Keys.RemoveRange(t - 1, t);
            fullChild.Data.RemoveRange(t - 1, t);

            if (!fullChild.IsLeaf)
            {
                newNode.Children.AddRange(fullChild.Children.GetRange(t, t));
                fullChild.Children.RemoveRange(t, t);
            }
        }

        public string Search(int key)
        {
            comparisonCount = 0;
            string result = Search(Root, key);
            Console.WriteLine($"Number of comparisons: {comparisonCount}"); 
            return result;
        }

        private string Search(BTreeNode node, int key)
        {
            int i = 0;

            while (i < node.Keys.Count && key > node.Keys[i])
            {
                comparisonCount++;
                i++;
            }

            comparisonCount++;

            if (i < node.Keys.Count && key == node.Keys[i])
                return node.Data[i];

            if (node.IsLeaf)
                return null;

            return Search(node.Children[i], key);
        }

        public void Delete(int key)
        {
            Delete(Root, key);

            if (Root.Keys.Count == 0 && !Root.IsLeaf)
                Root = Root.Children[0];
        }

        private void Delete(BTreeNode node, int key)
        {
            int idx = node.Keys.FindIndex(k => k == key);

            if (idx != -1)
            {
                if (node.IsLeaf)
                {
                    node.Keys.RemoveAt(idx);
                    node.Data.RemoveAt(idx);
                }
                else
                {
                    if (node.Children[idx].Keys.Count >= t)
                    {
                        int predKey = GetPredecessor(node, idx);
                        string predData = GetPredecessorData(node, idx);
                        node.Keys[idx] = predKey;
                        node.Data[idx] = predData;
                        Delete(node.Children[idx], predKey);
                    }
                    else if (node.Children[idx + 1].Keys.Count >= t)
                    {
                        int succKey = GetSuccessor(node, idx);
                        string succData = GetSuccessorData(node, idx);
                        node.Keys[idx] = succKey;
                        node.Data[idx] = succData;
                        Delete(node.Children[idx + 1], succKey);
                    }
                    else
                    {
                        Merge(node, idx);
                        Delete(node.Children[idx], key);
                    }
                }
            }
            else
            {
                if (node.IsLeaf)
                    return;

                bool lastChild = (idx == node.Keys.Count);
                if (node.Children[idx].Keys.Count < t)
                    Fill(node, idx);

                if (lastChild && idx > node.Keys.Count)
                    Delete(node.Children[idx - 1], key);
                else
                    Delete(node.Children[idx], key);
            }
        }

        private int GetPredecessor(BTreeNode node, int idx)
        {
            var cur = node.Children[idx];
            while (!cur.IsLeaf)
                cur = cur.Children[cur.Keys.Count];
            return cur.Keys[cur.Keys.Count - 1];
        }

        private string GetPredecessorData(BTreeNode node, int idx)
        {
            var cur = node.Children[idx];
            while (!cur.IsLeaf)
                cur = cur.Children[cur.Keys.Count];
            return cur.Data[cur.Data.Count - 1];
        }

        private int GetSuccessor(BTreeNode node, int idx)
        {
            var cur = node.Children[idx + 1];
            while (!cur.IsLeaf)
                cur = cur.Children[0];
            return cur.Keys[0];
        }

        private string GetSuccessorData(BTreeNode node, int idx)
        {
            var cur = node.Children[idx + 1];
            while (!cur.IsLeaf)
                cur = cur.Children[0];
            return cur.Data[0];
        }

        private void Merge(BTreeNode node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx + 1];

            child.Keys.Add(node.Keys[idx]);
            child.Data.Add(node.Data[idx]);
            child.Keys.AddRange(sibling.Keys);
            child.Data.AddRange(sibling.Data);

            if (!child.IsLeaf)
                child.Children.AddRange(sibling.Children);

            node.Keys.RemoveAt(idx);
            node.Data.RemoveAt(idx);
            node.Children.RemoveAt(idx + 1);
        }

        private void Fill(BTreeNode node, int idx)
        {
            if (idx > 0 && node.Children[idx - 1].Keys.Count >= t)
                BorrowFromPrev(node, idx);
            else if (idx < node.Keys.Count && node.Children[idx + 1].Keys.Count >= t)
                BorrowFromNext(node, idx);
            else
            {
                if (idx < node.Keys.Count)
                    Merge(node, idx);
                else
                    Merge(node, idx - 1);
            }
        }

        private void BorrowFromPrev(BTreeNode node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx - 1];

            child.Keys.Insert(0, node.Keys[idx - 1]);
            child.Data.Insert(0, node.Data[idx - 1]);

            if (!child.IsLeaf)
                child.Children.Insert(0, sibling.Children[sibling.Keys.Count]);

            node.Keys[idx - 1] = sibling.Keys[sibling.Keys.Count - 1];
            node.Data[idx - 1] = sibling.Data[sibling.Data.Count - 1];

            sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
            sibling.Data.RemoveAt(sibling.Data.Count - 1);
            if (!sibling.IsLeaf)
                sibling.Children.RemoveAt(sibling.Keys.Count);
        }

        private void BorrowFromNext(BTreeNode node, int idx)
        {
            var child = node.Children[idx];
            var sibling = node.Children[idx + 1];

            child.Keys.Add(node.Keys[idx]);
            child.Data.Add(node.Data[idx]);

            if (!child.IsLeaf)
                child.Children.Add(sibling.Children[0]);

            node.Keys[idx] = sibling.Keys[0];
            node.Data[idx] = sibling.Data[0];

            sibling.Keys.RemoveAt(0);
            sibling.Data.RemoveAt(0);
            if (!sibling.IsLeaf)
                sibling.Children.RemoveAt(0);
        }
    }
}
