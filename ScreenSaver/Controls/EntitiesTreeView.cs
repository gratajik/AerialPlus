﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aerial.Controls
{
    public class EntitiesTreeView : TreeView
    {
        private Dictionary<string, Asset> Movies;
        private bool updatingChecked = false;
        public EntitiesTreeView()
        {
            CheckBoxes = true;
            FullRowSelect = true;
            Location = new System.Drawing.Point(7, 19);
            Name = "tvMovies";
            ShowLines = false;
            ShowPlusMinus = false;
            Size = new System.Drawing.Size(138, 229);
            TabIndex = 15;
            AfterCheck += new TreeViewEventHandler(tvMovies_AfterCheck);
        }

        public void BuildTree(List<Asset> movies, List<string> selectedEntities)
        {
            Trace.WriteLine("BuildTree()");

            var image = Image.FromFile(@"d:\ChinaFlag.png");

            //ImageList = new ImageList();
            //ImageList.ImageSize = new Size(50, 50);
            //this.ImageList.Images.Add("china", image);


            updatingChecked = true;
            TreeNode root = new TreeNode(movies[0].accessibilityLabel);
            Nodes.Add(root);
            Movies = new Dictionary<string, Asset>();
            bool allChecked = true;
            foreach (var movie in movies)
            {
                if (movie.accessibilityLabel != root.Text)
                {
                    // checked root
                    if (allChecked) root.Checked = true;
                    // new root
                    root = new TreeNode(movie.accessibilityLabel);
                    Nodes.Add(root);
                }
                // add node
                var newNode = new TreeNode(movie.TimeAndIdNumbered());
               // newNode.SelectedImageKey ="china";
                
                root.Nodes.Add(newNode);
                newNode.Checked = selectedEntities.Contains(newNode.FullPath);
                allChecked = allChecked && newNode.Checked;
                Trace.WriteLine($"  Adding {movie.id}, {movie.ShortName()}, {movie.ToFullName()}");
                Movies.Add(root.Nodes[root.Nodes.Count - 1].FullPath, movie);
            }

            ExpandAll();

            updatingChecked = false;

            StartPlayer();
        }

        private void StartPlayer()
        {
            SelectedNode = Nodes[0].Nodes[0];
            Select();
            TopNode.EnsureVisible();
            Nodes[0].EnsureVisible();
        }

        public string ConcatChosenEntities()
        {
            var selected = "";
            foreach (TreeNode root in Nodes)
                foreach (TreeNode n in root.Nodes)
                    if (n.Checked)
                        selected += ";" + n.FullPath;

            return selected + ";";
        }


        private void tvMovies_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (updatingChecked) return;

            updatingChecked = true;

            if (!e.Node.FullPath.Contains("\\"))
            {
                foreach (TreeNode node in e.Node.Nodes)
                    node.Checked = e.Node.Checked;
            }
            else
            {
                foreach (TreeNode n in e.Node.Parent.Nodes)
                {
                    if (!n.Checked)
                    {
                        e.Node.Parent.Checked = false;
                        return;
                    }
                }
                e.Node.Parent.Checked = true;
            }

            updatingChecked = false;
        }

        internal string GetUrl(string fullPath)
        {
            return Movies[fullPath].url;
        }
    }
}
